class Definitions {
    public const int LISTENER_PORT = 20205;
    public const int SERVER_CLIENT_CAPACITY = 128;
} 

// SERVER_ means server -> client
public struct Packet {
	// loopback
	static public byte _PLAYER_ADDED = 1;
	static public byte _PLAYER_REMOVED = 2;

	// internal
	static public byte SERVER_INIT_HANDSHAKE = 3;
	static public byte SERVER_PING = 4;
	static public byte CLIENT_EXIT = 5;
	static public byte CLIENT_HANDSHAKE = 6;

	// custom
	static public byte CLIENT_REPLICATE = 7;
	static public byte SERVER_REMOVE_PLAYER = 8;
    static public byte SERVER_ADD_PLAYER = 9;
	static public byte SERVER_REPLICATE = 10;
	static public byte SERVER_SET_USERNAME = 11;
	static public byte SERVER_CHAT_MESSAGE = 12;
	static public byte CLIENT_CHAT_MESSAGE = 13;
}