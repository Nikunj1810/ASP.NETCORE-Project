using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;

namespace ASP.netcore_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AccountModel AccountObj = new();

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password)
        {
            var user = AccountObj.Login(Email, Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);
                HttpContext.Session.SetString("UserLastName", user.LastName);
                HttpContext.Session.SetString("UserPhone", user.PhoneNo);
                HttpContext.Session.SetString("UserAddress", user.Address);

                TempData["LoginSuccess"] = "You have successfully logged in!";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Invalid email or password";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AccountModel user)
        {
            if (ModelState.IsValid)
            {
                bool res = AccountObj.Insert(user);
                if (res)
                {
                    TempData["msg"] = "Registration successful! Please login with your credentials.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["msg"] = "Email already exists. Please try again.";
                }
            }
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "Logout Successfully";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = AccountObj.FindByEmail(email);
            if (user != null)
            {
                var token = GeneratePasswordResetToken(user);
                await SendPasswordResetEmailAsync(user.Email, token);
                TempData["SuccessMessage"] = "Password reset email sent. Please check your inbox.";
            }
            else
            {
                TempData["ErrorMessage"] = "Email not found. Please try again.";
            }

            // Return the same view to show the message (no redirection)
            return View();
        }


        private string GeneratePasswordResetToken(AccountModel user)
        {
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetTokenCreatedAt = DateTime.UtcNow;
            user.IsPasswordResetTokenUsed = false;
            AccountObj.Update(user);
            return user.PasswordResetToken;
        }

        private async Task SendPasswordResetEmailAsync(string email, string token)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["SmtpPort"]),
                Credentials = new NetworkCredential(emailSettings["Email"], emailSettings["Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["Email"]),
                Subject = "Password Reset Request",
                Body = $"Please reset your password using this link: https://localhost:7170/Account/ResetPassword?token={token}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
        // Assuming the token is passed as a parameter
        public IActionResult ResetPassword(string token)
        {
            var accountModel = new AccountModel();
            var user = accountModel.FindByToken(token);

            if (user != null && accountModel.ValidatePasswordResetToken(token))
            {
                // Proceed with the password reset process
                return View(); // Don't pass user to view for security reasons
            }
            else
            {
                // Token not found or invalid
                TempData["ErrorMessage"] = "Invalid or expired token.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid token.";
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return View();
            }

            var accountModel = new AccountModel();
            var user = accountModel.FindByToken(token);

            if (user != null && accountModel.ValidatePasswordResetToken(token))
            {
                // Update password
                user.Password = newPassword; // The Update method will hash the password
                bool updated = accountModel.Update(user);

                if (updated)
                {
                    // Mark token as used
                    accountModel.UsePasswordResetToken(token);
                    
                    // Set success message and redirect to home
                    TempData["SuccessMessage"] = "Your password has been reset successfully. You can now login with your new password.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update password. Please try again.";
                    return View();
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid or expired token.";
                return RedirectToAction("Login");
            }
        }

    }
}
