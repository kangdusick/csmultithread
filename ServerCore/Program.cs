using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
namespace ServerCore
{

    class Program
    {
        static Listener _listener;
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session(clientSocket);

                //보낸다
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                session.Send(sendBuff);

                Thread.Sleep(1000);

                session.Disconnect();
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        static void Main()
        {
            //DNS (Domain name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            //문지기
            _listener = new Listener(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            while (true)
            {
            }
        }
    }
}