using System;
using System.Threading;
using DotNettyLib.Application;

namespace DotNettyLib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // return;
            ThreadPool.QueueUserWorkItem((s) =>
            {
                StartServer();
            });
            
            ThreadPool.QueueUserWorkItem((s) =>
            {
                StartClient();
            });
            
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.X)
                    break;
            }
        }

        public static void StartServer()
        {
            IApplication applicationServer = new ServerApplication();
            
            applicationServer.Start();


            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.X)
                    break;
            }
            
            applicationServer.End();
        }
        
 
        public static void StartClient()
        {
            IApplication applicationServer = new ClientApplication();
            
            applicationServer.Start();

            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.X)
                    break;
            }
            
            applicationServer.End();
        }       
    }
}