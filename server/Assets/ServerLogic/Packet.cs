using System.Text;
using System.Collections.Generic;
using System;
using UnityEngine;

// this is torture do this later
public class ServerPacket {
    public List<Server.Client> clients;
    public int len = 0;
    public byte[] buffer = new byte[0];

    public ServerPacket(List<Server.Client> clients) {
        this.clients = clients;
    }

    // public void Write(byte[] data) {
    //     if (buffer.Length > ) {

    //     }
    // }

    // public void WriteString(string data) {
    //     // strings are prepended with a length
    //     WriteU16((ushort)data.Length);

    //     byte[] buffer = new byte[(ushort)data.Length];
    //     buffer = Encoding.Default.GetBytes(data);
    //     Write(buffer);
    //     // stream.Write(buffer, 0, buffer.Length);
    // }

    // public string ReadString() {

    //     // read every char as a byte (ascii)
    //     ushort str_length = ReadU16();
    //     string result = "";
    //     for (int i = 0; i < str_length; i++) {
    //         char next_char = (char)stream.ReadByte();
    //         result += next_char;
    //     }

    //     return result;
    // }

    // // a unsigned short is 16 bits, so we read 2 bytes
    // public ushort ReadU16() {
    //     byte[] length = new byte[2];
    //     length[0] = (byte)stream.ReadByte();
    //     length[1] = (byte)stream.ReadByte();

    //     return BitConverter.ToUInt16(length);
    // }

    // public void WriteU16(ushort number) {
    //     byte[] buffer = BitConverter.GetBytes(number);
    //     stream.Write(buffer, 0, buffer.Length);
    // }

    // public void WriteInt(int number) {
    //     byte[] buffer = BitConverter.GetBytes(number);
    //     stream.Write(buffer, 0, buffer.Length);
    // }

    // public void WriteFloat(float number) {
    //     byte[] buffer = BitConverter.GetBytes(number);
    //     stream.Write(buffer, 0, buffer.Length);
    // }

    // // floats are always 4 bytes according to a google search
    // public float ReadFloat() {
    //     byte[] num = new byte[4];
    //     num[0] = (byte)stream.ReadByte();
    //     num[1] = (byte)stream.ReadByte();
    //     num[2] = (byte)stream.ReadByte();
    //     num[3] = (byte)stream.ReadByte();

    //     return BitConverter.ToSingle(num);
    // }

    // public int ReadInt() {
    //     byte[] num = new byte[4];
    //     num[0] = (byte)stream.ReadByte();
    //     num[1] = (byte)stream.ReadByte();
    //     num[2] = (byte)stream.ReadByte();
    //     num[3] = (byte)stream.ReadByte();

    //     return BitConverter.ToInt16(num);
    // }

    // public void WriteVector3(Vector3 vector) {
    //     WriteFloat(vector.x);
    //     WriteFloat(vector.y);
    //     WriteFloat(vector.z);
    // }

    // public Vector3 ReadVector3() {
    //     return new Vector3(ReadFloat(),ReadFloat(),ReadFloat());
    // }

    // public void WriteHeader(byte header) { 
    //     stream.WriteByte(header);
    // }
}