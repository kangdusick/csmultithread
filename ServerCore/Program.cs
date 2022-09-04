using System;
using System.Threading;
namespace ServerCore
{
    class Program
    {
        static void MainThread(object state)
        {
            Console.WriteLine("1111");
        }
        static void Main()
        {
            ThreadPool.QueueUserWorkItem(MainThread);
            /*
            Thread t = new Thread(MainThread);
            t.Name = "테스트";
            t.Start();
            Console.WriteLine("2222");
            t.Join();
            */
            Console.WriteLine("asdf");
        }
    }
}