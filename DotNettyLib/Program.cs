using System;
using System.Threading;
using DotNetty.Transport.Bootstrapping;
using DotNettyLib.Application;
using NUnit.Framework;

namespace DotNettyLib
{
    class Program
    {
        struct MyStruct
        {
            public float X, Y;

            public override string ToString()
            {
                return X + ", " + Y;
            }
        }
        
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