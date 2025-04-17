    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using BCrypt.Net;

    namespace ASP.netcore_Project.Models
    {
        public class AdminModel
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string? Id { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            public string Password { get; set; }

            private readonly IMongoCollection<AdminModel> _admins;

            public AdminModel()
            {
                var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
                var database = client.GetDatabase("ASP_QuickCartDB");
                _admins = database.GetCollection<AdminModel>("Admins");

                // Call seed method to ensure default admin exists
            
            }

            public bool ValidateAdmin(string username, string password)
            {
                try
                {
                    var admin = _admins.Find(a => a.UserName == username).FirstOrDefault();
                    if (admin != null && BCrypt.Net.BCrypt.Verify(password, admin.Password))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MongoDB Error: " + ex.Message);
                }

                return false;
            }


        
       
        }
    }
