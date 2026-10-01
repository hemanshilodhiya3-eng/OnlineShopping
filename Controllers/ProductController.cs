// Provides MVC controller functionality.
using Microsoft.AspNetCore.Mvc;

// Provides memory caching functionality.
using Microsoft.Extensions.Caching.Memory;

// Provides access to the Product model.
using OnlineShopping.Models;

// Provides JSON serialization and deserialization.
using System.Text.Json;

// Namespace for application controllers.
namespace OnlineShopping.Controllers
{
    // Controller responsible for product management.
    public class ProductController : Controller
    {
        // Stores the memory cache service.
        private readonly IMemoryCache _memoryCache;

        // Constructor receives the memory cache service.
        public ProductController(IMemoryCache memoryCache)
        {
            // Store the memory cache service.
            _memoryCache = memoryCache;
        }

        // Displays the complete product list.
        public async Task<IActionResult> Index()
        {
            // Define the name used for the memory cache.
            string cacheKey = "ProductList";

            // Try to get the product list from memory cache.
            if (!_memoryCache.TryGetValue(
                cacheKey,
                out List<Product>? products))
            {
                // Create an HTTP client for the API request.
                using HttpClient client = new HttpClient();

                // Request product data asynchronously from Fake Store API.
                HttpResponseMessage response =
                    await client.GetAsync(
                        "https://fakestoreapi.com/products");

                // Stop execution if the API request failed.
                response.EnsureSuccessStatusCode();

                // Read the API response as text.
                string json =
                    await response.Content.ReadAsStringAsync();

                // Convert JSON data into API product objects.
                List<ApiProduct>? apiProducts =
                    JsonSerializer.Deserialize<List<ApiProduct>>(json);

                // Create an empty product list.
                products = new List<Product>();

                // Check that API data was received.
                if (apiProducts != null)
                {
                    // Process every product received from the API.
                    foreach (ApiProduct item in apiProducts)
                    {
                        // Convert API product into our Product model.
                        products.Add(new Product
                        {
                            // Store product ID.
                            ProductId = item.id,

                            // Store product title.
                            ProductName = item.title,

                            // Store product category.
                            Category = item.category,

                            // Store product price.
                            Price = item.price,

                            // Apply a sample 10 percent discount.
                            DiscountPercentage = 10
                        });
                    }
                }

                // Create memory cache settings.
                MemoryCacheEntryOptions cacheOptions =
                    new MemoryCacheEntryOptions();

                // Set cache expiration to 10 minutes.
                cacheOptions.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10);

                // Store the product list in memory cache.
                _memoryCache.Set(
                    cacheKey,
                    products,
                    cacheOptions);
            }

            // Check whether the customer name cookie exists.
            if (Request.Cookies["CustomerName"] == null)
            {
                // Store the customer name in a cookie.
                Response.Cookies.Append(
                    "CustomerName",
                    "Student");
            }

            // Store the selected category in session.
            HttpContext.Session.SetString(
                "SelectedCategory",
                "All");

            // Send the product list to the Index view.
            return View(products);
        }

        // Displays details of a selected product.
        // The response can be cached for 60 seconds.
        [ResponseCache(
            Duration = 60,
            Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Details(int id)
        {
            // Create an HTTP client.
            using HttpClient client = new HttpClient();

            // Request the selected product from the API.
            HttpResponseMessage response =
                await client.GetAsync(
                    $"https://fakestoreapi.com/products/{id}");

            // Return a 404 response if the product does not exist.
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            // Read the API response.
            string json =
                await response.Content.ReadAsStringAsync();

            // Convert JSON into an API product object.
            ApiProduct? item =
                JsonSerializer.Deserialize<ApiProduct>(json);

            // Return 404 if the API did not return a product.
            if (item == null)
            {
                return NotFound();
            }

            // Create our Product model from API data.
            Product product = new Product
            {
                // Store product ID.
                ProductId = item.id,

                // Store product name.
                ProductName = item.title,

                // Store product category.
                Category = item.category,

                // Store product price.
                Price = item.price,

                // Apply a 10 percent discount.
                DiscountPercentage = 10
            };

            // Store the selected category in session.
            HttpContext.Session.SetString(
                "SelectedCategory",
                product.Category);

            // Read the customer name from the cookie.
            ViewBag.CustomerName =
                Request.Cookies["CustomerName"] ?? "Student";

            // Send the product to the Details view.
            return View(product);
        }

        // Receives the order form submission.
        [HttpPost]
        public IActionResult Order(int productId)
        {
            // Read the customer name from the cookie.
            ViewBag.CustomerName =
                Request.Cookies["CustomerName"] ?? "Student";

            // Read the selected category from the session.
            ViewBag.Category =
                HttpContext.Session.GetString(
                    "SelectedCategory") ?? "Unknown";

            // Receive ProductId from the hidden field.
            ViewBag.ProductId = productId;

            // Display the order confirmation page.
            return View();
        }
    }

    // Represents the data structure returned by Fake Store API.
    public class ApiProduct
    {
        // Stores the API product ID.
        public int id { get; set; }

        // Stores the API product title.
        public string title { get; set; } = string.Empty;

        // Stores the API product price.
        public decimal price { get; set; }

        // Stores the API product category.
        public string category { get; set; } = string.Empty;
    }
}