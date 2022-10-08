using ServerCore;
using System.Net;

namespace DummyClient
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("더미 클라이언트 입니다.");
            //DNS (Domain name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            var connector = new Connector();

            connector.Connect(endPoint, () => { return new ServerSession(); });
            while (true)
            {
                try
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(100);
            }
        }
    }
}