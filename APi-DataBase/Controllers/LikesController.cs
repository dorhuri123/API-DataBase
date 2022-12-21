using APi_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace APi_DataBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly MySqlConnection _connection;

        public LikesController(MySqlConnection connection)
        {
            _connection = connection;
        }


        [HttpPost("ToggleLike")]
        public IActionResult ToggleLike([FromBody] Likes like)
        {
            try
            {
                _connection.Open();

                // Check if the like already exists
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM likes WHERE username=@username AND project_id=@projectId", _connection);
                cmd.Parameters.AddWithValue("@username", like.UserName);
                cmd.Parameters.AddWithValue("@projectId", like.Project_Id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    // Like already exists, so remove it
                    reader.Close();
                    cmd = new MySqlCommand("DELETE FROM likes WHERE username=@username AND project_id=@projectId", _connection);
                    cmd.Parameters.AddWithValue("@username", like.UserName);
                    cmd.Parameters.AddWithValue("@projectId", like.Project_Id);
                    cmd.ExecuteNonQuery();
                    return Ok("Like removed");
                }
                else
                {
                    // Like does not exist, so add it
                    reader.Close();
                    cmd = new MySqlCommand("INSERT INTO likes (username, project_id, time) VALUES (@username, @projectId, @time)", _connection);
                    cmd.Parameters.AddWithValue("@username", like.UserName);
                    cmd.Parameters.AddWithValue("@projectId", like.Project_Id);
                    cmd.Parameters.AddWithValue("@time", like.Time);
                    cmd.ExecuteNonQuery();
                    return Ok("Like added");
                }
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
