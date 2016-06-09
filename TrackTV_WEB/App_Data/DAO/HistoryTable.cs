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
    static class HistoryTable
    {
        public static String TABLE_NAME = "History";
        public static String SQL_INSERT = "insert into History(users_uID,episodes_eid) values(:userID, :episodeID) RETURNING History.Datetime INTO :output";
        public static String SQL_SELECT = "SELECT History.Users_uID, History.Episodes_eID, History.Datetime, Shows.Name, Episodes.Name FROM History inner join Users on Users.\"uID\" = History.Users_uID inner join Episodes on Episodes.\"eID\" = History.Episodes_eID inner join Shows on Shows.\"sID\" = Episodes.Shows_sID WHERE History.Users_uID=:userID";

        public static void markAsWatched(HistoryEntry h)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_INSERT);

            command.Parameters.Add(":userID", h.Users_uID);
            command.Parameters.Add(":episodeID", h.Episodes_eID);
            command.Parameters.Add(":output", OracleDbType.Date, ParameterDirection.Output);
            int ret = db.ExecuteNonQuery(command);
            if (ret != 0)
            {
                h.Datetime = Convert.ToDateTime(command.Parameters[":output"].Value.ToString());
            }
            
        }

        public static void getUserHistory(User u)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT);
                command.Parameters.Add(":userID", u.uID);
                OracleDataReader reader = db.Select(command);

                u.history = Read(reader);
                reader.Close();
                db.Close();
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
            }
        }

        private static Collection<HistoryEntry> Read(OracleDataReader reader)
        {
            Collection<HistoryEntry> history = new Collection<HistoryEntry>();

            while (reader.Read())
            {
                int i = -1;
                HistoryEntry h = new HistoryEntry();
                h.Users_uID = reader.GetInt32(++i);
                h.Episodes_eID = reader.GetInt32(++i);
                h.Datetime = reader.GetDateTime(++i);
                h.showName = reader.GetString(++i);
                h.episodeName = reader.GetString(++i);
                history.Add(h);
            }
            return history;
        }

    }
}
