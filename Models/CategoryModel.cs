using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ASP.netcore_Project.Models
{
    public class CategoryModel
    {
        private readonly IMongoCollection<CategoryModel> _categories;

        public CategoryModel()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _categories = database.GetCollection<CategoryModel>("Categories");
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonIgnore]
        public List<string> Categories { get; set; } = new List<string>();

        // 🔹 Add Category
        public bool ADD(CategoryModel Cat)
        {
            try
            {
                var exists = _categories.Find(c => c.Category == Cat.Category).FirstOrDefault();
                if (exists == null)
                {
                    _categories.InsertOne(Cat);
                    return true;
                }
                return false; // already exists
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDB Insert Error: " + ex.Message);
                return false;
            }
        }

        // 🔹 Delete Category
        public bool DeleteCategory(string categoryName)
        {
            try
            {
                var result = _categories.DeleteOne(c => c.Category == categoryName);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDB Delete Error: " + ex.Message);
                return false;
            }
        }

        // 🔹 Get All Categories
        public List<string> GetAllCategories()
        {
            List<string> categoryList = new();
            try
            {
                var all = _categories.Find(_ => true).ToList();
                categoryList = all.Select(c => c.Category).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDB Fetch Error: " + ex.Message);
            }
            return categoryList;
        }
    }
}
