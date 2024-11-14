using System.Collections.Generic;
using UnityEngine;

public class Replicator : MonoBehaviour {

    public struct ReplicatedClient {
        public ReplicatedVehiclePhysics player_model;
        public int id;
        public string username;
        public bool leaving;
    }

    public Client client;
    public ReplicatedVehiclePhysics replicated_tank_prefab;
    public Dictionary<int, ReplicatedClient> clients = new Dictionary<int, ReplicatedClient>();

    void Start() {
        client.RegisterPacketHandler(Packet.SERVER_ADD_PLAYER, HandleNewPlayerPacket);
        client.RegisterPacketHandler(Packet.SERVER_REMOVE_PLAYER, HandleRemovePlayerPacket);
        client.RegisterPacketHandler(Packet.SERVER_REPLICATE, HandleReplicationPacket);
        client.RegisterPacketHandler(Packet.SERVER_SET_USERNAME, HandleUsernameSet);
    }

    public ReplicatedClient GetReplicatedClient(int id) {
        return clients[id];
    }

    public void Reset() {
        for (int i = 0; i < clients.Count; i++) {
            ReplicatedClient tank = clients[i];
            Destroy(tank.player_model);
        }

        clients = new Dictionary<int, ReplicatedClient>();
    }
    
    void HandleUsernameSet() {
        int id = client.packet_handler.ReadInt();
        string username = client.packet_handler.ReadString();

        Debug.Log($"player {id} set username");

        ReplicatedClient replicated_client = GetReplicatedClient(id);
        replicated_client.username = username;
        replicated_client.player_model.banner.text = username;

        // structs are dumb
        clients[id] = replicated_client;
    }

    void HandleRemovePlayerPacket() {
        int id = client.packet_handler.ReadInt();
        bool dropped = client.packet_handler.ReadBool();

        ReplicatedClient tank = clients[id];
        Debug.Log($"player {id} leaving");
        Destroy(tank.player_model.gameObject);
        clients.Remove(id);
    }  

    void HandleNewPlayerPacket() {
        int id = client.packet_handler.ReadInt();
        Vector3 position = client.packet_handler.ReadVector3();
        
        ReplicatedClient replicated_client = new ReplicatedClient();
        replicated_client.player_model = Instantiate(replicated_tank_prefab, position, new Quaternion());
        replicated_client.id = id;

        clients[id] = replicated_client;
    }

    void HandleReplicationPacket() {
        int id = client.packet_handler.ReadInt();
        ReplicatedVehiclePhysics tank = clients[id].player_model;

        tank.tank.position = client.packet_handler.ReadVector3();
        tank.tank.rotation = Quaternion.Euler(client.packet_handler.ReadVector3());
        tank.tank.velocity = client.packet_handler.ReadVector3();
        tank.camera_rotation = client.packet_handler.ReadFloat();
        tank.camera_pitch = client.packet_handler.ReadFloat();
    }

}