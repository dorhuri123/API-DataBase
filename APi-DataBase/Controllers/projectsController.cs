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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<int>>> GetProjectsCount()
        {
            var projects = new List<Projects>();

            try
            {
                //open connection
                _connection.Open();
                //query for number of projects in db
                var command = new MySqlCommand("SELECT COUNT(*) FROM Projects", _connection);
                //execute query
                var count = command.ExecuteScalar();

                return Ok(count);
            }
            catch (MySqlException)
            {
                return BadRequest();
            }
        }


        // GET: api/projects
        [HttpGet("{startIndex}")]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjects(int startIndex)
        {
            var projects = new List<Projects>();

            try
            {
                //open connection
                _connection.Open();
                /*
                    query for getting the first 50 project from start index
                    line sorted according creeated time stamp and adding for
                    each project number of likes and comments
                */
                var command = new MySqlCommand(@"
                    SELECT p.*,
                    (SELECT COUNT(*) FROM Likes WHERE Project_Id = p.Id) AS Likes_Count,
                    (SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count
                    FROM Projects p
                    ORDER BY p.created_timestamp DESC
                    LIMIT @startIndex, 50", _connection);
                //adding parameter to query
                command.Parameters.AddWithValue("@startIndex", startIndex);
                //executing query
                using (var reader = command.ExecuteReader())
                {
                    //while there is project to read
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
                        //adding project to project list
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

        // GET: api/projects
        [HttpGet("{numVersions}/{startIndex}")]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjectsVersions(int numVersions, int startIndex)
        {
            var projects = new List<Projects>();

            try
            {
                //open connection
                _connection.Open();
                /*
                    query for getting the first 50 project from start index
                    line sorted according creeated time stamp and adding for
                    each project number of likes and comments this for every 
                    project that have larger number of version then numVersions
                    and more forks then the average
                */
                var command = new MySqlCommand("SELECT p.*, " +
                    "(SELECT COUNT(*) FROM Likes WHERE Project_Id = p.Id) AS Likes_Count, " +
                    "(SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count " +
                    "FROM Projects p JOIN Repositories r " +
                    "ON p.Repository_Id = r.Id JOIN " +
                    "(SELECT Project_Id, COUNT(*) AS num_versions FROM Versions GROUP BY Project_Id) AS v " +
                    "ON p.Id = v.Project_Id " +
                    "WHERE v.num_versions > @numVersions AND r.Forks_count > (SELECT AVG(Forks_count) FROM Repositories) " +
                    "ORDER BY p.created_timestamp DESC " +
                    "LIMIT @startIndex,50", _connection);
                //adding query parameters
                command.Parameters.AddWithValue("@startIndex", startIndex);
                command.Parameters.AddWithValue("@numVersions", numVersions);
                //execute query
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
                        //adding project to project list
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
