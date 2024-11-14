using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerChat : MonoBehaviour
{

    public struct ChatInfo {
        public Color color;
    }

    public Color[] colors = {
        Color.red,
        Color.white,
        Color.blue,
        Color.green,
        Color.cyan
    };

    public Server server;
    public Dictionary<int, ChatInfo> clients = new Dictionary<int, ChatInfo>();
    
    void Start() {
        server.RegisterPacketHandler(Packet.CLIENT_CHAT_MESSAGE, HandleChatRequest);
        server.RegisterPacketHandler(Packet._PLAYER_ADDED, HandlePlayerAdded);
        server.RegisterPacketHandler(Packet._PLAYER_REMOVED, HandlePlayerRemoved);
    }

    public void ServerChatMessage(string message) {
        server.clients.ForEach(client => {
            client.packet_handler.WriteHeader(Packet.SERVER_CHAT_MESSAGE);
            client.packet_handler.WriteBool(true);
            client.packet_handler.WriteString(message);
        });
    }

    // Client chat request
    void HandleChatRequest(Server.Client client) {
        ChatInfo sender_chat_info = clients[client.id];

        string message = client.packet_handler.ReadString();
        Debug.Log("Chatlog " + client.username + ": " + message);
        
        // no spamerino (((
        if (message.Length > 200) return;
        if (message == "" || message == " ") return;
        
        server.clients.ForEach(recv => {
            recv.packet_handler.WriteHeader(Packet.SERVER_CHAT_MESSAGE);
            recv.packet_handler.WriteBool(false);
            recv.packet_handler.WriteInt(client.id);
            recv.packet_handler.WriteColor(sender_chat_info.color);
            recv.packet_handler.WriteString(message);
        });
    }

    void HandlePlayerAdded(Server.Client client) {
        int random = Random.Range(0, colors.Length);
        ChatInfo chat_info = new ChatInfo();
        chat_info.color = colors[random];
        clients[client.id] = chat_info;

        ServerChatMessage($"{client.username} joined!!!");
    }

    void HandlePlayerRemoved(Server.Client client) {
        clients.Remove(client.id);

        if (client.connection_dropped) {
            ServerChatMessage($"{client.username} left (client.connection_dropped = true)");
        } else {
            ServerChatMessage($"{client.username} left (disconnected)");
        }
    }
}
