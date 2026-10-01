// Create the ASP.NET Core application builder.
var builder = WebApplication.CreateBuilder(args);

// Add MVC controller and view services.
builder.Services.AddControllersWithViews();

// Add memory cache services.
builder.Services.AddMemoryCache();

// Add response caching services.
builder.Services.AddResponseCaching();

// Add session services.
builder.Services.AddSession(options =>
{
    // Keep session data for 30 minutes.
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Prevent JavaScript from accessing the session cookie.
    options.Cookie.HttpOnly = true;

    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

// Build the application.
var app = builder.Build();

// Configure error handling.
if (!app.Environment.IsDevelopment())
{
    // Use the default error handling page.
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security.
    app.UseHsts();
}

// HTTPS redirection is disabled because
// the practical is running on HTTP port 5002.
// app.UseHttpsRedirection();

// Enable CSS, JavaScript and image files.
app.UseStaticFiles();

// Enable response caching.
app.UseResponseCaching();

// Enable routing.
app.UseRouting();

// Enable session management.
app.UseSession();

// Enable authorization.
app.UseAuthorization();

// Configure the default MVC route.
// Product/Index will open when localhost:5002 is visited.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

// Start the application.
app.Run();