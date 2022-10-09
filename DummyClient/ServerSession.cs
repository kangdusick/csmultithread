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
                playerID = 1001
            };

            //보낸다
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);

                ushort count = 0;
                bool success = true;

                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerID);
                count += 8;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

                if (success)
                {
                    Send(sendBuff);
                }
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
