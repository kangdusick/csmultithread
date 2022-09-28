using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine(numOfBytes);
        }
    }

    class Program
    {
        static Listener _listener;
        static void Main()
        {
            //DNS (Domain name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            //문지기
            _listener = new Listener();
            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
            }
        }
    }
}