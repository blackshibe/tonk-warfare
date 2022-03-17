using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour {

    public TMP_InputField address;
    public TMP_InputField username;
    public Client client;
    public VehiclePhysics local_tank_prefab;

    void Start() {
    }

    public void Connect() {
       Debug.Log($"connecting as {username.text} to {address.text}:{Definitions.LISTENER_PORT}");
       
       client.ip = address.text;
       client.local_username = username.text;
       client.Connect();

       GetComponent<Canvas>().enabled = false;
       // hopefully connected lol
       Instantiate(local_tank_prefab, new Vector3(-12, 1, 7), new Quaternion());
    }
}