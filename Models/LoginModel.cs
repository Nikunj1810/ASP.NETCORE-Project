
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace ASP.netcore_Project.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public bool ValidateUser(string email, string password)
        {
            try
            {
                using SqlConnection con = new();
                con.Open();
                using SqlCommand cmd = new("SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password", con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password); // Plain text password comparison

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0; // Returns true if user exists
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

    }
}
