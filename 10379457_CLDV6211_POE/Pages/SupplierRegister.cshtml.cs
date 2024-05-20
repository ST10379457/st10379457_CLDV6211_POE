using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace _10379457_CLDV6211_POE.Pages
{
    public class SupplierRegisterModel : PageModel
    {
        [BindProperty]
        public string SupplierName { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string PhoneNumber { get; set; }

        public string RegisterMessage { get; set; }

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(SupplierName) || string.IsNullOrEmpty(Password))
            {
                RegisterMessage = "Supplier name and password are required.";
                return Page(); // Return the same page with error message
            }

            try
            {
                string connectionString = "Data Source=myserver.database.windows.net;Initial Catalog=mydatabase;User ID=myusername;Password=mypassword;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if supplier name already exists
                    string checkSupplierNameQuery = "SELECT COUNT(*) FROM Supplier WHERE Supplier_Name = @SupplierName";
                    using (SqlCommand checkSupplierNameCommand = new SqlCommand(checkSupplierNameQuery, connection))
                    {
                        checkSupplierNameCommand.Parameters.AddWithValue("@SupplierName", SupplierName);
                        int existingSupplierCount = (int)checkSupplierNameCommand.ExecuteScalar();

                        if (existingSupplierCount > 0)
                        {
                            RegisterMessage = "Supplier name already exists. Please choose a different name.";
                            return Page(); // Return the same page with error message
                        }
                    }

                    // Insert new supplier into database
                    string insertQuery = "INSERT INTO Supplier (Supplier_Name, Password, Email, Phone_Number) VALUES (@SupplierName, @Password, @Email, @PhoneNumber)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@SupplierName", SupplierName);
                        insertCommand.Parameters.AddWithValue("@Password", Password);
                        insertCommand.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                        insertCommand.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);

                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            RegisterMessage = "Registration successful!";
                            // Optionally, you can redirect to a login page or display a confirmation message.
                            return RedirectToPage("/SupplierLogin"); // Redirect to Supplier login page after registration
                        }
                        else
                        {
                            RegisterMessage = "Registration failed. Please try again later.";
                            return Page(); // Return the same page with error message
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                RegisterMessage = "Error occurred during registration. Please try again later.";
                Console.WriteLine(ex.Message);
                return Page(); // Return the same page with error message
            }
        }
    }
}
