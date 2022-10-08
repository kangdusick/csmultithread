using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }
    class PlayerInfoReq : Packet
    {
        public long playerID;
    }
    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }
    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq()
            {
                size = 4,
                packetId = (ushort)PacketID.PlayerInfoReq,
                playerID = 1
            };

            //보낸다
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                byte[] size = BitConverter.GetBytes(packet.size);
                byte[] packetID = BitConverter.GetBytes(packet.packetId);
                byte[] playerID = BitConverter.GetBytes(packet.playerID);

                Array.Copy(size, 0, openSegment.Array, openSegment.Offset, size.Length);
                Array.Copy(packetID, 0, openSegment.Array, openSegment.Offset + size.Length, packetID.Length);
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);

                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine(numOfBytes);
        }
    }
}
