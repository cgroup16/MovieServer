using System.Data;
using System.Data.SqlClient;
using RuppinProj.BL;

namespace RuppinProj.DAL
{
    public class DBservicesUsers
    {
        public static string connectionString;

        static DBservicesUsers()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            connectionString = config.GetConnectionString("myProjDB");
        }

        private List<Users> ExecuteReaderAndBuildUsers(SqlCommand cmd)
        {
            List<Users> usersList = new List<Users>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                cmd.Connection = con;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    usersList.Add(Users.FromReader(reader));
                }
            }
            return usersList;
        }

        public List<Users> GetAllUsers()
        {
            SqlCommand cmd = new SqlCommand("GetAllUsersSP"); // קריאה ל-SP
            cmd.CommandType = CommandType.StoredProcedure;
            return ExecuteReaderAndBuildUsers(cmd);
        }


        public Users GetUserById(int id)
        {
            SqlCommand cmd = new SqlCommand("GetUserByIdSP");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            List<Users> result = ExecuteReaderAndBuildUsers(cmd);
            return result.FirstOrDefault();
        }

        public bool InsertUser(Users user)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("InsertUserSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    user.AddParameters(cmd);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected == 1;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000) // RAISERROR על אימייל קיים
                {
                    return false;
                }
                return false; // גם שגיאות אחרות תחזיר false ולא תזרוק
            }
        }

        public bool DeleteUser(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DeleteUserSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting user: " + ex.Message);
                return false;
            }
        }

        public bool UpdateUser(Users user)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateUserSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    user.AddParameters(cmd); // שימוש במתודה שמוסיפה פרמטרים

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating user: " + ex.Message);
                return false;
            }
        }
        public Users Login(string email, string password)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("LoginUserSP", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                List<Users> result = ExecuteReaderAndBuildUsers(cmd);
                return result.FirstOrDefault();
            }
        }

        public void ToggleUserActive(int userId, bool newStatus)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Users SET Active = @Active WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.Parameters.AddWithValue("@Active", newStatus);
                cmd.ExecuteNonQuery();
            }
        }



    }
}
