using Microsoft.AspNetCore.Mvc;
using RuppinProj.BL;
using RuppinProj.DAL;
using System.Data.SqlClient;

namespace RuppinProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DBservicesUsers db = new DBservicesUsers();

        // GET: api/Users
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            return db.GetAllUsers();
        }

        // GET api/Users/ById/5
        [HttpGet("ById/{id}")]
        public ActionResult<Users> GetById(int id)
        {
            Users user = db.GetUserById(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Users user)
        {
            bool success = db.InsertUser(user);
            if (success)
                return Ok();
            else
                return Conflict("Email already exists");
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] Users loginUser)
        {
            Users result = db.Login(loginUser.Email, loginUser.Password);

            if (result == null)
                return Unauthorized("Invalid email or password");

            if (!result.Active)
                return Unauthorized("User is not active");

            return Ok(result);
        }




        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = db.DeleteUser(id);
            if (success)
                return Ok("User deleted successfully.");
            else
                return NotFound("User not found or already deleted.");
        }

        [HttpPut]
        public IActionResult Update([FromBody] Users user)
        {
            bool success = db.UpdateUser(user);
            if (success)
                return Ok("User updated successfully.");
            else
                return NotFound("User not found or update failed.");
        }

        [HttpPut("ToggleActive")]
        public IActionResult ToggleActive([FromBody] Users user)
        {
            try
            {
                db.ToggleUserActive(user.Id, user.Active);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
