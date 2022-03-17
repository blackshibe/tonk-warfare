using System.Text;
using System.Net.Sockets;
using System;
using UnityEngine;

class StreamWrite {
    public NetworkStream stream;

    public StreamWrite(NetworkStream stream) {
        this.stream = stream;
    }

    public void WriteString(string data) {
        // strings are prepended with a length
        WriteU16((ushort)data.Length);

        byte[] buffer = new byte[(ushort)data.Length];
        buffer = Encoding.Default.GetBytes(data);
        stream.Write(buffer, 0, buffer.Length);
    }

    public string ReadString() {

        // read every char as a byte (ascii)
        ushort str_length = ReadU16();
        string result = "";
        for (int i = 0; i < str_length; i++) {
            char next_char = (char)stream.ReadByte();
            result += next_char;
        }

        return result;
    }

    // a unsigned short is 16 bits, so we read 2 bytes
    public ushort ReadU16() {
        byte[] length = new byte[2];
        length[0] = (byte)stream.ReadByte();
        length[1] = (byte)stream.ReadByte();

        return BitConverter.ToUInt16(length);
    }

    public void WriteU16(ushort number) {
        byte[] buffer = BitConverter.GetBytes(number);
        stream.Write(buffer, 0, buffer.Length);
    }

    public void WriteHeader(byte header) { 
        stream.WriteByte(header);
    }
}