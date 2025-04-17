using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace ASP.netcore_Project.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IMongoCollection<CartModel> _cartCollection;
        private readonly IMongoCollection<ProductModel> _productCollection;
        private readonly IMongoCollection<OrderModel> _orderCollection;

        public CheckoutController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _cartCollection = database.GetCollection<CartModel>("Cart");
            _productCollection = database.GetCollection<ProductModel>("Products");
            _orderCollection = database.GetCollection<OrderModel>("Orders");
        }

        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();
            if (!cartItems.Any())
                return RedirectToAction("Cart", "Cart");

            return View(cartItems);
        }

        // POST: Checkout/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            try
            {
                // Get all cart items for the user
                var cartItems = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();

                if (!cartItems.Any())
                    return RedirectToAction("Cart", "Cart");

                // Validate shipping info
                var fullName = HttpContext.Request.Form["FullName"];
                var email = HttpContext.Request.Form["Email"];
                var phone = HttpContext.Request.Form["Phone"];
                var address = HttpContext.Request.Form["Address"];
                var city = HttpContext.Request.Form["City"];
                var state = HttpContext.Request.Form["State"];
                var zipCode = HttpContext.Request.Form["ZipCode"];

                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) ||
                    string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state) ||
                    string.IsNullOrEmpty(zipCode))
                {
                    TempData["ErrorMessage"] = "Please fill all required shipping information fields.";
                    return RedirectToAction("Index");
                }

                // Create order
                var order = new OrderModel
                {
                    UserId = userId,
                    Items = cartItems.Select(c => new OrderItemModel
                    {
                        ProductId = c.ProductId,
                        Name = c.ProductName,
                        Price = (double)c.Price,
                        Size = c.Size,
                        SizeType = c.SizeType,
                        Quantity = c.Quantity,
                        ImageUrl = c.ImageUrl
                    }).ToList(),
                    ShippingInfo = new ShippingInfo
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Address = address,
                        City = city,
                        State = state,
                        ZipCode = zipCode
                    },
                    PaymentMethod = HttpContext.Request.Form["PaymentMethod"],
                    Subtotal = (double)cartItems.Sum(c => c.Price * c.Quantity),
                    DeliveryFee = cartItems.Sum(c => c.Price * c.Quantity) < 500m ? 150.0 : 0.0,
                    OrderTotal = (double)(cartItems.Sum(c => c.Price * c.Quantity) + (cartItems.Sum(c => c.Price * c.Quantity) < 500m ? 150.0m : 0m)),
                    OrderDate = DateTime.Now,
                    Status = "pending"
                };

                await _orderCollection.InsertOneAsync(order);

                // Store the order ID in TempData
                TempData["OrderId"] = order.Id;

                // Clear cart
                await _cartCollection.DeleteManyAsync(c => c.UserId == userId);

                return RedirectToAction("OrderConfirmation", "Checkout");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error placing order: " + ex.Message);
                return RedirectToAction("Cart", "Cart");
            }
        }

        // GET: Checkout/OrderConfirmation
        public async Task<IActionResult> OrderConfirmation()
        {
            try
            {
                // Retrieve the order ID from TempData
                var orderId = TempData["OrderId"]?.ToString();

                // Pass the order ID to the view
                ViewBag.OrderId = orderId;

                if (!string.IsNullOrEmpty(orderId))
                {
                    // Retrieve the order details from the database
                    var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();

                    if (order != null)
                    {
                        // Send email notification
                        var smtpClient = new SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("quickcartservice118@gmail.com", "rpef whyp umig dcim"),
                            EnableSsl = true,
                        };

                        // Build a detailed HTML email body
                        var itemsHtml = "";
                        foreach (var item in order.Items)
                        {
                            itemsHtml += $"<tr>\n" +
                                       $"  <td>{item.Name}</td>\n" +
                                       $"  <td>{item.Size} {item.SizeType}</td>\n" +
                                       $"  <td>{item.Quantity}</td>\n" +
                                       $"  <td>₹{item.Price}</td>\n" +
                                       $"  <td>₹{item.Price * item.Quantity}</td>\n" +
                                       $"</tr>\n";
                        }

                        var emailBody = $@"
                            <html>
                            <head>
                                <style>
                                body {{
                                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                    background-color: #f9f9f9;
                                    margin: 0;
                                    padding: 0;
                                }}
                                .container {{
                                    max-width: 600px;
                                    margin: 40px auto;
                                    background-color: #ffffff;
                                    border-radius: 8px;
                                    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                                    padding: 30px;
                                }}
                                h1, h2 {{
                                    color: #333333;
                                    margin-bottom: 10px;
                                }}
                                p {{
                                    color: #555555;
                                    line-height: 1.6;
                                }}
                                table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    margin-top: 20px;
                                }}
                                th, td {{
                                    padding: 12px;
                                    text-align: left;
                                    border-bottom: 1px solid #e0e0e0;
                                }}
                                th {{
                                    background-color: #f0f0f0;
                                    color: #333333;
                                    font-weight: 600;
                                }}
                                .total-row td {{
                                    font-weight: bold;
                                    color: #000;
                                }}
                                .footer {{
                                    margin-top: 30px;
                                    font-size: 14px;
                                    color: #777;
                                    text-align: center;
                                }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                <h1>Order Confirmation</h1>
                                <p>Hi {order.ShippingInfo.FullName},</p>
                                <p>Thanks for shopping with <strong>QuickCart</strong>! Your order has been successfully received and is now being processed.</p>

                                <h2>Order Details</h2>
                                <p><strong>Order ID:</strong> {order.Id}</p>
                                <p><strong>Order Date:</strong> {order.OrderDate:MMMM dd, yyyy HH:mm}</p>
                                <p><strong>Payment Method:</strong> {order.PaymentMethod}</p>

                                <h2>Order Summary</h2>
                                <table>
                                    <tr>
                                    <th>Product</th>
                                    <th>Size</th>
                                    <th>Quantity</th>
                                    <th>Price</th>
                                    <th>Total</th>
                                    </tr>
                                    {itemsHtml}
                                </table>

                                <table>
                                    <tr class='total-row'>
                                    <td colspan='4'>Subtotal:</td>
                                    <td>₹{order.Subtotal}</td>
                                    </tr>
                                    <tr class='total-row'>
                                    <td colspan='4'>Delivery Fee:</td>
                                    <td>₹{order.DeliveryFee}</td>
                                    </tr>
                                    <tr class='total-row'>
                                    <td colspan='4'>Order Total:</td>
                                    <td>₹{order.OrderTotal}</td>
                                    </tr>
                                </table>

                                <h2>Shipping Information</h2>
                                <p><strong>Name:</strong> {order.ShippingInfo.FullName}</p>
                                <p><strong>Address:</strong> {order.ShippingInfo.Address}, {order.ShippingInfo.City}, {order.ShippingInfo.State}, {order.ShippingInfo.ZipCode}</p>
                                <p><strong>Phone:</strong> {order.ShippingInfo.Phone}</p>

                                <p>If you have any questions, feel free to contact us at <a href='mailto:quickcartservice118@gmail.com'>quickcartservice118@gmail.com</a>.</p>
                                <p>We appreciate your business!</p>

                                <div class='footer'>
                                    &copy; {DateTime.Now.Year} QuickCart. All rights reserved.
                                </div>
                                </div>
                            </body>
                            </html>";


                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("quickcartservice118@gmail.com"),
                            Subject = "QuickCart - Order Confirmation #" + order.Id,
                            Body = emailBody,
                            IsBodyHtml = true,
                        };
                        mailMessage.To.Add(order.ShippingInfo.Email);

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending order confirmation email: " + ex.Message);
                // Still show the confirmation page even if email fails
                return View();
            }
        }
    }
}