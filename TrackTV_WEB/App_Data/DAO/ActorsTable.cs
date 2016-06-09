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
    static class ActorsTable
    {
        public static String TABLE_NAME = "Actors";
        public static String SQL_INSERT = "INSERT INTO Actors(Name, Gender) VALUES (:name, :gender) RETURNING Actors.\"aID\" INTO :output";
        public static String SQL_SELECT = "SELECT * FROM Actors";
        public static String SQL_SELECT_BYID = "SELECT * FROM Actors where aID = :actorID";
        public static String SQL_SELECT_STARRING = "SELECT Actors.\"aID\", Actors.Name, Actors.Gender, Actors.Amountofawards, Actors.average FROM starsin JOIN Actors ON Actors.\"aID\" = starsin.Actors_aID WHERE StarsIn.Shows_sID = :showID";
        public static String SQL_EDIT_NAME = "UPDATE Actors SET name = :name WHERE \"aID\" = :actorID";
        public static String SQL_EDIT_GENDER = "UPDATE Actors SET Gender = :gender WHERE \"aID\" = :actorID";
        public static String SQL_SELECT_BEST_BY_AVERAGE = "SELECT Name FROM ( SELECT Name FROM Actors GROUP BY Name, Average ORDER BY AVERAGE desc) WHERE rownum = 1";
        public static String SQL_SELECT_ACTOR_BYID = "Select Name, Gender, AmountOfAwards, Average from Actors where \"aID\" =:actorID";


        public static Actor getBestActor()
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_BEST_BY_AVERAGE);
            OracleDataReader reader = db.Select(command);
            
            if (reader.Read())
            {
                Actor a = new Actor();
                a.Name = reader.GetString(0);
                return a;
            } else
            {
                return null;
            }
        }

        public static void getActorDetail(Actor a)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_ACTOR_BYID);
            command.Parameters.Add(":actorID", a.aID);
            OracleDataReader reader = db.Select(command);

            if (reader.Read())
            {
                a.Name = reader.GetString(0);
                a.Gender = reader.GetString(1);
                a.AmountOfAwards = reader.GetInt32(2);
                a.Average = reader.GetDouble(3);
                reader.Close();
                db.Close();
            }
            else
            {
                reader.Close();
                db.Close();
            }
        }

        public static void execActorsAverage() //// STORED PROCEDURE
        {
            Database db = new Database();
            db.Connect();

            OracleCommand command = db.CreateCommand("ActorsAverage");
            command.CommandType = CommandType.StoredProcedure;
            db.ExecuteNonQuery(command);
        }

        public static void updateName(Actor a, string newName)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_EDIT_NAME);
            command.BindByName = true;
            command.Parameters.Add(":name", newName);
            command.Parameters.Add(":actorID", a.aID);
            int ret = db.ExecuteNonQuery(command);
            if(ret != 0)
            {
                a.Name = newName;
            }
        }

        public static void updateGender(Actor a, string newGender)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_EDIT_GENDER);
            command.BindByName = true;
            command.Parameters.Add(":gender", newGender);
            command.Parameters.Add(":actorID", a.aID);
            int ret = db.ExecuteNonQuery(command);
            if (ret != 0)
            {
                a.Gender = newGender;
            }
        }

        public static int insert(Actor a)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_INSERT);

            command.BindByName = true;
            command.Parameters.Add(":name", a.Name);
            command.Parameters.Add(":gender", a.Gender);
            command.Parameters.Add(":output", OracleDbType.Int32, ParameterDirection.Output);

            int ret = db.ExecuteNonQuery(command);

            a.aID = Convert.ToInt32(command.Parameters[":output"].Value.ToString());
            return ret;
        }

        public static Collection<Actor> getAllActors()
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT);
                OracleDataReader reader = db.Select(command);

                Collection<Actor> actors = Read(reader);
                reader.Close();
                db.Close();
                return actors;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        public static Collection<Actor> getStarringActors(int sID)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_STARRING);
                command.Parameters.Add(":showID", sID);
                OracleDataReader reader = db.Select(command);

                Collection<Actor> actors = Read(reader);
                reader.Close();
                db.Close();
                return actors;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        private static Collection<Actor> Read(OracleDataReader reader)
        {
            Collection<Actor> actors = new Collection<Actor>();

            while (reader.Read())
            {
                int i = -1;
                Actor a = new Actor();
                a.aID = reader.GetInt32(++i);
                a.Name = reader.GetString(++i);
                a.Gender = reader.GetString(++i);
                a.AmountOfAwards = reader.GetInt32(++i);
                a.Average = reader.GetDouble(++i);
                a.starsIn = ShowsTable.getShowsActorStarsIn(a.aID);
                actors.Add(a);
            }
            return actors;
        }

    }
}
