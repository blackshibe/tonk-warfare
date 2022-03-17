using UnityEngine;

public class NetworkManager : MonoBehaviour {
    private static NetworkManager _singleton;
    public static NetworkManager Singleton {
        get => _singleton;
        private set {
            if (_singleton == null) {
                _singleton = value;
	    }
            else if (_singleton != value) {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    ushort port;
    ushort max_players;

    private void Awake() {
        Singleton = this;
    }
}
