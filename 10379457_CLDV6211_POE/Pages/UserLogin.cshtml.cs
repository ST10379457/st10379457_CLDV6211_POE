using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace _10379457_CLDV6211_POE.Pages
{
    public class UserLoginModel : PageModel
    {
        public string LoginMessage { get; set; }

        public void OnGet()
        {
            
        }

        public void OnPost(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LoginMessage = "Please enter both username and password.";
                return;
            }

            try
            {
                string connectionString = "Data Source=myserver.database.windows.net;Initial Catalog=mydatabase;User ID=myusername;Password=mypassword;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM [User] WHERE Username = @Username AND Password = @Password";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User authenticated successfully
                                LoginMessage = "Login successful!";
                            }
                            else
                            {
                                // Username or password incorrect
                                LoginMessage = "Invalid username or password.";
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
            }
        }
    }
}
