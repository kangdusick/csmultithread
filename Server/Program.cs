namespace Server
{
    class Program
    {
        static void MainThread()
        {
            while (true)
                Console.WriteLine("1111");
        }
        static void Main()
        {
            Thread t = new Thread(MainThread);
            t.Start();
            Console.WriteLine("2222");
            t.Join();
            Console.WriteLine("asdf");
            Console.WriteLine("Asdf");
        }
    }
}