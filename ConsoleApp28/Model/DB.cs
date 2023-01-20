using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ConsoleApp28
{
     
    public class DB
    {     
        //Check userID in DB
        public bool CheckUserID(long userID, string userName)
        {
            try
            {
                using (System.Data.SQLite.SQLiteConnection conn =
                new System.Data.SQLite.SQLiteConnection($"data source={Properties.Resources.dataSource}"))
                {
                    using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                    {
                        conn.Open();
                        string readUserID = $"SELECT count(1) FROM AllUsers where UserID = {userID}";
                        string writeUserID = $"insert into AllUsers (UserID, UserName) values ({userID}, '{userName}')";

                        SQLiteCommand commandRead = new SQLiteCommand(readUserID, conn);
                        SQLiteCommand commandWrite = new SQLiteCommand(writeUserID, conn);

                        int a = Convert.ToInt32(commandRead.ExecuteScalar());
                        if (a == 0 && userName != "")
                        {
                            commandWrite.ExecuteNonQuery();
                            conn.Close();
                            return false;
                        }
                        else
                        {
                            conn.Close();
                            return true;
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                LogsErrorWrite(ex.Message, "CheckUserID");
                return false;
            }
            
            
        }
        // Command off
        public string CheckReminder(long userID)
        {
            try
            {
                DateTime time = DateTime.MinValue;
                using (System.Data.SQLite.SQLiteConnection conn =
                    new System.Data.SQLite.SQLiteConnection($"data source={Properties.Resources.dataSource}"))
                {
                    using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                    {
                        conn.Open();
                        string readUserID = $"SELECT TimeReminder FROM AllUsers where UserID = {userID}";
                        string writeTimeUserID = $"update AllUsers set TimeReminder = null where UserID = {userID}";
                        SQLiteCommand commandRead = new SQLiteCommand(readUserID, conn);
                        SQLiteCommand commandWrite = new SQLiteCommand(writeTimeUserID, conn);
                        SQLiteDataReader readList = commandRead.ExecuteReader();
                        while (readList.Read())
                        {
                            if (readList["TimeReminder"].ToString() != "")
                            {
                                time = (DateTime)readList["TimeReminder"];
                            }

                        }
                        if (time != null && time != DateTime.MinValue)
                        {
                            commandWrite.ExecuteNonQuery();
                            conn.Close();
                            return "Нагадування видалено ";
                        }
                        else
                        {
                            conn.Close();
                            return "Нагадування відсутнє";
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                LogsErrorWrite(ex.Message, "CheckReminder");
                return "Нажаль виникла помилка, спробуйте виконати цю операцію пізніше";
            }
            
        }
        // Create reminder
        public string CreateReminder(long userID, DateTime time)
        {
            string res = "Нажаль виникла помилка, спробуйте виконати цю операцію пізніше";
            try
            {
                using (System.Data.SQLite.SQLiteConnection conn =
               new System.Data.SQLite.SQLiteConnection($"data source={Properties.Resources.dataSource}"))
                {
                    using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                    {
                        conn.Open();
                        string writeTimeUserID = $"update AllUsers set TimeReminder = '{time.ToShortTimeString()}' where UserID = {userID}";
                        SQLiteCommand commandWrite = new SQLiteCommand(writeTimeUserID, conn);
                        commandWrite.ExecuteNonQuery();
                        res = $"Нагадування створене на {time.ToString("HH:mm")}";
                        conn.Close();
                    }
                }
                return res;
            }
            catch(Exception ex)
            {
                return LogsErrorWrite(ex.Message, "CreateReminder");
                //return res;
            }
            
        }

        // Check time reminder
        public List<long> CheckTimeReminder()
        {
            try
            {
                DateTime time = DateTime.Now.AddSeconds(-DateTime.Now.Second);
                List<long> list = new List<long>();
                using (System.Data.SQLite.SQLiteConnection conn =
                    new System.Data.SQLite.SQLiteConnection($"data source={Properties.Resources.dataSource}"))
                {
                    using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                    {
                        conn.Open();
                        string readUserID = $"SELECT UserID FROM AllUsers where TimeReminder = '{time.ToShortTimeString()}'";
                        SQLiteCommand commandRead = new SQLiteCommand(readUserID, conn);
                        SQLiteDataReader readList = commandRead.ExecuteReader();
                        while (readList.Read())
                        {
                            if ((long)readList["UserID"] != 0)
                            {
                                list.Add((long)readList["UserID"]);
                            }

                        }
                        conn.Close();
                    }
                }
                return list;
            }
            catch(Exception ex)
            {
                LogsErrorWrite(ex.Message, "CheckTimeReminder");
                List<long> list = new List<long>();
                return list; 
            }
            
        }

        // Edit data last reminder
        public void UpdateDateLastReminder(long userID)
        {
            try
            {
                using (System.Data.SQLite.SQLiteConnection conn =
                               new System.Data.SQLite.SQLiteConnection($"data source={Properties.Resources.dataSource}"))
                {
                    using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                    {
                        conn.Open();
                        string writeTimeLastUpdateUserID = $"update AllUsers set LastReminder = '{DateTime.Now}' where UserID = {userID}";
                        SQLiteCommand commandWrite = new SQLiteCommand(writeTimeLastUpdateUserID, conn);
                        commandWrite.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                LogsErrorWrite(ex.Message, "UpdateDateLastReminder");
            }
            
        }  
        public string LogsErrorWrite(string text, string nameMethod)
        {
            try
            {
                using (System.Data.SQLite.SQLiteConnection conn =
                               new System.Data.SQLite.SQLiteConnection($"data source={Properties.Resources.dataSource}"))
                {
                    using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                    {
                        conn.Open();
                        string writeError = $"insert into LogErrors (TextError, MethodName) values ({text}, '{nameMethod}')";
                        SQLiteCommand commandWrite = new SQLiteCommand(writeError, conn);
                        commandWrite.ExecuteNonQuery();
                        conn.Close();
                        return "Нажаль виникла помилка, спробуйте виконати цю операцію пізніше";
                    }
                }
            }
            catch
            {
                return "Нажаль виникла помилка, спробуйте виконати цю операцію пізніше";
            }
        }
    }
}
