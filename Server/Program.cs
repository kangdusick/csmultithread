using ServerCore;
using System.Net;

namespace Server
{
    class Program
    {
        static Listener _listener;
        static void Main()
        {
            Console.WriteLine("Server.cs입니다.");
            //DNS (Domain name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            //문지기
            _listener = new Listener();
            _listener.Init(endPoint, () => { return new ClientSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
            }
        }
    }
}