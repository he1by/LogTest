using System;

namespace LogUsers
{
    using System.Threading;
    using LogComponent;
    using LogComponent.FileHelper;

    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger(new FileHelper(@"C:\LogTest", $"log{DateTime.Now.Ticks}"));

            for (int i = 0; i < 15; i++)
            {
                logger.WriteLog("Number with Flush: " + i.ToString());
                Console.WriteLine($"Thread 1 with Flush handle: {i}");
                Thread.Sleep(50);
            }

            logger.Stop(false);

            var logger2 = new Logger(new FileHelper(@"C:\LogTest", $"log{DateTime.Now.Ticks}"));

            for (int i = 50; i > 0; i--)
            {
                logger2.WriteLog("Number with No flush: " + i.ToString());
                Console.WriteLine($"Thread 2 with No flush handle: {i}");
                Thread.Sleep(20);
            }

            logger2.Stop(true);

            Console.ReadLine();
        }
    }
}
