using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumbers
{
    class Program
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> Rng =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        static object Lock = new object();
        static int Coprimes = 0;

        static void Main(string[] args)
        {
            long count = -1;

            Console.WriteLine("Calculating PI with random numbers!");
            if (args.Length == 0 || !long.TryParse(args[0], out count))
            {
                Console.Write("How many random numbers should be used? ");
                while (!long.TryParse(Console.ReadLine(), out count) && count <= 0) { }
            }
            var timer = Stopwatch.StartNew();

            Parallel.For(0, count, (i) => { GenerateCoprime(); });
            timer.Stop();

            Console.WriteLine();
            Console.WriteLine($"Count: {count}\tCofactors: {count - Coprimes}\tCoprimes: {Coprimes}\tTotal Time: {timer.ElapsedMilliseconds}ms");
            var pi = Math.Sqrt(6 / (Coprimes / (double)count));
            Console.WriteLine($"PI ~= {pi}");
        }

        static bool TryGenerateCoprime() => gcd(Rng.Value.Next(), Rng.Value.Next()) == 1;

        static void GenerateCoprime()
        {
            if (TryGenerateCoprime())
                Interlocked.Increment(ref Coprimes);
        }

        static int gcd(int x, int y)
        {
            if (y == 0)
                return x;
            else
                return gcd(y, x % y);
        }
    }
}