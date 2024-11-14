using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

class JesusChrist {
    public string reason;
    public bool use_reason;
}

static class ClientDisconnectReason {
    public static JesusChrist data = new JesusChrist();
}

public class Client : MonoBehaviour {

    Thread client_connection_thread;

    public TcpClient client = new TcpClient();  
    public NetworkStream stream;
    public PacketHandler packet_handler;
    public VehiclePhysics local_vehicle;
    public delegate void HandlePacket();
    public Dictionary<byte, HandlePacket> packet_handlers = new Dictionary<byte, HandlePacket>();

    public string ip = "127.0.0.1";
    public string local_username = "guest";

    public int local_id = -1;
    public bool connected = false;
    public bool ingame = false;
    
    // 60hz hopefully
    Delay replication_timer = new Delay((1/60) * 1000);
    Delay server_timeout_timer = new Delay(3500);
    float last_packet;

    void Start() {}

    // todo: remove
    public string connection_result = "";
    public bool connection_status = true;
    public async void Connect() {
        Debug.Log($"Client.cs Connect() to localhost:{Definitions.LISTENER_PORT}");
        last_packet = Time.timeSinceLevelLoad;

        try {
            await client.ConnectAsync(ip, Definitions.LISTENER_PORT);
            client_connection_thread = new Thread(new ThreadStart(ClientAction));
            client_connection_thread.Start();
            
            connection_result = "Connected";
            connection_status = true;
        } catch (System.Net.Sockets.SocketException except) {
            connection_result = except.Message;
            connection_status = false;
        }
    }

    void ClientAction() {
        Debug.Log($"Doing something probably");
        stream = client.GetStream();
        packet_handler = new PacketHandler(stream);
        connected = true;
    }

    public void RegisterPacketHandler(byte target, HandlePacket function) {
		packet_handlers[target] = function;
	}

    void FixedUpdate() {
        if (connected) {
            if (!client.Connected) {
                connected = false;
                return;
            }

            // handle incoming data
            if (stream.DataAvailable) {
                int header = stream.ReadByte();
                last_packet = Time.timeSinceLevelLoad;

                Debug.Log(header);
                if (header == Packet.SERVER_INIT_HANDSHAKE) {
                    local_id = packet_handler.ReadInt();
                    // The position is read later on by Menu.cs
                    // wow this codebase sucks ass

                    Debug.Log($"received local ID {local_id}");
                    packet_handler.WriteHeader(Packet.CLIENT_HANDSHAKE);
                    packet_handler.WriteString(local_username);
                } else if (header == Packet.SERVER_PING) {
                    // lol
                } 

                if (packet_handlers[(byte)header] != null) packet_handlers[(byte)header]();
                else throw new System.Exception($"Unhandled incoming packet! ID: {(byte)header}");
            }

            // timeout
            // if ((Time.timeSinceLevelLoad - last_packet) > 5) {
            //     Debug.Log("Client timeout");

            //     connected = false;
            //     ClientDisconnectReason.data.reason = "Server timeout (5 seconds without reply)";
            //     ClientDisconnectReason.data.use_reason = true;
            //     return;
            // }

            // client replication
            if (replication_timer.expired() && ingame) {
                replication_timer.reset();
                packet_handler.WriteHeader(Packet.CLIENT_REPLICATE);
                packet_handler.WriteVector3(local_vehicle.tank.position);
                packet_handler.WriteVector3(local_vehicle.tank.transform.rotation.eulerAngles);
                packet_handler.WriteVector3(local_vehicle.tank.velocity);
                packet_handler.WriteFloat(local_vehicle.camera_rotation);
                packet_handler.WriteFloat(local_vehicle.camera_pitch);
            }
        }

    }

    void OnApplicationQuit() {
        if (packet_handler != null) {
            packet_handler.WriteHeader(Packet.CLIENT_EXIT);
            Debug.Log("quitting");
        }
    }
}