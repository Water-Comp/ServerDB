using Login;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ServerLogic
{
    class Logic
    {
        /*
            Create() - Create new table, arguments: table name and array of names of columns
            SendUpdate() - Send new data to server, arguments: table name and array of data
            ReciveUpdate() - Send new data to client, arguments: table name and lastest value of time
            GetLastImage() - Send lastest image of mission: arguments: table name(+_img)
            CheckTopicality() - Check actuall, arguments: table name and lastest values of time
            GetLastest() - Send lastest set of data to client, arguments: table name
            GetColumns() - Send set of columns in table, arguments: table name
            List() - Send list of missions
        */

        DB dB = null;
        public static List<string> Commands = new List<string>();

        //global variable using to send answer to client
        public string answer = "";

        //global variables using in method Devide(), GetTime() or GetIMG()
        private string mission_name = "";
        private List<string> arguments = null;
        private uint time;
        private string parameter;

        public Logic()
        {
            //TODO Dodać Check Sume 
            //TODO SendOne
            //TODO MemoryCheck
            //TODO Flagowanie
            //TODO Baza
        }

        /*TODO Jak najszybciej
         * CheckSume
         * Restrukturyzacja bazy danych (table -> bazy)
         * SendOne
         * MemoryCheck
         * Flagowanie
         */

        public string ToAnswer(string ans)
        {
            answer = "";
            return ans;      
        }

        private bool Exist(string table)
        {
            string sql = "SELECT * FROM TABLES WHERE Table_Name = '" + mission_name + "'";
            if (dB.Query(sql) == "") return false;
            else return true;
        }

        private void Divide_(List<string> args)
        {
            mission_name = "";
            arguments = new List<string>();

            for (int i = 0; i < args.Count; i++)
            {
                if (i == 0) mission_name = args[i];
                else arguments.Add(args[i]);
            }
        }

        //Cut table name off rest of arguments
        private void Divide(List<string> args)
        {
            mission_name = "";
            arguments = new List<string>();

            for (int i = 0; i < args.Count; i++)
            {
                if (i == 0) mission_name = args[i];
                else if (i == 1) parameter = args[i];
                else arguments.Add(args[i]);
            }
        }

        //Cut table name off time (using in ReciveUpdate)
        private void GetTime(List<string> args)
        {
            int time_0;
            mission_name = args[0];
            parameter = args[1];
            Int32.TryParse(args[2], out time_0);
            time = Convert.ToUInt32(time_0);
        }

        //Create new table, arguments: table name and array of names of columns
        public void Create(List<string> args)
        {
            try
            {
                Divide_(args);
                ConnectWithMission(mission_name);
                //Create new database in folder bin\Debug\Missions
                string path = @"Missions\" + mission_name + ".db";
                DB new_mission = new DB(path);

                //Create structure of data in this database
                /*Create table of Images*/
                string sql_img = "CREATE TABLE Images (Time int, Image text)";
                new_mission.Query(sql_img);

                /*Create and fill table of parameters*/
                string sql_struc = "CREATE TABLE Structure (Structure text)";
                new_mission.Query(sql_struc);
                string sql_struc_fill = "INSERT INTO Structure VALUES ('Images ";
                for (int i = 0; i<arguments.Count;i++)
                {
                    sql_struc_fill += arguments[i];
                    if (i + 1 != arguments.Count) sql_struc_fill += " ";
                }
                sql_struc_fill += "')";
                new_mission.Query(sql_struc_fill);

                /*Create tables of parameters*/
                for (int i = 0; i < arguments.Count; i++)
                {
                    string sql = "CREATE TABLE " + arguments[i] + " (Time int, Value real)";
                    new_mission.Query(sql);
                }
                //Make an answer
                answer = Answers.Succesful;
            }
            catch(Exception e)
            {
                answer = e.Message;
            }

        }

        //Send new data to server, arguments: table name and array of data
        public void SendUpdate(List<string> args)
        {
            try
            {
                Divide(args);
                ConnectWithMission(mission_name);
                for (int i = 0;i<arguments.Count;i++)
                {
                    if (i % 2 == 1)
                    {
                        string sql;
                        float value;
                        string value_img;
                        int time_ = Int32.Parse(arguments[i - 1]);
                        if (parameter != "Images")
                        {
                            value = float.Parse(arguments[i], CultureInfo.InvariantCulture.NumberFormat);
                            sql = "INSERT INTO " + parameter + " VALUES (" + time_ + ", " + value + ")";
                        }
                        else
                        {
                            value_img = arguments[i];
                            sql = "INSERT INTO " + parameter + " VALUES (" + time_ + ", '" + value_img + "')";
                        }
                        dB.Query(sql);
                    }
                }
                answer = Answers.Succesful;
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        //Send new data to client, arguments: table name and lastest value of time
        public void ReciveUpdate(List<string> args)
        {
            try
            {
                GetTime(args);
                ConnectWithMission(mission_name);
                string sql = "SELECT * FROM " + parameter + " WHERE time > " + time;
                answer = dB.Query(sql);
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        //Send lastest image of mission: arguments: table name
        public void GetLastImage(List<string> args)
        {
            try
            {
                ConnectWithMission(mission_name);
                string sql_t = "SELECT MAX(Time) FROM Images";
                string sql = "SELECT Image FROM Images WHERE Time = " + dB.Query(sql_t);
                answer = dB.Query(sql);
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        //Check actuall, arguments: table name and lastest values of time
        public void CheckTopicality(List<string> args)
        {
            try
            {
                GetTime(args);
                ConnectWithMission(mission_name);
                string sql = "SELECT * FROM " + parameter + " WHERE time > " + time;
                if (dB.Query(sql) == "") answer = "Yes";
                else answer = "No";
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        //Send lastest set of data to client, arguments: table name
        public void GetLastest(List<string> args)
        {
            try
            {
                Divide(args);
                ConnectWithMission(mission_name);
                string sql_time = "SELECT MAX(time) FROM " + parameter;
                int max_time = Int32.Parse(dB.Query(sql_time));
                string sql = "SELECT * FROM " + parameter + " WHERE time = " + max_time;
                answer = dB.Query(sql);
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        //Send set of columns in table, arguments: table name
        public void GetColumns(List<string> args)
        {
            try
            {
                Divide(args);
                ConnectWithMission(mission_name);
                answer = dB.Query("SELECT Structure FROM Structure");
            }
            catch (NullReferenceException e)
            {
                answer = e.Message;
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        //Send list of missions
        public void List()
        {
            try
            {
                List<string> something = new List<string>();
                List<string> fileT = new List<string>();
                string path = "Missions";
                string[] files = Directory.GetFiles(path, "*.db");
                for (int i = 0; i < files.Length; i++) fileT.Add(files[i]);

                string[] file = fileT.ToArray();
                for (int i = 0; i < file.Length; i++)
                {
                    string tmp = "";
                    List<char> one_file = new List<char>(file[i].ToArray());
                    for (int j = 0; j < 3; j++) one_file.RemoveAt(one_file.Count - 1);
                    one_file.RemoveRange(0, path.ToArray().Length + 1);
                    for (int j = 0; j < one_file.Count; j++)
                    {
                        tmp += one_file[j];
                    }
                    something.Add(tmp);
                }

                string to_ans = "";

                for (int i = 0; i < something.Count; i++)
                {
                    to_ans += something[i];
                    if (i + 1 != something.Count) to_ans += " ";
                }
                answer = to_ans;
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }

        public void ConnectWithMission(string mission)
        {
            try
            {
                string path = @"Missions\" + mission + ".db";
                dB = new DB(path);
            }
            catch (Exception e)
            {
                answer = e.Message;
            }
        }
    }
}
