var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // ✅ Enable session support

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ Serve static files (important for images, CSS, etc.)

app.UseRouting(); // ✅ Ensure routing is set up

app.UseSession(); // ✅ Enable session middleware
app.UseAuthorization();  // ✅ Only if needed (for role-based access)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
