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
    static class RatingsTable
    {
        public static String SQL_INSERT = "INSERT INTO Rating(Users_uID,Shows_sID,Percentage) VALUES (:uID, :sID, :percentage)";
        public static String SQL_SELECT = "SELECT * FROM Rating WHERE Rating.Shows_sID=:sID";

        public static Collection<Rating> getAllRatings(int sID)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT);
                command.Parameters.Add(":sID", sID);
                OracleDataReader reader = db.Select(command);

                Collection<Rating> ratings = Read(reader);
                reader.Close();
                db.Close();
                return ratings;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        public static bool rateShow(Rating r) //// STORED PROCEDURE RATE SHOW + STAR RATING + ACTORSAVERAGE
        {
            Database db = new Database();
            db.Connect();

            OracleCommand command = db.CreateCommand("RateShow");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("userId", OracleDbType.Int32,r.Users_uID, ParameterDirection.Input);
            command.Parameters.Add("showID", OracleDbType.Int32,r.Shows_sID, ParameterDirection.Input);
            command.Parameters.Add("percentage", OracleDbType.Int32, r.Percentage, ParameterDirection.Input);
            OracleParameter outcomeparam = command.Parameters.Add("outcome", OracleDbType.Varchar2);
            outcomeparam.Direction = ParameterDirection.Output;
            outcomeparam.Size = 20;

            db.ExecuteNonQuery(command);

            String result = command.Parameters["outcome"].Value.ToString();
            if (result.Equals("SUCCESS"))
            {
                return true;
            } else
            {
                return false;
            }
        }


        private static Collection<Rating> Read(OracleDataReader reader)
        {
            Collection<Rating> ratings = new Collection<Rating>();

            while (reader.Read())
            {
                int i = -1;
                Rating r = new Rating();
                r.Users_uID = reader.GetInt32(++i);
                r.Shows_sID = reader.GetInt32(++i);
                r.Percentage = reader.GetInt32(++i);
                ratings.Add(r);
            }
            return ratings;
        }

    }
}
