// This namespace contains the models used by the application.
namespace OnlineShopping.Models
{
    // This class represents a product in the online shopping system.
    public class Product
    {
        // Stores the unique ID of the product.
        public int ProductId { get; set; }

        // Stores the name of the product.
        public string ProductName { get; set; } = string.Empty;

        // Stores the category of the product.
        public string Category { get; set; } = string.Empty;

        // Stores the original price of the product.
        public decimal Price { get; set; }

        // Stores the discount percentage of the product.
        public decimal DiscountPercentage { get; set; }

        // Calculates and returns the price after applying the discount.
        public decimal GetDiscountedPrice()
        {
            // Calculate the discount amount.
            decimal discountAmount =
                Price * DiscountPercentage / 100;

            // Subtract the discount from the original price.
            decimal discountedPrice =
                Price - discountAmount;

            // Return the final selling price.
            return discountedPrice;
        }
    }
}