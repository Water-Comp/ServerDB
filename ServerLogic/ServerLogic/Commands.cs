using Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLogic
{
    class Commands
    {
        Logic logic = new Logic();
        public Commands()
        {
            ;
        }
        public string Do(string main)
        {
            string[] tmp = main.Split(' ');

            //Cut command off arguments
            string command = null;
            List<string> args = new List<string>();
            for (int i = 0; i < tmp.Length; i++)
            {
                if (i == 0) command = tmp[i];
                else args.Add(tmp[i]);
            }

            //Make List of commands
            List<string> Command = new List<string>();
            Command.Add("Create");
            Command.Add("SendUpdate");
            Command.Add("ReciveUpdate");
            Command.Add("GetLastImage");
            Command.Add("CheckTopicality");
            Command.Add("GetLastest");
            Command.Add("GetColumns");
            Command.Add("List");
            Command.Add("CheckMemory");

            int num = Command.IndexOf(command);
            //In case of first word of command try to do or return error
            switch (num)
            {
                case 0:
                    logic.Create(args);
                    return Answer();

                case 1:
                    logic.SendUpdate(args);
                    return Answer();

                case 2:
                    logic.ReciveUpdate(args);
                    return Answer();

                case 3:
                    logic.GetLastImage(args);
                    return Answer();

                case 4:
                    logic.CheckTopicality(args);
                    return Answer();

                case 5:
                    logic.GetLastest(args);
                    return Answer();

                case 6:
                    logic.GetColumns(args);
                    return Answer();

                case 7:
                    logic.List();
                    return Answer();

                case 8:
                    logic.CheckMemory();
                    return Answer();

                default:
                    return Answers.Invalid_Command;
            }



        }

        private string Answer()
        {
            return logic.ToAnswer(logic.answer);
        }
    }
}
