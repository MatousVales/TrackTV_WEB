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
    static class EpisodesTable
    {
        public static String TABLE_NAME = "Episodes";
        public static String SQL_INSERT = "INSERT INTO Episodes(Shows_sID,Name,Airingdate) VALUES (:sID, :name, :airingdate) RETURNING Episodes.\"eID\" INTO :output";
        public static String SQL_SELECT = "SELECT * FROM Episodes WHERE Episodes.Shows_sID=:sID";


        public static void insert(Episode e)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_INSERT);

            command.BindByName = true;
            command.Parameters.Add(":sID", e.Shows_sID);
            command.Parameters.Add(":name", e.Name);
            command.Parameters.Add(":airingdate", e.Airingdate);
            command.Parameters.Add(":output", OracleDbType.Int32, ParameterDirection.Output);

            int ret = db.ExecuteNonQuery(command);

            e.eID = Convert.ToInt32(command.Parameters[":output"].Value.ToString());

        }

        public static Collection<Episode> getAllEpisodes(int sID)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT);
                command.Parameters.Add(":sID", sID);
                OracleDataReader reader = db.Select(command);

                Collection<Episode> episodes = Read(reader);
                reader.Close();
                db.Close();
                return episodes;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        private static Collection<Episode> Read(OracleDataReader reader)
        {
            Collection<Episode> episodes = new Collection<Episode>();

            while (reader.Read())
            {
                int i = -1;
                Episode e = new Episode();
                e.eID = reader.GetInt32(++i);
                e.Shows_sID = reader.GetInt32(++i);
                e.Name = reader.GetString(++i);
                e.Airingdate = reader.GetDateTime(++i);
                episodes.Add(e);
                Console.WriteLine(e.Name);
            }
            return episodes;
        }

    }
}
