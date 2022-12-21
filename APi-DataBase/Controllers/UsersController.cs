﻿using APi_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace APi_DataBase.Controllers
{
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
                else
                {
                    reader.Close();
                    cmd = new MySqlCommand("INSERT INTO users (username, password, full_name) VALUES (@username, @password, @fullName)", _connection);
                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@fullName", user.Full_Name);
                    cmd.ExecuteNonQuery();
                    return Ok();
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
    }
}
