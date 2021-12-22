using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace DotNettyLib.log
{
    public class Log
    {
        public static void Info(string str) => Print("Info", str);
        
        
        public static void Warning(string str) => Print("Waring", str);
        
        public static void Error(string str) => Print("Error", str);
        
        public static void Debug(string str) => Print("Debug", str);

        private static void Print(string type, string str)
        {
            Console.WriteLine("[" + type + "]:" + str);
            // Category type
        }
    }
}