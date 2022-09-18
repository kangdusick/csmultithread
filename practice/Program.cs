using System;

    delegate int intOp(int a, int b);
class Practice
{
    public static int Add(int a, int b)
    {
        return a + b;
    }
    public static int Mul(int a, int b)
    {
        return a* b;
    }
    static void Main()
    {
        intOp[] arOp = { Add, Mul };
        int a = 3, b = 5;
        int o;
        Console.WriteLine("1.덧셈 2.곱셈");
        o = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine(arOp[o - 1](a,b));
    }
}