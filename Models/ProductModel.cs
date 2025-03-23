using System.ComponentModel.DataAnnotations;

namespace ASP.netcore_Project.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }
    }
}
