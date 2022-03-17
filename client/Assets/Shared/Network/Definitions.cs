class Definitions {
    public const int LISTENER_PORT = 20205;
    public const int SERVER_CLIENT_CAPACITY = 128;
} 

// SERVER_ means server -> client
struct Packet {
	static public byte SERVER_INIT_HANDSHAKE = 1;
	static public byte CLIENT_HANDSHAKE = 1;
}