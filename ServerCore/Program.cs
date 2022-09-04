using System;
using System.Threading;
namespace ServerCore
{
    class Program
    {
        static int x;
        static int y;
        static int r1;
        static int r2;
        static void Thread1()
        {
            y = 1;
            Thread.MemoryBarrier();
            r1 = x;
        }
        static void Thread2()
        {
            x = 1;
            Thread.MemoryBarrier();
            r2 = y;
        }
        static void Main()
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = r1 = r2 = 0;
                var t1 = new Task(Thread1);
                var t2 = new Task(Thread2);
                t1.Start();
                t2.Start();
                Task.WaitAll(t1, t2);
                if(r1==0&&r2==0)
                {
                    break;
                }
            }
           
            Console.WriteLine(count.ToString());
        }
    }
}