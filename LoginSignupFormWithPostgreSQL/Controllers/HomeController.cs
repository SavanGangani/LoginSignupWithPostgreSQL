using LoginSignupFormWithPostgreSQL.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;

namespace LoginSignupFormWithPostgreSQL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private const string ConnectionString = "Host=localhost;Database=LoginSignupSample;Username=postgres;Password=savan";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LogIn(User user)
        {
            int userCount =  UserExist(user);
            Console.WriteLine("Hey Here "+userCount+ "User avalable");
            return View();
        }

        private int UserExist(User user)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM users WHERE email= @email AND password = @password";
                    command.Parameters.Add(new NpgsqlParameter("@email", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters["@email"].Value = user.EMail;
                    command.Parameters.Add(new NpgsqlParameter("@password", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters["@password"].Value = user.Password;
                    try
                    {
                        //command.ExecuteNonQuery();
                        int no = Convert.ToInt32(command.ExecuteScalar());
                        Console.WriteLine("HELLO \nNumber of user  = "+ command.ExecuteScalar());
                        Console.WriteLine(no);
                        return no;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return 0;
        }


        public IActionResult SignUp(User user)
        {
            InsertUser(user);
            return View();
        }

        private void InsertUser(User user)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO users (email, password) VALUES (@email, @password)";
                    command.Parameters.AddWithValue("@email", user.EMail);
                    command.Parameters.AddWithValue("@password", user.Password);
                    try
                    {
                        command.ExecuteNonQuery();
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
