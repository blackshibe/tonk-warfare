using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour {

    TcpClient client = new TcpClient();  
    Thread client_connection_thread;
    NetworkStream client_stream;
    PacketHandler packet_handler;

    public string ip = "127.0.0.1";
    public bool connected = false;

    public int local_id = -1;
    public string local_username = "guest";

    void Start() {}

    public async void Connect() {
        Debug.Log($"connecting to localhost:{Definitions.LISTENER_PORT}");
        // todo: retry connecting
        await client.ConnectAsync(ip, Definitions.LISTENER_PORT);
        Debug.Log($"Connected, probably");

        client_connection_thread = new Thread(new ThreadStart(ClientAction));
        client_connection_thread.Start();
    }

    void ClientAction() {
        Debug.Log($"Doing something probably");
        client_stream = client.GetStream();
        packet_handler = new PacketHandler(client_stream);
        connected = true;
    }

    void FixedUpdate() {
        if (connected)
            if (client_stream.DataAvailable) {
                int header = client_stream.ReadByte();

                if (header == Packet.SERVER_INIT_HANDSHAKE) {
                    local_id = packet_handler.ReadU16();
                    Debug.Log($"received local ID {local_id}");
                    packet_handler.WriteHeader(Packet.CLIENT_HANDSHAKE);
                    packet_handler.WriteString(local_username);
                } 
            }
    }
}
