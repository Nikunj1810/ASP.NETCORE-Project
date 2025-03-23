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

        // 🔹 Validate User Login (Without Hashing)
       
        // 🔹 Update User Details
        public bool UpdateUser(AccountModel account)
        {
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("UPDATE Users SET FirstName = @FirstName, LastName = @LastName, PhoneNo = @PhoneNo, Address = @Address, Password = @Password WHERE Email = @Email", con);
                cmd.Parameters.AddWithValue("@FirstName", account.FirstName);
                cmd.Parameters.AddWithValue("@LastName", account.LastName);
                cmd.Parameters.AddWithValue("@PhoneNo", account.PhoneNo);
                cmd.Parameters.AddWithValue("@Address", account.Address);
                cmd.Parameters.AddWithValue("@Password", account.Password); // Updating password directly
                cmd.Parameters.AddWithValue("@Email", account.Email);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // 🔹 Get User by Email
        public AccountModel GetUserByEmail(string email)
        {
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("SELECT UserId, FirstName, LastName, PhoneNo, Address, Email FROM Users WHERE Email = @Email", con);
                cmd.Parameters.AddWithValue("@Email", email);

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new AccountModel
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        PhoneNo = reader["PhoneNo"].ToString(),
                        Address = reader["Address"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}
