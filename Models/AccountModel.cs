using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
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

        // 🔹 Insert New User (Register)
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
                cmd.Parameters.AddWithValue("@Password", user.Password); // Plain text password

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // 🔹 Login Method
        public AccountModel Login(string email, string password)
        {
            AccountModel user = null;
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("SELECT * FROM Users WHERE Email = @Email AND Password = @Password", con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password); // Plain text comparison

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new AccountModel
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        PhoneNo = reader["PhoneNo"].ToString(),
                        Address = reader["Address"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString() // Retrieve password (Avoid in real-world apps)
                    };
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
