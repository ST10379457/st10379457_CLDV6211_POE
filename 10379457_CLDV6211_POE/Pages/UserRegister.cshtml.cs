using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace _10379457_CLDV6211_POE.Pages
{
    public class UserRegisterModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

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

        public void OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Email))
            {
                RegisterMessage = "Username, password, and email are required fields.";
                return;
            }

            try
            {
                string connectionString = "Data Source=myserver.database.windows.net;Initial Catalog=mydatabase;User ID=myusername;Password=mypassword;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if username already exists
                    string checkUsernameQuery = "SELECT COUNT(*) FROM [User] WHERE Username = @Username";
                    using (SqlCommand checkUsernameCommand = new SqlCommand(checkUsernameQuery, connection))
                    {
                        checkUsernameCommand.Parameters.AddWithValue("@Username", Username);
                        int existingUserCount = (int)checkUsernameCommand.ExecuteScalar();

                        if (existingUserCount > 0)
                        {
                            RegisterMessage = "Username already exists. Please choose a different username.";
                            return;
                        }
                    }

                    // Insert new user into database
                    string insertQuery = "INSERT INTO [User] (Username, Password, Email, Phone_Number) VALUES (@Username, @Password, @Email, @PhoneNumber)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", Username);
                        insertCommand.Parameters.AddWithValue("@Password", Password);
                        insertCommand.Parameters.AddWithValue("@Email", Email);
                        insertCommand.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);

                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            RegisterMessage = "Registration successful!";
                            
                        }
                        else
                        {
                            RegisterMessage = "Registration failed. Please try again later.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                RegisterMessage = "Error occurred during registration. Please try again later.";
                Console.WriteLine(ex.Message);
            }
        }
    }
}
