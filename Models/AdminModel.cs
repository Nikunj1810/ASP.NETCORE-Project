using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace ASP.netcore_Project.Models
{
    public class AdminModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool ValidateAdmin(string username, string password)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QuickCartDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                string query = "SELECT Password FROM Admin WHERE UserName = @UserName";
                using (SqlCommand cmd = new(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string hashedPassword = result.ToString();
                        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    }
                }
            }
            return false;
        }
    }
}
