using Microsoft.AspNetCore.Mvc;
using RuppinProj;
using RuppinProj.BL;


namespace RuppinProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DBservices db = new DBservices();

        // 1. הוספת סרט (Insert)

        [HttpPost]
        public IActionResult Post([FromBody] Movie movie)
        {
            bool success = db.InsertMovie(movie);
            if (success)
                return Ok("Movie Added successfully.");
            else
                return BadRequest("Failed to insert movie");
        }

        [HttpPost("BulkInsert")]
        public IActionResult BulkInsert([FromBody] List<Movie> movies)
        {
            try
            {
                DBservices db = new DBservices();
                bool success = db.InsertMoviesBulk(movies);
                if (success)
                    return Ok();
                else
                    return BadRequest("Failed to insert movies.");
            }
            catch (Exception ex)
            {
                return BadRequest("Server error: " + ex.Message);
            }
        }



        // 2. שליפת כל הסרטים (GetAllMovies)

        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            return db.GetAllMovies();
        }
        // 6. שליפת סרט בודד לפי ID

        [HttpGet("{id}")]
        public ActionResult<Movie> Get(int id)
        {
            Movie movie = db.GetMovieById(id);
            if (movie == null)
                return NotFound("Movie not found");
            return Ok(movie);
        }


        // 3. שליפת סרטים לפי טייטל (GetByTitle) דרך Query String

        [HttpGet("ByTitle")]
        public IEnumerable<Movie> GetByTitle([FromQuery] string title)
        {
            return db.GetByTitle(title);
        }

        // 4. שליפת סרטים לפי טווח תאריכים (GetByReleaseDate) דרך Path

        [HttpGet("from/{startDate}/until/{endDate}")]
        public IEnumerable<Movie> GetByReleaseDate(DateTime startDate, DateTime endDate)
        {
            return db.GetByReleaseDate(startDate, endDate);
        }



        // 5. מחיקת סרט לפי ID (DeleteMovie) דרך Path
        // למשל: https://localhost:7275/api/Movies/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = db.DeleteMovie(id);
            if (success)
                return Ok("Movie deleted successfully.");
            else
                return NotFound("Movie not found");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, [FromBody] Movie updatedMovie)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid ID.");

                DBservices db = new DBservices();
                bool success = db.UpdateMovie(id, updatedMovie);

                if (!success)
                    return NotFound($"Movie with ID {id} not found.");

                return Ok($"Movie with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("RentMovie")]
        public IActionResult RentMovie([FromBody] RentedMovie rental)
        {
            try
            {
                db.RentMovie(rental.UserId, rental.MovieId, rental.RentStart, rental.RentEnd);
                return Ok("Movie rented successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error renting movie: " + ex.Message);
            }
        }

        [HttpGet("RentedByUser/{userId}")]
        public IActionResult GetRentedMoviesByUser(int userId)
        {
            List<Movie> rentals = db.GetRentedMoviesByUser(userId);

            if (rentals == null || rentals.Count == 0)
                return NotFound("No rented movies found");

            return Ok(rentals);
        }



        [HttpDelete("CancelRental")]
        public IActionResult CancelRental(
       [FromQuery] int userId,
       [FromQuery] int movieId)
        {
            bool success = db.CancelRental(userId, movieId);
            if (!success)
                return NotFound("Rental not found or already deleted");

            return Ok("Rental cancelled");
        }
        [HttpPut("TransferRental")]
        public IActionResult TransferRental(
            [FromQuery] int fromUserId,
            [FromQuery] int toUserId,
            [FromQuery] int movieId)
        {
            try
            {
                bool success = db.TransferRental(fromUserId, toUserId, movieId);

                if (!success)
                    return NotFound("Transfer failed.");

                return Ok("Rental transferred.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // שולח את ההודעה ללקוח
            }
        }

    }
}
