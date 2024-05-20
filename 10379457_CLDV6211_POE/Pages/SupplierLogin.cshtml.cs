using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace _10379457_CLDV6211_POE.Pages
{
    public class SupplierLoginModel : PageModel
    {
        [BindProperty]
        public string SupplierName { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string LoginMessage { get; set; }

        public void OnGet()
        {
           
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(SupplierName) || string.IsNullOrEmpty(Password))
            {
                LoginMessage = "Supplier name and password are required.";
                return Page(); // Return the same page with error message
            }

            try
            {
                string connectionString = "Data Source=myserver.database.windows.net;Initial Catalog=mydatabase;User ID=myusername;Password=mypassword;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Supplier WHERE Supplier_Name = @SupplierName AND Password = @Password";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierName", SupplierName);
                        command.Parameters.AddWithValue("@Password", Password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Supplier authenticated successfully
                                // Redirect to another page or perform further actions
                                return RedirectToPage("/Index"); 
                            }
                            else
                            {
                                // Supplier name or password incorrect
                                LoginMessage = "Invalid supplier name or password.";
                                return Page(); // Return the same page with error message
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                LoginMessage = "Error occurred while logging in. Please try again later.";
                Console.WriteLine(ex.Message);
                return Page(); // Return the same page with error message
            }
        }
    }
}
