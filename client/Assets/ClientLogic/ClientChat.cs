using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// todo: delete old tmp_text samples
public class ClientChat : MonoBehaviour {
    
    public Replicator replicator;
    public Client client;
    public Canvas chat_canvas;
    public TMP_Text sample_text;
    public VerticalLayoutGroup chat_content;
    public TMP_InputField textbox;

    public List<TMP_Text> samples = new List<TMP_Text>();
    int max_messages = 15;

    void Start() {
        client.RegisterPacketHandler(Packet.SERVER_CHAT_MESSAGE, HandlePlayerMessage);
    }

    public void HandlePlayerMessage() {
        bool is_server = client.packet_handler.ReadBool();

        if (is_server) {
            string message = client.packet_handler.ReadString();
            AddSample(Color.white, "server", message);
        } else {
            int sender_id = client.packet_handler.ReadInt();
            Color color = client.packet_handler.ReadColor();
            string message = client.packet_handler.ReadString();

            if (sender_id == client.local_id) AddSample(color, client.local_username, message);
            else AddSample(color, replicator.GetReplicatedClient(sender_id).username, message);
        }
    }

    public void SendText() {
        if (!chat_canvas.enabled) return;

        client.packet_handler.WriteHeader(Packet.CLIENT_CHAT_MESSAGE);

        string text = textbox.text;
        client.packet_handler.WriteString(text);

        textbox.text = "";
        textbox.DeactivateInputField();
    }

    public void Toggle(bool enabled) {
        chat_canvas.enabled = enabled;
    }

    public void AddSample(Color color, string sender, string text) {
        if (max_messages < samples.Count) {
            Destroy(samples[0]);
            samples.RemoveAt(0);
        }

        TMP_Text sample = Instantiate(sample_text, chat_content.transform);
        sample.color = color;
        sample.text = $"[{sender}]: {text}";

        // forces a redraw or smth idk without this the vertical layout stacks elements on top of each other
        chat_content.spacing = 0.1f;
        samples.Add(sample);
    }

    void FixedUpdate() {
        if (!chat_canvas.enabled) return;

        chat_content.spacing = 0;

        bool chat = Input.GetKeyDown(KeyCode.Slash);
        if (chat) {
            textbox.ActivateInputField();
        }
    }
}
