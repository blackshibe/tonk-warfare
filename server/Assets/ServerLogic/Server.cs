using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class Server : MonoBehaviour {

    struct ServerClient {
        public StreamWrite stream_write;
        public NetworkStream stream;
        public TcpClient client;
        public int id;
        public string username;
    }

    TcpListener server = new TcpListener(IPAddress.Any, Definitions.LISTENER_PORT);  
    Thread server_incoming_connection_thread;
    List<ServerClient> clients = new List<ServerClient>(Definitions.SERVER_CLIENT_CAPACITY);

    ushort next_id = 0;

    void Start() {
        server.Start();
        Debug.Log($"Server hosting on port {Definitions.LISTENER_PORT}");

        server_incoming_connection_thread = new Thread(new ThreadStart(ListenForClients));
        server_incoming_connection_thread.Start();
    }

    void ListenForClients() {
        Debug.Log($"Listening for clients");

        while (true) {
            TcpClient tcp_client = server.AcceptTcpClient();  
            NetworkStream stream = tcp_client.GetStream();

            // temporary, probably
            ushort id = (ushort)(next_id + 1);
            next_id++;
            
            ServerClient client = new ServerClient();
            client.client = tcp_client;
            client.stream_write = new StreamWrite(stream);
            client.stream = stream;
            client.id = id;

            Debug.Log($"client ID {client.id} joined, init handshake");
            stream.WriteByte(Packet.SERVER_INIT_HANDSHAKE);
            client.stream_write.WriteU16(id);
            clients.Add(client);
        }
    }

    void FixedUpdate() {
        clients.ForEach(client => {
            if (client.stream.DataAvailable) {
                int header = client.stream.ReadByte();

                if (header == Packet.CLIENT_HANDSHAKE) {
                    string username = client.stream_write.ReadString();
                    Debug.Log($"received handshake username {username}");
                    client.username = username;
                }
            } 
        });
        // foreach (var client in clients) {}
    }
}