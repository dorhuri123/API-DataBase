using APi_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace APi_DataBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MySqlConnection _connection;

        public UsersController(MySqlConnection connection)
        {
            _connection = connection;
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] Users user)
        {
            try
            {
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username=@username", _connection);
                cmd.Parameters.AddWithValue("@username", user.UserName);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    return BadRequest("Username already exists.");
                }
                else if(user.UserName != "" && user.Password != "" && user.Full_Name != "")
                {
                    reader.Close();
                    cmd = new MySqlCommand("INSERT INTO users (username, password, full_name) VALUES (@username, @password, @fullName)", _connection);
                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@fullName", user.Full_Name);
                    cmd.ExecuteNonQuery();
                    return Ok();
                }
                else
                {
                    return BadRequest("Empty filled");
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("SignIn")]
        public IActionResult SignIn([FromBody] Users user)
        {
            try
            {
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username=@username AND password=@password", _connection);
                cmd.Parameters.AddWithValue("@username", user.UserName);
                cmd.Parameters.AddWithValue("@password", user.Password);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: api/GetUsersAboveAvg1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Projects>>> GetUsersAboveAvg()
        {
            var users = new List<Users>();

            try
            {
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT UserName " +
                    "FROM comments " +
                    "GROUP BY UserName " +
                    "HAVING COUNT(*) > (SELECT AVG(num_comments) " +
                    "FROM (SELECT UserName, COUNT(*) AS num_comments FROM comments GROUP BY UserName) AS temp)", _connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new Users
                        {
                            UserName = reader.GetString("UserName"),
                        };
                        users.Add(user);
                    }
                }

                return Ok(users);
            }

            catch
            {
                return BadRequest();
            }
        }
    }
}