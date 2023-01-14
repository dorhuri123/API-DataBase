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
                //open connection
                _connection.Open();
                //for checking if username already exist in db
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username=@username", _connection);
                //adding the username parameter
                cmd.Parameters.AddWithValue("@username", user.UserName);
                //executing query
                MySqlDataReader reader = cmd.ExecuteReader();
                //if username exisxt in db return bad requst
                if (reader.HasRows)
                {
                    return BadRequest("Username already exists.");
                }
                //for validations of signup not getting empty string in registration field
                else if(user.UserName != "" && user.Password != "" && user.Full_Name != "")
                {
                    reader.Close();
                    //insert the username with is full name and password to the db
                    cmd = new MySqlCommand("INSERT INTO users (username, password, full_name) VALUES (@username, @password, @fullName)", _connection);
                    //adding query parameters
                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@fullName", user.Full_Name);
                    cmd.ExecuteNonQuery();
                    return Ok();
                }
                //if filed was not filled correctly
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
                //query chcking if the username and password exist together in the db
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username=@username AND password=@password", _connection);
                cmd.Parameters.AddWithValue("@username", user.UserName);
                cmd.Parameters.AddWithValue("@password", user.Password);
                //executing query
                MySqlDataReader reader = cmd.ExecuteReader();
                //if exist in db return ok
                if (reader.HasRows)
                {
                    return Ok();
                }
                //if not exist in db return Unauthorized
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

        // GET: api/GetUsersAboveAvg
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Projects>>> GetUsersAboveAvg()
        {
            var users = new List<Users>();

            try
            {
                //open connection
                _connection.Open();
                //query for getting the users who number of there comment is above average
                MySqlCommand cmd = new MySqlCommand("SELECT UserName FROM comments" +
                "GROUP BY UserName HAVING COUNT(*) > (SELECT AVG(num_comments)" +
                "FROM (SELECT UserName, COUNT(*) AS num_comments" +
                "FROM comments GROUP BY UserName) AS temp)", _connection);
                //execute query
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new Users
                        {
                            //setting user
                            UserName = reader.GetString("UserName"),
                        };
                        //adding to user list
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