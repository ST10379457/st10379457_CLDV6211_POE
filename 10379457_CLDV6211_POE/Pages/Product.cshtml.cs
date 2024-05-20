using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _10379457_CLDV6211_POE.Pages
{
    public class ProductModel : PageModel
    {
        public List<ProductInfo> products = new List<ProductInfo>();
        public double TotalCost { get; set; }
        public string PurchaseMessage { get; set; }

        public void OnGet()
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                string connectionString = "Data Source=st10379457server.database.windows.net;User ID=st10379457;Password=admin$321;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM [dbo].[Product];";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductInfo productInfo = new ProductInfo();
                                productInfo.ProductID = reader.GetInt32(0);
                                productInfo.Product_Name = reader.GetString(2);
                                productInfo.Price = reader.GetDouble(3);
                                productInfo.Category = reader.GetString(4);
                                productInfo.Availability = reader.GetInt32(5);

                                products.Add(productInfo);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public IActionResult OnPostUpdateAvailability(int productId, int quantity)
        {
            // Find the product by productId
            var productToUpdate = products.Find(p => p.ProductID == productId);
            if (productToUpdate == null)
            {
                return RedirectToPage(); // Handle error: product not found
            }

            // Check if the requested quantity is available
            if (quantity > productToUpdate.Availability)
            {
                ModelState.AddModelError(string.Empty, "Not enough availability.");
                return Page(); // Return the same page with error message
            }

            // Update availability
            productToUpdate.Availability -= quantity;
            return RedirectToPage(); // Refresh the page
        }

        public IActionResult OnPostPurchase(string location)
        {
            // Calculate total cost based on selected products and quantities
            // This is just a basic example, you should implement your own logic
            TotalCost = CalculateTotalCost();

            // Save order to database (assuming you have Order table and appropriate logic)
            bool orderSaved = SaveOrder(location);

            if (orderSaved)
            {
                PurchaseMessage = "Purchase successful!";
                // Optionally clear any session or temporary data
                return RedirectToPage(); // Refresh the page
            }
            else
            {
                PurchaseMessage = "Purchase failed. Please try again later.";
                return Page(); // Return the same page with error message
            }
        }

        private double CalculateTotalCost()
        {
            double total = 0;
            foreach (var product in products)
            {
                // Assuming a simple total based on all products in the list
                total += product.Price * (product.Availability > 0 ? product.Availability : 0);
            }
            return total;
        }

        private bool SaveOrder(string location)
        {
            try
            {
                string connectionString = "Data Source=st10379457server.database.windows.net;User ID=st10379457;Password=admin$321;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Save order to database (assuming Order table structure)
                    string sql = "INSERT INTO [dbo].[Order] (UserID, Total_Cost, Date_Of_Purchase, Location) VALUES (@UserID, @TotalCost, @DateOfPurchase, @Location);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", 1); 
                        command.Parameters.AddWithValue("@TotalCost", TotalCost);
                        command.Parameters.AddWithValue("@DateOfPurchase", DateTime.Now);
                        command.Parameters.AddWithValue("@Location", location);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public class ProductInfo
        {
            public int ProductID { get; set; }
            public string Product_Name { get; set; }
            public double Price { get; set; }
            public string Category { get; set; }
            public int Availability { get; set; }
        }
    }
}
