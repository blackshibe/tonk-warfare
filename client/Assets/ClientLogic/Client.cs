using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour {

    TcpClient client = new TcpClient();  
    Thread client_connection_thread;
    NetworkStream client_stream;
    StreamWrite stream_write;

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
        stream_write = new StreamWrite(client_stream);
        connected = true;
    }

    void FixedUpdate() {
        if (connected)
            if (client_stream.DataAvailable) {
                int header = client_stream.ReadByte();

                if (header == Packet.SERVER_INIT_HANDSHAKE) {
                    local_id = stream_write.ReadU16();
                    Debug.Log($"received local ID {local_id}");
                    stream_write.WriteHeader(Packet.CLIENT_HANDSHAKE);
                    stream_write.WriteString(local_username);
                } 
            }
    }
}
