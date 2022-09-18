using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
namespace ServerCore
{
   
    class Program
    {
        static ThreadLocal<string> ThreadName = new();
        static void WhoAmI()
        {

        }
        static void Main()
        {
            //DNS (Domain name System)
            string host = Dns.GetHostName();
            Socket listenSocket = new Socket();
        }
    }
}