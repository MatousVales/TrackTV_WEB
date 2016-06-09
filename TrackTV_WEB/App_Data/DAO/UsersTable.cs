using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tracktv.DTO;
using System.Collections.ObjectModel;
using System.Data;

namespace Tracktv.DAO
{
    static class UsersTable
    {
        public static String TABLE_NAME = "Users";
        public static String SQL_SELECT_DETAIL = "SELECT Users.DailyScore, Comments.Text, Comments.Score, y.showname, y.countOfEpisodes, a.ActorName,a.timesInHistory " +
                                                  "FROM Users " +
                                                  "JOIN Comments ON Users.\"uID\" = Comments.Users_uID " +
                                                  "JOIN (" +
                                                        "SELECT History.Users_uID as ID, Shows.Name as showname, COUNT(*) as countOfEpisodes " +
                                                        "FROM History " +
                                                        "JOIN Episodes on Episodes.\"eID\" = History.EPISODES_EID " +
                                                        "JOIN Shows on Shows.\"sID\" = Episodes.Shows_sID " +
                                                        "GROUP BY History.Users_uID, Shows.Name " +
                                                        "HAVING History.Users_uID=:userID " +
                                                        ") y ON Users.\"uID\" = y.ID " +
                                                  "JOIN (" +
                                                        "SELECT History.Users_uID as userID, Actors.Name AS ActorName, Count(Actors.Name) AS timesInHistory " +
                                                        "FROM History " +
                                                        "INNER JOIN Users ON Users.\"uID\" = history.users_uID " +
                                                        "INNER JOIN Episodes ON Episodes.\"eID\" = history.episodes_eID " +
                                                        "INNER JOIN ShowsLibrary ON ShowsLibrary.Shows_sID = Episodes.shows_sID " +
                                                        "INNER JOIN Shows ON Shows.\"sID\" = ShowsLibrary.Shows_sID " +
                                                        "INNER JOIN StarsIn ON StarsIn.Shows_sID = Shows.\"sID\" " +
                                                        "INNER JOIN Actors ON Actors.\"aID\" = StarsIn.Actors_aID " +
                                                        "GROUP BY ShowsLibrary.Users_uID, History.Users_uID, Actors.Name " +
                                                        "HAVING ShowsLibrary.Users_uID =:userID AND History.Users_uID =:userID ORDER BY timesInHistory DESC " +
                                                        ") a on Users.\"uID\" = a.userID " +
                                                  "WHERE Comments.SCORE = (SELECT MAX(Score) " +
                                                                          "FROM Comments " +
                                                                          "GROUP BY  Users_uID " +
                                                                          "HAVING Users_uID =:userID" +
                                                                          ")" +
                                                                          " AND rownum = 1";

        public static String SQL_SELECT_USER = "SELECT * FROM Users WHERE Users.\"uID\"=:userID";
        public static String SQL_SELECT_LOGIN = "SELECT Users.Password, Users.\"uID\" FROM Users WHERE Users.Login=:Login";


        public static User CalculateDailyScore() //// STORED PROCEDURE DAILYSCORE
        {
            Database db = new Database();
            db.Connect();

            OracleCommand command = db.CreateCommand("DailyScore");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("score", OracleDbType.Int32, ParameterDirection.Output);
            OracleParameter outcomeparam = command.Parameters.Add("userLogin", OracleDbType.Varchar2);
            outcomeparam.Direction = ParameterDirection.Output;
            outcomeparam.Size = 20;
            db.ExecuteNonQuery(command);
            int score = Convert.ToInt32(command.Parameters["score"].Value.ToString());
            if(score == 0)
            {
                return null;
            } else
            {
                User a = new User();
                a.Login = command.Parameters["userLogin"].Value.ToString();
                a.DailyScore = score;
                return a;
            }
        }

        public static User register(String fName, String pwd, bool admin) //// STORED PROCEDURE ADD USER
        {
            Database db = new Database();
            db.Connect();

            OracleCommand command = db.CreateCommand("AddUser");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("fName", OracleDbType.Varchar2, fName,ParameterDirection.Input);
            command.Parameters.Add("pwd", OracleDbType.Varchar2, pwd, ParameterDirection.Input);
            command.Parameters.Add("isuseradmin", OracleDbType.Int32, Convert.ToInt32(admin), ParameterDirection.Input);
            command.Parameters.Add("userID", OracleDbType.Int32, ParameterDirection.Output);

            db.ExecuteNonQuery(command);
            User u = getUserByID(Convert.ToInt32(command.Parameters["userID"].Value.ToString()));
            return u;
        }

        public static User login(String Login, String password)
        {
            Database db = new Database();
            if (db.Connect()){
                OracleCommand command = db.CreateCommand(SQL_SELECT_LOGIN);
                command.Parameters.Add(":Login", Login);

                OracleDataReader reader = db.Select(command);
                
                reader.Read();
                if (reader.GetString(0).Equals(password)){
                   User u = getUserByID(reader.GetInt32(1));
                   reader.Close();
                   db.Close();
                   return u;
                }
                else
                {
                   reader.Close();
                   db.Close();
                   return null;
                }
            }
            else
            {
                //connection failed
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        public static User getUserByID(int uID)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_USER);
                command.Parameters.Add(":userID", uID);

                OracleDataReader reader = db.Select(command);

                Collection<User> users = Read(reader);
                reader.Close();
                db.Close();
                return users[0];
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        public static Collection<User> getAllUsers()
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_USER);
                OracleDataReader reader = db.Select(command);

                Collection<User> users = Read(reader);
                reader.Close();
                db.Close();
                return users;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        public static bool getUserDetail(User u)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_DETAIL);
                command.Parameters.Add(":userID", u.uID);
                OracleDataReader reader = db.Select(command);
                if (reader.Read()){
                     u.DailyScore = reader.GetInt32(0);
                     u.bestComment = new Comment();
                     u.bestComment.Text = reader.GetString(1);
                     u.bestComment.Score = reader.GetInt32(2);
                     u.mostWatchedShow = new Show();
                     u.mostWatchedShow.Name = reader.GetString(3);
                     u.mostWatchedShow.timesInUserHistory = reader.GetInt32(4);
                     u.mostWatchedActor = new Actor();
                     u.mostWatchedActor.Name = reader.GetString(5);
                     u.mostWatchedActor.timesInUserHistory = reader.GetInt32(6);
                     reader.Close();
                     db.Close();
                     return true;
                 }
                 else
                 {
                     reader.Close();
                     db.Close();
                     return false;
                 }
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return false;
            }
        }

        private static Collection<User> Read(OracleDataReader reader)
        {
            Collection<User> Users = new Collection<User>();

            while (reader.Read())
            {
                int i = -1;
                User u = new User();
                u.uID = reader.GetInt32(++i);
                u.Login = reader.GetString(++i);
                u.Password = reader.GetString(++i);
                u.isAdmin = reader.GetInt32(++i) == 1;
                u.Name = reader.GetString(++i);
                u.DailyScore = reader.GetInt32(++i);
                Users.Add(u);
            }
            return Users;
        }

    }
}
