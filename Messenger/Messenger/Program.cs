using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger
{
    class Program
    {
        static void Main(string[] args)
        {
            string IP = Console.ReadLine();
            while(true) Console.WriteLine(TCP.Connect(IP, Console.ReadLine(), 3456));
        }
    }
}
