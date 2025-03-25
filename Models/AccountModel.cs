using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;

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
        public string Password { get; set; } // Stored as plain text (Not Recommended)

       private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QuickCartDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        // 🔹 Insert New User
        public bool Insert(AccountModel user)
        {
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("INSERT INTO Users (FirstName, LastName, PhoneNo, Address, Email, Password) VALUES (@FirstName, @LastName, @PhoneNo, @Address, @Email, @Password)", con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);
                cmd.Parameters.AddWithValue("@Address", user.Address);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password); // Password stored as plain text

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}