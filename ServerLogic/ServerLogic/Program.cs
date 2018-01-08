using Login;
using Serwer2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Commands commands = new Commands();
                    string to_respond = "";
                    string input = "";
                    string IP = Console.ReadLine();
                    Connection connect = new Connection(IP, 3456);
                    while (true)
                    {

                        connect.Connect();
                        input = connect.Recive();
                        System.Console.WriteLine(input);
                        to_respond = commands.Do(input);
                        Console.WriteLine(to_respond);
                        connect.Respond(to_respond);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
