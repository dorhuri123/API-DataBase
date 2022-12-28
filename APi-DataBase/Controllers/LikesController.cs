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
        // GET: api/projects
        [HttpGet("{startIndex}, {username}")]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjectsUserDontLike(int startIndex, string username)
        {
            var projects = new List<Projects>();

            try
            {
                _connection.Open();
                var command = new MySqlCommand("SELECT p.* " +
                    "FROM projects p " +
                    "WHERE p.id NOT IN " +
                    "(" +
                    "SELECT l.project_id " +
                    "FROM likes l " +
                    "WHERE l.username = @username" +
                    ")  LIMIT @startIndex,50                                                                                                                                                                            ", _connection);
                command.Parameters.AddWithValue("@startIndex", startIndex);
                command.Parameters.AddWithValue("@username", username);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var project = new Projects
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Created_Timestamp = reader.GetDateTime("created_timestamp"),
                            Homepage_Url = reader.GetString("Homepage_Url"),
                            Repository_Url = reader.GetString("Repository_Url"),
                            Language = reader.GetString("Language"),
                            Repository_Id = reader.GetInt32("Repository_Id"),
                        };
                        projects.Add(project);
                    }
                }

                return Ok(projects);
            }
            catch (MySqlException e)
            {
                return BadRequest(e);
            }
        }

    }
}
