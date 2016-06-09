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
    static class ShowsTable
    {
        public static String TABLE_NAME = "Shows";
        public static String SQL_INSERT = "INSERT INTO Shows(Name, Director, hasGoldenGlobe, Genre) VALUES (:name, :director, :hasGoldenGlobe, :genre) RETURNING Shows.\"sID\" INTO :output";
        public static String SQL_SELECT = "SELECT * FROM Shows";
        public static String SQL_SELECT_SHOWSACTORSTARSIN = "SELECT Shows.\"sID\", Shows.Name, Shows.Director, Shows.hasGoldenGlobe,Shows.Genre,Shows.Stars FROM starsIn JOIN Shows on Shows.\"sID\" = starsin.Shows_sID WHERE StarsIn.Actors_aID = :actorID";
        public static String SQL_EDIT_NAME = "UPDATE Shows SET name = :name WHERE \"sID\" = :showID";
        public static String SQL_EDIT_DIRECTOR = "UPDATE Shows SET Director = :director WHERE \"sID\" = :showID";
        public static String SQL_EDIT_GENRE = "UPDATE Shows SET Genre = :genre WHERE \"sID\" = :showID";
        public static String SQL_SELECT_STARS = "SELECT Stars FROM Shows where  \"sID\" = :showID";
        public static String SQL_SELECT_SUGGESTED_SHOWS ="SELECT Name " +
                                                         "FROM Shows " +
                                                         "WHERE genre = ( " +
                                                                        "SELECT y.genre " +
                                                                        "FROM (" +
                                                                              "SELECT Shows.genre as genre, COUNT(*) as amount " +
                                                                              "FROM ShowsLibrary " +
                                                                              "JOIN Shows ON ShowsLibrary.Shows_sID = Shows.\"sID\" " +
                                                                              "GROUP BY Shows.genre, ShowsLibrary.Users_uID " +
                                                                              "HAVING ShowsLibrary.Users_uID =:userID ORDER BY amount DESC" +
                                                                             ") y " +
                                                                        "WHERE rownum = 1 " +
                                                                       ")" +
                                                         "AND HASGOLDENGLOBE = 1 " +
                                                         "AND Name NOT IN( " +
                                                                         "SELECT Shows.Name from Shows " +
                                                                         "INNER JOIN ShowsLibrary on ShowsLibrary.Shows_sID = Shows.\"sID\" " +
                                                                         "WHERE ShowsLibrary.Users_uID =:userID " +
                                                                        ")";
        public static String SQL_SELECT_TOP_10 = "SELECT y.name FROM ( SELECT Shows.Name as name, AVG(Rating.Percentage) as avg FROM Rating JOIN Shows on Shows.\"sID\" = Rating.Shows_sID GROUP BY Shows_sID, Shows.Name order by avg desc ) y WHERE ROWNUM <= 10";
        public static String SQL_SHOW_DETAIL = "SELECT Shows.Name, Shows.Director, Shows.Genre, Shows.Stars, y.percentage, c.name " +
                                               "FROM Shows " +
                                               "JOIN(SELECT AVG(Rating.Percentage) as percentage, Rating.Shows_sID as ID " +
                                                    "FROM Rating " +
                                                    "GROUP BY Shows_sID  " +
                                                     ") " +
                                               "y on Shows.\"sID\" = y.ID " +
                                               "JOIN (SELECT Actors.Name as name, StarsIn.Shows_sID as ID " +
                                               "FROM starsin JOIN Actors ON Actors.\"aID\" = starsin.Actors_aID" +
                                               ") c on Shows.\"sID\" = c.ID " +
                                               "WHERE Shows.\"sID\" =:showID";
        public static String SQL_LINK_ACTORSHOW = "INSERT INTO StarsIn VALUES (:actorID,:showID)";
        public static String SQL_SELECT_LISTING = "SELECT Shows.Name, Shows.\"sID\" FROM shows";
        public static String SQL_SELECT_LISTINGDETAIL_BY_SID = "SELECT Shows.Director, Shows.hasGoldenGlobe, Shows.Genre FROM Shows WHERE Shows.\"sID\" =:showID";

        public static Collection<Show> getListing()
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_LISTING);
            OracleDataReader reader = db.Select(command);
            Collection<Show> shows = new Collection<Show>();

            while (reader.Read())
            {
                Show s = new Show();
                s.Name = reader.GetString(0);
                s.sID = reader.GetInt32(1);
                shows.Add(s);
            }
            reader.Close();
            db.Close();
            return shows;
        }

        public static void getListingDetail(Show s)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_LISTINGDETAIL_BY_SID);
            command.Parameters.Add(":showID", s.sID);
            OracleDataReader reader = db.Select(command);
            while (reader.Read())
            {
                s.Director = reader.GetString(0);
                s.hasGoldenGlobe = reader.GetInt32(1) == 1;
                s.Genre = reader.GetString(2);
            }
            reader.Close();
            db.Close();
        }

        public static void linkShowActor(Show s, Actor a)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_LINK_ACTORSHOW);
            command.Parameters.Add(":actorID", a.aID);
            command.Parameters.Add(":showID", s.sID);
            
            int ret = db.ExecuteNonQuery(command);
            if(ret != 0){
                s.starring.Add(a);
                a.starsIn.Add(s);
            }
        }

        public static void readShowDetail(Show s)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SHOW_DETAIL);
            command.Parameters.Add(":showID", s.sID);
            OracleDataReader reader = db.Select(command);
            while (reader.Read())
            {
                s.Name = reader.GetString(0);
                s.Director = reader.GetString(1);
                s.Genre = reader.GetString(2);
                s.Stars = reader.GetInt32(3);
                s.average = reader.GetDouble(4);
                Actor a = new Actor();
                a.Name = reader.GetString(5);
                s.starring.Add(a);
            }
            reader.Close();
            db.Close();
        }

        public static Collection<Show> topTenShows(){
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_SELECT_TOP_10);
            OracleDataReader reader = db.Select(command);

            Collection<Show> topTen = new Collection<Show>();

            while (reader.Read())
            {
                Show s = new Show();
                s.Name = reader.GetString(0);
                topTen.Add(s);
            }
            reader.Close();
            db.Close();
            return topTen;
        }

        public static void updateName(Show s, string newName)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_EDIT_NAME);
            command.BindByName = true;
            command.Parameters.Add(":name", newName);
            command.Parameters.Add(":showID", s.sID);
            int ret = db.ExecuteNonQuery(command);
            if (ret != 0)
            {
                s.Name = newName;
            }

        }

        public static void updateDirector(Show s, string newDirector)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_EDIT_DIRECTOR);
            command.BindByName = true;
            command.Parameters.Add(":director", newDirector);
            command.Parameters.Add(":showID", s.sID);
            int ret = db.ExecuteNonQuery(command);
            if (ret != 0)
            {
                s.Director = newDirector;
            }
        }

        public static void awardGoldenGlobe(int sID) /////// STORED PROCEDURE 2
        {
            Database db = new Database();
            db.Connect();

            OracleCommand command = db.CreateCommand("ActorAwards");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("showID", OracleDbType.Int32, sID, ParameterDirection.Input);
            db.ExecuteNonQuery(command);
        }

        public static void updateGenre(Show s, string newGenre)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_EDIT_GENRE);
            command.BindByName = true;
            command.Parameters.Add(":genre", newGenre);
            command.Parameters.Add(":showID", s.sID);
            int ret = db.ExecuteNonQuery(command);
            if (ret != 0)
            {
                s.Genre = newGenre;
            }
        }

        public static int getStarsByID(int showID){
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_STARS);
                command.Parameters.Add(":showID", showID);

                OracleDataReader reader = db.Select(command);

                int stars = 0;
                if (reader.Read()){
                    stars = reader.GetInt32(0);
                }
                reader.Close();
                db.Close();
                return stars;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return 0; ;
            }

        }

        public static void insert(Show s)
        {
            Database db = new Database();
            db.Connect();
            OracleCommand command = db.CreateCommand(SQL_INSERT);

            command.BindByName = true;
            command.Parameters.Add(":name", s.Name);
            command.Parameters.Add(":director", s.Director);
            command.Parameters.Add(":hasGoldenGlobe", Convert.ToInt32(s.hasGoldenGlobe));
            command.Parameters.Add(":genre", s.Genre);
            command.Parameters.Add(":output", OracleDbType.Int32, ParameterDirection.Output);
            
            db.ExecuteNonQuery(command);
            s.sID = Convert.ToInt32(command.Parameters[":output"].Value.ToString());
        }

        public static Collection<Show> getShowsActorStarsIn(int aID)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_SHOWSACTORSTARSIN);
                command.Parameters.Add(":actorID", aID);
                OracleDataReader reader = db.Select(command);

                Collection<Show> shows = Read(reader);
                reader.Close();
                db.Close();
                return shows;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        public static void getSuggestedShows(User u)
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT_SUGGESTED_SHOWS);
                command.Parameters.Add(":userID", u.uID);

                OracleDataReader reader = db.Select(command);
            
                while (reader.Read())
                {
                    Show s = new Show();
                    s.Name = reader.GetString(0);
                    u.suggestedShows.Add(s);
                }
                reader.Close();
                db.Close();
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
            }
        }

        public static Collection<Show> getAllShows()
        {
            Database db = new Database();
            if (db.Connect())
            {
                OracleCommand command = db.CreateCommand(SQL_SELECT);
                OracleDataReader reader = db.Select(command);

                Collection<Show> shows = Read(reader);
                reader.Close();
                db.Close();
                return shows;
            }
            else
            {
                Console.WriteLine("Connection to the database failed, please try again!");
                return null;
            }
        }

        private static Collection<Show> Read(OracleDataReader reader)
        {
            Collection<Show> Shows = new Collection<Show>();

            while (reader.Read())
            {
                int i = -1;
                Show s = new Show();
                s.sID = reader.GetInt32(++i);
                s.Name = reader.GetString(++i);
                s.Director = reader.GetString(++i);
                s.hasGoldenGlobe = reader.GetInt32(++i) == 1;
                s.Genre = reader.GetString(++i);
                s.Stars = reader.GetInt32(++i);
                Shows.Add(s);
            }
            return Shows;
        }

    }
}
