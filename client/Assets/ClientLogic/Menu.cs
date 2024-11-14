using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour {

	public TMP_InputField address;
	public TMP_InputField username;
	public TMP_Text status;

	public Client client;
	public Replicator replicator;
	public VehiclePhysics local_tank_prefab;
	public ClientChat chat;

	void Start() {
		client.RegisterPacketHandler(Packet.SERVER_INIT_HANDSHAKE, Connected);
		client.RegisterPacketHandler(Packet.SERVER_PING, Ping);

		Entered();

		Screen.SetResolution(1280, 720, FullScreenMode.Windowed, 60);
	}

	public void Entered() {
		if (ClientDisconnectReason.data.use_reason) status.text = ClientDisconnectReason.data.reason;
		else status.text = "game version: idk";
		client.connection_status = true;
		client.ingame = false;
	}

	public void Connected() {
		Debug.Log("Game starting");
		
		Vector3 position = client.packet_handler.ReadVector3();
		client.local_vehicle = Instantiate(local_tank_prefab, position, new Quaternion());
		client.local_vehicle.chat = chat;
		client.local_vehicle.client = client;
		
		GetComponent<Canvas>().enabled = false;
		chat.Toggle(true);
		client.ingame = true;
	}

	// todo: track whether the client actually connected
	public void Connect() {
		Debug.Log($"connecting as {username.text} to {address.text}:{Definitions.LISTENER_PORT}");
		status.text = $"connecting: \"{username.text}\" to {address.text}:{Definitions.LISTENER_PORT}";
		client.ip = address.text;
		client.local_username = username.text;
		client.Connect();

		Debug.Log($"result: {client.connection_result}");
		if (client.connection_status) status.text = client.connection_result; 
		else status.text = $"couldnt connect, heres the error: {client.connection_result}";
	}

	public void LocalPlay() {
		GetComponent<Canvas>().enabled = false;
		VehiclePhysics tank = Instantiate(local_tank_prefab, new Vector3(-12, 1, 7), new Quaternion());
		tank.chat = chat;
		tank.client = client;
	}

	public void Update() {
		if (!client.connected && client.ingame) {
			Debug.Log("resetting the game");
			
			chat.Toggle(false);
			Cursor.lockState = CursorLockMode.None;
			Destroy(client.local_vehicle);
			GetComponent<Canvas>().enabled = true;
			replicator.Reset();
			SceneManager.LoadScene(0);
		}
	}

	public void Ping() {}
}