using UnityEngine;

public class Replicator : MonoBehaviour {

    public Server server;

    void Start() {
        server.RegisterPacketHandler(Packet.CLIENT_REPLICATE, HandleReplicationPacket);
    }

    void HandleReplicationPacket(Server.Client sender) {
        Vector3 position = sender.packet_handler.ReadVector3();
        Vector3 rotation = sender.packet_handler.ReadVector3();
        Vector3 velocity = sender.packet_handler.ReadVector3();
        float camera_rotation = sender.packet_handler.ReadFloat();
        float camera_pitch = sender.packet_handler.ReadFloat();

        server.clients.ForEach(client => {
            if (client.id != sender.id) {
                client.packet_handler.WriteHeader(Packet.SERVER_REPLICATE);
                client.packet_handler.WriteInt(sender.id);
                client.packet_handler.WriteVector3(position);
                client.packet_handler.WriteVector3(rotation);
                client.packet_handler.WriteVector3(velocity);
                client.packet_handler.WriteFloat(camera_rotation);
                client.packet_handler.WriteFloat(camera_pitch);
            } 
        });

    }

}