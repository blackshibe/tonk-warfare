using System.Text;
using System.Net.Sockets;
using System;
using UnityEngine;

// wrapper for NetworkStream to make writing data easier

public class PacketHandler {
    public NetworkStream stream;

    public PacketHandler(NetworkStream stream) {
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

    public void WriteInt(int number) {
        byte[] buffer = BitConverter.GetBytes(number);
        stream.Write(buffer, 0, buffer.Length);
    }

    public void WriteFloat(float number) {
        byte[] buffer = BitConverter.GetBytes(number);
        stream.Write(buffer, 0, buffer.Length);
    }

    // floats are always 4 bytes according to a google search
    public float ReadFloat() {
        byte[] num = new byte[4];
        num[0] = (byte)stream.ReadByte();
        num[1] = (byte)stream.ReadByte();
        num[2] = (byte)stream.ReadByte();
        num[3] = (byte)stream.ReadByte();

        return BitConverter.ToSingle(num);
    }

    public int ReadInt() {
        byte[] num = new byte[4];
        num[0] = (byte)stream.ReadByte();
        num[1] = (byte)stream.ReadByte();
        num[2] = (byte)stream.ReadByte();
        num[3] = (byte)stream.ReadByte();

        return BitConverter.ToInt16(num);
    }

    public void WriteVector3(Vector3 vector) {
        WriteFloat(vector.x);
        WriteFloat(vector.y);
        WriteFloat(vector.z);
    }
    
    public void WriteColor(Color color) {
        WriteFloat(color.r);
        WriteFloat(color.g);
        WriteFloat(color.b);
    }

    public void WriteBool(bool _bool) {
        byte[] data = BitConverter.GetBytes(_bool);
        stream.Write(data, 0, data.Length);
    }

    public bool ReadBool() {
        byte[] data = new byte[1];
        data[0] = (byte)stream.ReadByte();
        return BitConverter.ToBoolean(data);
    }

    public Vector3 ReadVector3() {
        return new Vector3(ReadFloat(),ReadFloat(),ReadFloat());
    }

    public Color ReadColor() {
        return new Color(ReadFloat(),ReadFloat(),ReadFloat());
    }

    public void WriteHeader(byte header) { 
        stream.WriteByte(header);
    }
}