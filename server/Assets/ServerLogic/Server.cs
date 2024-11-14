using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

// todo: handle clients disconnecting lmao
public class Server : MonoBehaviour {


    public delegate void HandlePacket(Client client);

    // Structs are value types which means they are copied when they are passed around.
    // So if you change a copy you are changing only that copy, not the original and not any other copies which might be around. 
    public struct Client {
        public PacketHandler packet_handler;
        public NetworkStream stream;
        public TcpClient client;
        public int id;
        public string username;
        public bool leaving;
        public bool connection_dropped;
        public Delay timeout;
    }

    public struct ClientEventInfo {
        public byte packet;
        public Client client;

        public ClientEventInfo(byte packet, Client client) {
            this.packet = packet;
            this.client = client;
        }    
    }

    TcpListener server;
    Thread server_incoming_connection_thread;
    public List<Client> clients = new List<Client>(Definitions.SERVER_CLIENT_CAPACITY);
    // todo: support for many events at once
    public Dictionary<byte, HandlePacket> packet_handlers = new Dictionary<byte, HandlePacket>();
    public List<ClientEventInfo> client_event_queue = new List<ClientEventInfo>();

    int next_id = 0;
    Vector3 starting_position = new Vector3(-12, 1, 7);
    Delay ping_loop = new Delay(500);

    void Start() {
        server = new TcpListener(IPAddress.Any, Definitions.LISTENER_PORT);  
        server.Start();
        Debug.Log($"Server hosting on port {Definitions.LISTENER_PORT}");

        server_incoming_connection_thread = new Thread(new ThreadStart(ListenForClients));
        server_incoming_connection_thread.Start();
    }

	public void RegisterPacketHandler(byte target, HandlePacket function) {
		packet_handlers[target] = function;
	}

    void ListenForClients() {
        Debug.Log($"Listening for clients");

        while (true) {
            TcpClient tcp_client = server.AcceptTcpClient();  
            NetworkStream stream = tcp_client.GetStream();

            // temporary, probably
            ushort id = (ushort)(next_id + 1);
            next_id++;
            
            Client new_client = new Client();
            new_client.client = tcp_client;
            new_client.packet_handler = new PacketHandler(stream);
            new_client.stream = stream;
            new_client.id = id;
            new_client.timeout = new Delay(4000);

            Debug.Log($"client ID {new_client.id} joined, init handshake");
            new_client.packet_handler.WriteHeader(Packet.SERVER_INIT_HANDSHAKE);
            new_client.packet_handler.WriteInt(id);
            new_client.packet_handler.WriteVector3(starting_position);

            // replicate all existing players
            clients.ForEach(client => {
                if (client.id != new_client.id) {
                    client.packet_handler.WriteHeader(Packet.SERVER_ADD_PLAYER);
                    client.packet_handler.WriteInt(new_client.id);
                    client.packet_handler.WriteVector3(starting_position);
                }
            });

            // replicate new player to everyone already ingame
            clients.ForEach(client => {
                if (client.id != new_client.id) {
                    new_client.packet_handler.WriteHeader(Packet.SERVER_ADD_PLAYER);
                    new_client.packet_handler.WriteInt(client.id);
                    new_client.packet_handler.WriteVector3(starting_position);

                    if (client.username != null) {
                        new_client.packet_handler.WriteHeader(Packet.SERVER_SET_USERNAME);
                        new_client.packet_handler.WriteInt(client.id);
                        new_client.packet_handler.WriteString(client.username);
                    }
                }
            });

            clients.Add(new_client);
        }
    }

    void FixedUpdate() {
        // if (!server.Server.) {
        //     Debug.Log("Server disconnected");
        //     // server.Server.Close();
        //     // Start();
        //     return;
        // };

        for (int i = 0; i < clients.Count; i++) {
            Client client = clients[i]; 

            // if the client does not send over any data for 4 seconds, the connection is assumed dead
            if (client.timeout.expired()) {
                client.leaving = true;
                client.connection_dropped = true;
                client_event_queue.Add(new ClientEventInfo(Packet._PLAYER_REMOVED, client));
                clients[i] = client;
                continue;
            }

            if (ping_loop.expired()) {
                ping_loop.reset();
				client.packet_handler.WriteHeader(Packet.SERVER_PING);
                Debug.Log("sending keepalive ping");
            }

            if (client.stream.DataAvailable) {
                client.timeout.reset();
				
                // why does ReadByte not return a byte?????
                int header = client.stream.ReadByte();
                if (header == Packet.CLIENT_HANDSHAKE) {
                    string username = client.packet_handler.ReadString();
                    Debug.Log($"received handshake username {username}");
                    client.username = username;
                    clients.ForEach(recv => {
                        if (recv.id != client.id) {
                            recv.packet_handler.WriteHeader(Packet.SERVER_SET_USERNAME);
                            recv.packet_handler.WriteInt(client.id);
                            recv.packet_handler.WriteString(username);
                        }
                    });

                    client_event_queue.Add(new ClientEventInfo(Packet._PLAYER_ADDED, client));
                } else if (header == Packet.CLIENT_EXIT) {
                    client.leaving = true;
                    client_event_queue.Add(new ClientEventInfo(Packet._PLAYER_REMOVED, client));
                } else {
                    client_event_queue.Add(new ClientEventInfo((byte)header, client));
                }
            } 

            clients[i] = client;
        }

        // todo: handle unexpected socket disconnect
        for (int i = 0; i < clients.Count; i++) {
            Client client = clients[i];
            if (client.leaving) {
                clients.ForEach(recv => {
                    if (recv.id != client.id) {
                        recv.packet_handler.WriteHeader(Packet.SERVER_REMOVE_PLAYER);
                        recv.packet_handler.WriteInt(client.id);
                        recv.packet_handler.WriteBool(client.connection_dropped);
                    }
                });

                Debug.Log($"client {client.id} disconnected");
                clients.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < client_event_queue.Count; i++) {
            ClientEventInfo info = client_event_queue[i];
            packet_handlers[info.packet](info.client);
        }

        client_event_queue.Clear();
    }
}