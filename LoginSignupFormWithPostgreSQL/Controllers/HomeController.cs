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
            return RedirectToAction("LogIn");
        }

        public IActionResult LogIn(User user)
        {
            int userCount =  UserExist(user);
            if (userCount != 1)
            {
                ViewBag.message = "Please Enter Valid Credential";
                Console.WriteLine("Please Enter Valid Credential");
            }
            else
            {
                ViewBag.message = "Login Successful";
                Console.WriteLine("Login Successful");
            }
            Console.WriteLine("Hey Here "+userCount+ "User available");
            return View();
        }

        public IActionResult SignUp(User user)
        {
            InsertUser(user);
            return View();
        }

        private void InsertUser(User user)
        {
            int userCount = CheckUserAlreadyAvailableOrNot(user);
            if (userCount >= 1) {
                ViewBag.message = "User Already avilable";
                Console.WriteLine("User Already avilable");
                return;
            }
            else if (user.RePassword != user.Password) {
                ViewBag.message = "Password are not same";
                Console.WriteLine("Password are not same");
                return;
            }
            else
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
                            RedirectToAction("LogIn");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                        }
                    }
                }
            }
        }

        private int CheckUserAlreadyAvailableOrNot(User user)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM users WHERE email= @email";
                    command.Parameters.Add(new NpgsqlParameter("@email", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters["@email"].Value = user.EMail;
                    try
                    {
                        int no = Convert.ToInt32(command.ExecuteScalar());
                        Console.WriteLine("HELLO \nNumber of user  = " + command.ExecuteScalar());
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
                        int no = Convert.ToInt32(command.ExecuteScalar());
                        Console.WriteLine("HELLO \nNumber of user  = " + command.ExecuteScalar());
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
