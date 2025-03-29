    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using BCrypt.Net; // Import BCrypt library

namespace ASP.netcore_Project.Models
{
    public class AccountModel
    {
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNo { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // Store the hashed password

        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QuickCartDB;Integrated Security=True;";

        // 🔹 Insert New User (Register with Hashed Password)
        public bool Insert(AccountModel user)
        {
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();

                // Hash the password before storing
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                using SqlCommand cmd = new("INSERT INTO Users (FirstName, LastName, PhoneNo, Address, Email, Password) VALUES (@FirstName, @LastName, @PhoneNo, @Address, @Email, @Password)", con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);
                cmd.Parameters.AddWithValue("@Address", user.Address);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // 🔹 Login Method (Verify Hashed Password)
        public AccountModel Login(string email, string password)
        {
            AccountModel user = null;
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("SELECT * FROM Users WHERE Email = @Email", con);
                cmd.Parameters.AddWithValue("@Email", email);

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedHashedPassword = reader["Password"].ToString(); // Get the stored hashed password

                    // Verify the entered password with the stored hash
                    if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                    {
                        user = new AccountModel
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            PhoneNo = reader["PhoneNo"].ToString(),
                            Address = reader["Address"].ToString(),
                            Email = reader["Email"].ToString(),
                            Password = storedHashedPassword // Store hashed password (for security)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login Error: " + ex.Message);
            }
            return user; // Returns null if login fails
        }
    }
}
