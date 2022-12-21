using Microsoft.AspNetCore.Mvc;
using APi_DataBase.Models;
using System.Data;
using MySqlConnector;

namespace APi_DataBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MySqlConnection _connection;

        public ProjectsController(MySqlConnection connection)
        {
            _connection = connection;
        }

        // GET: api/projects
        [HttpGet("startIndex")]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjects(int startIndex)
        {
            var projects = new List<Projects>();

            try
            {
                _connection.Open();

                var command = new MySqlCommand(@"
                    SELECT p.*, COUNT(l.Project_Id) AS Likes_Count,
                        (SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count
                    FROM Projects p
                    LEFT JOIN Likes l ON l.Project_Id = p.Id
                    GROUP BY p.Id
                    LIMIT @startIndex, 50", _connection);

                command.Parameters.AddWithValue("@startIndex", startIndex);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var project = new Projects
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Homepage_Url = reader.GetString("Homepage_Url"),
                            Licenses = reader.GetString("Licenses"),
                            Repository_Url = reader.GetString("Repository_Url"),
                            Language = reader.GetString("Language"),
                            Repository_Id = reader.GetInt32("Repository_Id"),
                            Likes_Count = reader.GetInt32("Likes_Count"),
                            Comments_Count = reader.GetInt32("Comments_Count")
                        };
                        projects.Add(project);
                    }
                }
                
                return Ok(projects);
            }
            catch (MySqlException)
            {
                return BadRequest();
            }
        }
    }
}
