using System;
using System.Data.SqlClient;

namespace TestSqlConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"data source=HP-DISKTOP\TEST;initial catalog=Myclub;integrated security=True;encrypt=False;MultipleActiveResultSets=True;";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection successful.");
                    Console.ReadLine(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.ReadLine();

            }
        }
    }
}
