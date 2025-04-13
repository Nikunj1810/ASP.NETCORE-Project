using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using BCrypt.Net;

namespace ASP.netcore_Project.Models
{
    public class AccountModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenCreatedAt { get; set; }
        public bool IsPasswordResetTokenUsed { get; set; }

        private readonly IMongoCollection<AccountModel> _users;

        public AccountModel()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _users = database.GetCollection<AccountModel>("Users");
        }

        public bool Insert(AccountModel user)
        {
            try
            {
                var exists = _users.Find(u => u.Email == user.Email).Any();
                if (exists)
                    return false;

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _users.InsertOne(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert Error: " + ex.Message);
                return false;
            }
        }

        // Updated Update method with password flag
        public bool Update(AccountModel user, bool isPasswordPlainText = true)
        {
            try
            {
                var filter = Builders<AccountModel>.Filter.Eq(u => u.Email, user.Email);
                string passwordToStore = isPasswordPlainText
                    ? BCrypt.Net.BCrypt.HashPassword(user.Password)
                    : user.Password;

                var update = Builders<AccountModel>.Update
                    .Set(u => u.FirstName, user.FirstName)
                    .Set(u => u.LastName, user.LastName)
                    .Set(u => u.PhoneNo, user.PhoneNo)
                    .Set(u => u.Address, user.Address)
                    .Set(u => u.Password, passwordToStore)
                    .Set(u => u.PasswordResetToken, user.PasswordResetToken)
                    .Set(u => u.PasswordResetTokenCreatedAt, user.PasswordResetTokenCreatedAt)
                    .Set(u => u.IsPasswordResetTokenUsed, user.IsPasswordResetTokenUsed);

                var result = _users.UpdateOne(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update Error: " + ex.Message);
                return false;
            }
        }

        public AccountModel? FindByEmail(string email)
        {
            try
            {
                return _users.Find(u => u.Email == email).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FindByEmail Error: " + ex.Message);
                return null;
            }
        }

        public AccountModel? Login(string email, string password)
        {
            try
            {
                var user = _users.Find(u => u.Email == email).FirstOrDefault();
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login Error: " + ex.Message);
            }

            return null;
        }

        public AccountModel? FindByToken(string token)
        {
            try
            {
                return _users.Find(u => u.PasswordResetToken == token).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FindByToken Error: " + ex.Message);
                return null;
            }
        }

        public bool ValidatePasswordResetToken(string token)
        {
            var user = FindByToken(token);
            if (user == null || user.IsPasswordResetTokenUsed)
                return false;

            if (user.PasswordResetTokenCreatedAt.HasValue &&
                (DateTime.UtcNow - user.PasswordResetTokenCreatedAt.Value).TotalHours > 1)
            {
                user.IsPasswordResetTokenUsed = true;
                Update(user, false);
                return false;
            }

            return true;
        }

        public void UsePasswordResetToken(string token)
        {
            var user = FindByToken(token);
            if (user != null && !user.IsPasswordResetTokenUsed)
            {
                user.IsPasswordResetTokenUsed = true;
                Update(user, false);
            }
        }

        // ✅ New method for resetting password
        public bool ResetPassword(string token, string newPassword)
        {
            var user = FindByToken(token);
            if (user == null || user.IsPasswordResetTokenUsed)
                return false;

            user.Password = newPassword; // plain password
            user.IsPasswordResetTokenUsed = true;
            return Update(user, isPasswordPlainText: true); // will hash it once
        }
    }
}
