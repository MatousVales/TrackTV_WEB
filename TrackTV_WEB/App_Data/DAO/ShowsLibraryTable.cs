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
    static class ShowsLibraryTable
    {
        public static String SQL_SELECT = "SELECT * FROM  iShowsLibrary WHERE ShowsLibrary.Users_uID=:uID";
        public static String SQL_FOLLOW = "INSERT INTO ShowsLibrary VALUES (:userID, :showID, 1)";
        public static String SQL_SELECT_UPCOMING = "SELECT episodes.airingdate, episodes.Name, Shows.Name " +
                                                   "FROM episodes " +
                                                   "JOIN Shows on Shows.\"sID\" = episodes.Shows_sID " +
                                                   "JOIN ShowsLibrary on ShowsLibrary.Shows_sID = Shows.\"sID\" " +
                                                   "WHERE ShowsLibrary.USERS_UID =:userID " +
                                                   "AND ShowsLibrary.ISFOLLOWED = '1' " +
                                                   "AND episodes.AIRINGDATE >= TRUNC(SYSDATE) " +
                                                   "AND episodes.AIRINGDATE <= TRUNC(SYSDATE) + 7";

        public static bool getUpcomingEpisodes(User u)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_UPCOMING);
            command.Parameters.Add(":userID", u.uID);
            OracleDataReader reader = db.Select(command);

            while (reader.Read())
            {
                Episode e = new Episode();
                e.Airingdate = reader.GetDateTime(0);
                e.Name = reader.GetString(1);
                e.ShowName = reader.GetString(2);
                u.upcomingEpisodes.Add(e);
            }
            
            if(u.upcomingEpisodes.Count != 0)
            {
                reader.Close();
                db.Close();
                return true;
            } else
            {
                reader.Close();
                db.Close();
                return false;
            }
        }

        public static void followShow(User u, Show s)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_FOLLOW);

            command.BindByName = true;
            command.Parameters.Add(":userID", u.uID);
            command.Parameters.Add(":showID", s.sID);

            int ret = db.ExecuteNonQuery(command);
            if (ret != 0)
            {
                ShowsLibraryEntry se = new ShowsLibraryEntry();
                se.Users_uID = u.uID;
                se.Shows_sID = s.sID;
                se.isFollowed = true;
                u.usersLibrary.Add(se);
            }
        }


        public static Collection<ShowsLibraryEntry> getUserLibrary(int uID)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT);
                command.Parameters.Add(":uID", uID);
                OracleDataReader reader = db.Select(command);

                Collection<ShowsLibraryEntry> ShowsLibrary = Read(reader);
                reader.Close();
                db.Close();
                return ShowsLibrary;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

       
        private static Collection<ShowsLibraryEntry> Read(OracleDataReader reader)
        {
            Collection<ShowsLibraryEntry> ShowsLibrary = new Collection<ShowsLibraryEntry>();

            while (reader.Read())
            {
                int i = -1;
                ShowsLibraryEntry s = new ShowsLibraryEntry();
                s.Users_uID = reader.GetInt32(++i);
                s.Shows_sID = reader.GetInt32(++i);
                s.isFollowed = reader.GetInt32(++i) == 1;
                ShowsLibrary.Add(s);
            }
            return ShowsLibrary;
        }

    }
}
