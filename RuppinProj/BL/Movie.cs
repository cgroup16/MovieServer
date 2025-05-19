using System.Data.SqlClient;

namespace RuppinProj
{
    public class Movie
    {
        int id;
        string url;
        string primaryTitle;
        string description;
        string primaryImage;
        int year;
        DateTime releaseDate;
        string language;
        double budget;
        double grossWorldwide;
        string genres;
        bool isAdult;
        int runtimeMinutes;
        float averageRating;
        int numVotes;

        public Movie(int id, string url, string primaryTitle, string description, string primaryImage, int year, DateTime releaseDate, string language, double budget, double grossWorldwide, string genres, bool isAdult, int runtimeMinutes, float averageRating, int numVotes)
        {
            Id = id;
            Url = url;
            PrimaryTitle = primaryTitle;
            Description = description;
            PrimaryImage = primaryImage;
            Year = year;
            ReleaseDate = releaseDate;
            Language = language;
            Budget = budget;
            GrossWorldwide = grossWorldwide;
            Genres = genres;
            IsAdult = isAdult;
            RuntimeMinutes = runtimeMinutes;
            AverageRating = averageRating;
            NumVotes = numVotes;
        }

        public int Id { get => id; set => id = value; }
        public string Url { get => url; set => url = value; }
        public string PrimaryTitle { get => primaryTitle; set => primaryTitle = value; }
        public string Description { get => description; set => description = value; }
        public string PrimaryImage { get => primaryImage; set => primaryImage = value; }
        public int Year { get => year; set => year = value; }
        public DateTime ReleaseDate { get => releaseDate; set => releaseDate = value; }
        public string Language { get => language; set => language = value; }
        public double Budget { get => budget; set => budget = value; }
        public double GrossWorldwide { get => grossWorldwide; set => grossWorldwide = value; }
        public string Genres { get => genres; set => genres = value; }
        public bool IsAdult { get => isAdult; set => isAdult = value; }
        public int RuntimeMinutes { get => runtimeMinutes; set => runtimeMinutes = value; }
        public float AverageRating { get => averageRating; set => averageRating = value; }
        public int NumVotes { get => numVotes; set => numVotes = value; }

        public int priceToRent { get; set; }






        public Movie() { } //בנאי ריק         

        public void AddSqlParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@PrimaryTitle", PrimaryTitle);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@PrimaryImage", PrimaryImage);
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
            cmd.Parameters.AddWithValue("@Language", Language);
            cmd.Parameters.AddWithValue("@Budget", Budget);
            cmd.Parameters.AddWithValue("@GrossWorldwide", GrossWorldwide);
            cmd.Parameters.AddWithValue("@Genres", Genres);
            cmd.Parameters.AddWithValue("@IsAdult", IsAdult);
            cmd.Parameters.AddWithValue("@RuntimeMinutes", RuntimeMinutes);
            cmd.Parameters.AddWithValue("@AverageRating", AverageRating);
            cmd.Parameters.AddWithValue("@NumVotes", NumVotes);
        }
        public static Movie FromReader(SqlDataReader reader)
        {
            return new Movie
            {
                Id = (int)reader["Id"],
                Url = reader["Url"].ToString(),
                PrimaryTitle = reader["PrimaryTitle"].ToString(),
                Description = reader["Description"].ToString(),
                PrimaryImage = reader["PrimaryImage"].ToString(),
                Year = (int)reader["Year"],
                ReleaseDate = (DateTime)reader["ReleaseDate"],
                Language = reader["Language"].ToString(),
                Budget = Convert.ToDouble(reader["Budget"]),
                GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                Genres = reader["Genres"].ToString(),
                IsAdult = (bool)reader["IsAdult"],
                RuntimeMinutes = (int)reader["RuntimeMinutes"],
                AverageRating = Convert.ToSingle(reader["AverageRating"]),
                priceToRent = (int)reader["priceToRent"],
                NumVotes = (int)reader["NumVotes"]
            };
        }




    }
}
