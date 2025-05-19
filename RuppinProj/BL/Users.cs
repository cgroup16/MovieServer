using System.Data.SqlClient;
using System.Linq;

namespace RuppinProj.BL
{

    public class Users
    {
        int id;
        string name;
        string email;
        string password;
        bool active;

        public Users(int id, string name, string email, string password, bool active)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            this.password = password;
            this.active = active;
        }

        public Users() { }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public bool Active { get => active; set => active = value; }
        public static Users FromReader(SqlDataReader reader)
        {
            return new Users
            {
                Id = (int)reader["Id"],
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                Active = reader["Active"] != DBNull.Value ? (bool)reader["Active"] : true
            };
        }

        public void AddParameters(SqlCommand cmd)
        {
            if (this.Id > 0) // רק אם יש Id קיים, נשלח אותו
            {
                cmd.Parameters.AddWithValue("@Id", this.Id);
            }
            cmd.Parameters.AddWithValue("@Name", this.Name);
            cmd.Parameters.AddWithValue("@Email", this.Email);
            cmd.Parameters.AddWithValue("@Password", this.Password);
            cmd.Parameters.AddWithValue("@Active", this.Active);
        }



    }
}
