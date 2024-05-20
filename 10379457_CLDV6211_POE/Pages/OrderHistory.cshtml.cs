using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _10379457_CLDV6211_POE.Pages
{
    public class OrderHistoryModel : PageModel
    {
        public List<OrderInfo> Orders { get; set; }

        public void OnGet()
        {
            Orders = new List<OrderInfo>();

            try
            {
                string connectionString = "Data Source=st10379457server.database.windows.net;User ID=st10379457;Password=admin$321;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM [dbo].[Order] WHERE UserID = @UserID;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", 1); 

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderInfo orderInfo = new OrderInfo();
                                orderInfo.OrderID = reader.GetInt32(0);
                                orderInfo.UserID = reader.GetInt32(1);
                                orderInfo.TotalCost = reader.GetDouble(2);
                                orderInfo.DateOfPurchase = reader.GetDateTime(3);
                                orderInfo.Location = reader.GetString(4);

                                Orders.Add(orderInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Handle exceptions as needed
            }
        }

        public class OrderInfo
        {
            public int OrderID { get; set; }
            public int UserID { get; set; }
            public double TotalCost { get; set; }
            public DateTime DateOfPurchase { get; set; }
            public string Location { get; set; }
        }
    }
}
