using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tracktv.DTO;
using System.Collections.ObjectModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Tracktv.DAO
{
    static class CommentsTable
    {
        public static String TABLE_NAME = "Comments";
        public static String SQL_INSERT = "INSERT INTO Comments(Users_uID,Shows_sID,Text) VALUES (:userID, :showID, :Text) RETURNING Comments.\"cID\" INTO :output";
        public static String SQL_SELECT = "SELECT * FROM Comments WHERE Comments.Shows_sID=:sID";
        public static String SQL_UPVOTE = "UPDATE Comments SET Score = Score + 1 WHERE \"cID\" = :cID";
        public static String SQL_DOWNVOTE = "UPDATE Comments SET Score = Score - 1 WHERE \"cID\" = :cID";
        public static String SQL_DELETE = "DELETE FROM Comments WHERE \"cID\" = :cID";
        public static String SQL_SELECT_COMMENTS_BY_SHOWID = "SELECT Comments.\"cID\", Comments.Text, Users.Login, Comments.Score, Comments.Shows_sID FROM Comments JOIN Users on Users.\"uID\" = Comments.Users_uID WHERE Comments.Shows_sID =:showID";

        public static void getComments(Show s)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_COMMENTS_BY_SHOWID);
            command.Parameters.Add(":showID", s.sID);
            OracleDataReader reader = db.Select(command);
            bool exists = false;
            while (reader.Read())
            {
                Comment c = new Comment();
                c.cID = reader.GetInt32(0);
                c.Text = reader.GetString(1);
                c.userLogin = reader.GetString(2);
                c.Score = reader.GetInt32(3);
                foreach(Comment com in s.comments)
                {
                    if(com.cID == c.cID)
                    {
                        com.Score = c.Score;
                        exists = true;
                    }
                }
                if (!exists)
                {
                    s.comments.Add(c);
                }
                exists = false;
            }

            reader.Close();
            db.Close();

        }

        public static int delete(int cID)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_DELETE);
            command.BindByName = true;
            command.Parameters.Add(":cID", cID);
            int ret = db.ExecuteNonQuery(command);

            return ret;
        }

        public static void upvote(int cID)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_UPVOTE);
            command.BindByName = true;
            command.Parameters.Add(":cID", cID);
            int ret = db.ExecuteNonQuery(command);
           
        }

        public static void downvote(int cID)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_DOWNVOTE);
            command.BindByName = true;
            command.Parameters.Add(":cID", cID);
            int ret = db.ExecuteNonQuery(command);
        }

        public static void insert(Comment c)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_INSERT);

            command.Parameters.Add(":userID", c.Users_uID);
            command.Parameters.Add(":showID", c.Shows_sID);
            command.Parameters.Add(":Text", c.Text);
            command.Parameters.Add(":output", OracleDbType.Int32, ParameterDirection.Output);

            db.ExecuteNonQuery(command);
            c.cID = Convert.ToInt32(command.Parameters[":output"].Value.ToString());
        }


        private static Collection<Comment> Read(OracleDataReader reader)
        {
            Collection<Comment> comments = new Collection<Comment>();

            while (reader.Read())
            {
                int i = -1;
                Comment c = new Comment();
                c.cID = reader.GetInt32(++i);
                c.Users_uID = reader.GetInt32(++i);
                c.Shows_sID = reader.GetInt32(++i);
                c.Text = reader.GetString(++i);
                c.Datetime = reader.GetDateTime(++i);
                c.Score = reader.GetInt32(++i);
                comments.Add(c);
                Console.WriteLine(c.Text);
            }
            return comments;
        }
    }
}
