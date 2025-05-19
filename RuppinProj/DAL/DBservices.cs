using RuppinProj;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using RuppinProj.BL;

namespace RuppinProj
{
    public class DBservices
    {
        public static string connectionString;

        static DBservices()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            connectionString = config.GetConnectionString("myProjDB");
        }
        // 1. Insert Movie
        public bool InsertMovie(Movie m)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertMovieSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                m.AddSqlParameters(cmd);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        // 2. Get All Movies
        public List<Movie> GetAllMovies()
        {
            List<Movie> movies = new List<Movie>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetAllMoviesSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        movies.Add(Movie.FromReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching movies: " + ex.Message);
            }

            return movies;
        }

        // 3. Get Movies By Title
        public List<Movie> GetByTitle(string title)
        {
            List<Movie> movies = new List<Movie>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetByTitleSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PrimaryTitle", title);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        movies.Add(Movie.FromReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching movies by title: " + ex.Message);
            }

            return movies;
        }

        // 4. Get Movies By Release Date Range
        public List<Movie> GetByReleaseDate(DateTime startDate, DateTime endDate)
        {
            List<Movie> movies = new List<Movie>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetByReleaseDateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        movies.Add(Movie.FromReader(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching movies by release date: " + ex.Message);
            }

            return movies;
        }

        // 5. Delete Movie
        public bool DeleteMovie(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DeleteMovieSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    con.Close();

                    if (result != null && Convert.ToInt32(result) == 1)
                        return true; // מחיקה הצליחה
                    else
                        return false; // הסרט כבר מחוק או לא קיים
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting movie: " + ex.Message);
                return false;
            }
        }

        public Movie GetMovieById(int id)
        {
            Movie movie = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetMovieByIdSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    movie = Movie.FromReader(reader);
                }
            }
            return movie;
        }
        public bool InsertMoviesBulk(List<Movie> movies)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    foreach (var movie in movies)
                    {
                        SqlCommand cmd = new SqlCommand("InsertMovieSP", con, transaction); // משתמש ב-SP שלך
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        movie.AddSqlParameters(cmd); // במקום לרשום שדה-שדה ידנית

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw; // במקום רק להדפיס
                }


            }
        }
        public bool UpdateMovie(int id, Movie updatedMovie)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateMovieSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", id);
                    updatedMovie.AddSqlParameters(cmd);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating movie: {ex.Message}");

            }

        }
        public void RentMovie(int userId, int movieId, DateTime rentStart, DateTime rentEnd)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("RentMovieSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@MovieId", movieId);
                cmd.Parameters.AddWithValue("@RentStart", rentStart);
                cmd.Parameters.AddWithValue("@RentEnd", rentEnd);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Movie> GetRentedMoviesByUser(int userId)
        {
            List<Movie> rentedMovies = new List<Movie>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetRentedMoviesByUserSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    rentedMovies.Add(Movie.FromReader(reader));
                }

                reader.Close();
            }

            return rentedMovies;
        }



        public bool CancelRental(int userId, int movieId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("CancelRentalSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@MovieId", movieId);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool TransferRental(int fromUserId, int toUserId, int movieId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("TransferRentalSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromUserId", fromUserId);
                cmd.Parameters.AddWithValue("@ToUserId", toUserId);
                cmd.Parameters.AddWithValue("@MovieId", movieId);

                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    // זיהוי שגיאת FOREIGN KEY
                    if (ex.Message.Contains("FOREIGN KEY"))
                        throw new Exception("Cannot transfer rental: target user does not exist.");

                    // אפשרות: לטפל בשגיאות אחרות כאן אם צריך

                    throw; // להעביר שגיאות אחרות כמות שהן
                }
            }
        }
    }
}
