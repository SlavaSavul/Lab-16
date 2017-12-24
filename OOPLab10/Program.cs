using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;

using System.Reflection;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;




namespace OOPLab10
{

    class Program
    {
        static void Main(string[] args)
        {

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            Task task1 = new Task(SearchPrimes.EratosthenSieve);
            Task task2 = new Task(SearchPrimes.EratosthenSieve, tokenSource.Token);


            task1.Start();
            Console.WriteLine("ID: " + task1.Id);
            if (task1.IsCompleted) Console.WriteLine("Задача завершена");
            else Console.WriteLine("Задача не завершена");

            stopwatch1.Stop();
            Task.WaitAll(task1); Console.WriteLine(stopwatch1.ElapsedMilliseconds + " милисекунд ");
            task2.Start();
            tokenSource.Cancel();
            Thread.Sleep(2000);
            if (task2.IsCanceled) Console.WriteLine("Задача отменена");


            Task<int> Operand1 = new Task<int>(SearchPrimes.FirstOperand);
            Task<int> Operand2 = new Task<int>(SearchPrimes.SecondOperand);
            Task<int> Operand3 = new Task<int>(SearchPrimes.ThirdOperand);
            Operand1.Start();
            Operand2.Start();
            Operand3.Start();
            int op1 = Operand1.Result;
            int op2 = Operand2.Result;
            int op3 = Operand3.Result;
            Task.WaitAll(Operand1, Operand2, Operand3);
            Task Result = new Task(() => Console.WriteLine(Math.Round(Math.Sin(op2) * op3 - op1, 3)));
            Result.Start();


            Task.WaitAny(Result);
            Task one = Task.Run(() => Console.Write("One...."));
            Task two = Task.Run(() => Console.Write("Two..."));
            Task three = Task.WhenAll(one, two).ContinueWith(t => Console.Write("Three...."));


            Task<int> First = Task.Run(() => Enumerable.Range(1, 10000).Count(r => (r % 2 == 0 && r % 3 == 0 && r > 6000)));
            TaskAwaiter<int> awaiter = First.GetAwaiter();
            Task Continue = new Task(() => { Console.WriteLine(awaiter.GetResult()); });

            awaiter.OnCompleted(() => { Continue.Start(); });


            Task.WaitAny(Continue);
           
            Parallel.For(1, 10, SearchPrimes.Factorial);
            Console.WriteLine("EndFor");
            ParallelLoopResult result = Parallel.ForEach<int>(new List<int>() { 1, 3, 5, 8 }, SearchPrimes.Factorial);
            Console.WriteLine("EndFrEach");
            Parallel.Invoke(() => { Console.WriteLine("One"); },
                           () => { Console.WriteLine("Two"); },
                           () => { Console.WriteLine("Free"); });
            //////////////////////////////////////////////////


            Task Pr = new Task(SearchPrimes.provider);
            Task Cn = new Task(SearchPrimes.consumer);
            Pr.Start();
            Cn.Start();

            Console.WriteLine();
            Console.WriteLine();
            SearchPrimes.DisplayResultAsync();

            Console.ReadKey();
        }

        static class SearchPrimes 
        {
            static int N = 10001;
            static IEnumerable<int> Range = Enumerable.Range(0, N).Select(x => x);
            static int[] Massiv = new int[N];
            static int Count = 0;
            static BlockingCollection<int> blockcoll = new BlockingCollection<int>(5);

            static public void provider()
            {
                for (int j = 0; j < 5; j++)
                {
                    blockcoll.Add(j );
                    Console.WriteLine("Производится число " + j );
                    Thread.Sleep(30*j);
                }
                blockcoll.CompleteAdding();
            }
            static public void consumer()
            {
                int i;
                for (int j = 0; j < 10; j++)
                {
                    if (blockcoll.TryTake(out i))
                    {
                        Thread.Sleep(20);
                        Console.WriteLine("Потребляется число: " + i);
                    }
                    else Console.WriteLine("Покупатель уходит");
                }
            }
            static public void EratosthenSieve()
            {
               foreach (int i in Range)
                {
                    Massiv[Count++] = i;
                }
                Massiv[2] = 0;
                int mark = 2;
                int j = 0;
                while (mark < N)
                {
                    for (int i = 2; i < N; i++)
                    {
                        j = i * mark;
                        if(j<N)
                        Massiv[j] = 0;
                    }
                    mark++;
                }
                 foreach (int i in Massiv)
                    {
                        if (i != 0) Console.Write(i + "  ");
                    }
                
                Console.WriteLine();
                
            }
            static public int FirstOperand()
            {
                return 2;
            }
            static public int SecondOperand()
            {
                return 70;
            }
            static public int ThirdOperand()
            {
                return 10;
            }
            static public void Factorial(int x)
            {
                int result = 1;

                for (int i = 1; i <= x; i++)
                {
                    result *= i;
                }
                Console.WriteLine("Выполняется задача {0}", Task.CurrentId);
                Console.WriteLine("Факториал числа {0} равен {1}", x, result);
            }
            static public async void DisplayResultAsync()
            {
                int num = 5;

                int result = await FactorialAsync(num);
                Thread.Sleep(3000);
                Console.WriteLine("Факториал числа {0} равен {1}", num, result);
            }
            static public Task<int> FactorialAsync(int x)
            {
                int result = 1;

                return Task.Run(() => {
                    for (int i = 1; i <= x; i++)
                    {
                        result *= i;
                    }
                    return result;    });
            }
        }












    }
}
