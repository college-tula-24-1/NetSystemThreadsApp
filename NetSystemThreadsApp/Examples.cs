using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSystemThreadsApp
{
    static public class Examples
    {
        static int count = 0;

        static object locker = new();
        static Mutex mutex = new();

        public static void ThreadWelcomeExample()
        {
            Thread mainThread = Thread.CurrentThread;

            //Console.WriteLine(mainThread.Name);
            //mainThread.Name = "Main Thread";
            //Console.WriteLine(mainThread.Name);

            //Console.WriteLine(mainThread.IsAlive);
            //Console.WriteLine(mainThread.Priority);
            //Console.WriteLine(mainThread.ManagedThreadId);
            //Console.WriteLine(mainThread.ThreadState);
            //Console.WriteLine(Thread.GetDomain());

            Thread threadOne = new Thread(new ParameterizedThreadStart(PrintString));
            Thread threadTwo = new Thread(PrintString);
            Thread threadThree = new Thread((object? str) =>
            {
                if (str is StrCount strCount)
                    for (int i = 0; i < strCount.count; i++)
                        Console.WriteLine($"{strCount.str}: {i}");
            });

            threadOne.Start(new StrCount("First", 20));
            threadTwo.Start(new StrCount("Second", 30));
            threadThree.Start(new StrCount("Third", 10));

            PrintString(new StrCount("Main", 20));
        }

        public static void ThreadLockExample()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread thread = new(CountIncMutex);
                thread.Start();
                thread.Name = i.ToString();
            }
        }

        public static void ThreadSemaphoreExample()
        {
            for (int i = 0; i < 10; i++)
            {
                Car car = new(i + 1);
            }
        }


        static void ActionCount(object? count)
        {
            if (count is int countSize)
                for (int i = 0; i < countSize; i++)
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {i}");
        }

        static void PrintString(object str)
        {
            if (str is StrCount strCount)
                for (int i = 0; i < strCount.count; i++)
                    Console.WriteLine($"{strCount.str}: {i}");
        }

        static void CountInc()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (locker)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}: {count}");
                    count++;
                    Thread.Sleep(100);
                }
            }

        }

        static void CountIncMonitor()
        {
            bool flag = false;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    flag = false;
                    Monitor.Enter(locker, ref flag);
                    Console.WriteLine($"{Thread.CurrentThread.Name}: {count}");
                    count++;
                    Thread.Sleep(100);
                }
                finally
                {
                    if (flag) Monitor.Exit(locker);
                }
            }

        }

        static void CountIncMutex()
        {
            for (int i = 0; i < 10; i++)
            {
                mutex.WaitOne();
                Console.WriteLine($"{Thread.CurrentThread.Name}: {count}");
                count++;
                Thread.Sleep(100);
                mutex.ReleaseMutex();
            }

        }
    }

    record class StrCount(string str, int count);

    class Car
    {
        static Semaphore semaphore = new(4, 4);
        Thread carThread;
        int count = 5;

        static Random random = new();

        public Car(int id)
        {
            carThread = new(Parking);
            carThread.Name = $"Car #{id}";
            carThread.Start();
        }

        public void Parking()
        {
            while (count > 0)
            {
                semaphore.WaitOne();

                Console.WriteLine($"{Thread.CurrentThread.Name} IN parking");

                Thread.Sleep(random.Next(1000, 3000));

                Console.WriteLine($"{Thread.CurrentThread.Name} OUT parking ({count - 1})");

                semaphore.Release();

                count--;

                Thread.Sleep(random.Next(2000, 4000));
            }
        }
    }
}
