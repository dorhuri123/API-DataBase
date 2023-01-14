using APi_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

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

        [HttpGet("{username}/{startIndex}")]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjectsUserDontLike(string username, int startIndex)
        {
            var projects = new List<Projects>();

            try
            {
                //open the sql connection
                _connection.Open();
                /*
                    query for getting the first 50 project that the user
                    didnt like sorted according creeated time stamp
                */
                var command = new MySqlCommand("SELECT p.*, COUNT(l.Project_Id) AS Likes_Count, (SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count" +
                    " FROM Projects p LEFT JOIN Likes l " +
                    "ON l.Project_Id = p.Id WHERE p.id NOT IN " +
                    "(SELECT l.project_id FROM likes l WHERE l.username = @username ) " +
                    "GROUP BY p.Id " +
                    "ORDER BY p.created_timestamp DESC " +
                    "LIMIT @startIndex, 50", _connection);
                //adding query parameter
                command.Parameters.AddWithValue("@startIndex", startIndex);
                command.Parameters.AddWithValue("@username", username);
                //executing query
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //setting project with all is properties
                        var project = new Projects
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            Created_Timestamp = reader.GetDateTime("created_timestamp"),
                            Homepage_Url = reader.IsDBNull("Homepage_Url") ? null : reader.GetString("Homepage_Url"),
                            Repository_Url = reader.GetString("Repository_Url"),
                            Language = reader.IsDBNull("Language") ? null : reader.GetString("Language"),
                            Repository_Id = reader.GetInt32("Repository_Id"),
                            Likes_Count = reader.GetInt32("Likes_Count"),
                            Comments_Count = reader.GetInt32("Comments_Count")
                        };
                        //adding to projects list
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
