using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;

using Microsoft.Data.SqlClient;

namespace ASP.netcore_Project.Models
{
    public class CategoryModel
    {
        [Required]
        public string Category { get; set; }

        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QuickCartDB;Integrated Security=True;";

        public bool ADD(CategoryModel Cat)
        {
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();

                using SqlCommand cmd = new("INSERT INTO Category (Category) VALUES (@Category)", con);
                cmd.Parameters.AddWithValue("@Category", Cat.Category);

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
