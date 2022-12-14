using Microsoft.AspNetCore.Mvc;
using APi_DataBase.Models;
using MySql.Data.MySqlClient;
using System.Data;
using MySqlConnector;

namespace APi_DataBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MySqlConnector.MySqlConnection _connection;

        public ProjectsController(MySqlConnector.MySqlConnection connection)
        {
            _connection = connection;
        }

        // GET: api/projects
        [HttpGet("start_index")]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjects(int start_index)
        {
            List<Projects> projects_list = new List<Projects>();
            try
            {
                _connection.Open();
                string query = "SELECT * FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY projects.id) as rn FROM projects) x WHERE rn > @start_index and rn <= @end_index";
                MySqlConnector.MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@start_index", start_index);
                cmd.Parameters.AddWithValue("@end_index", start_index + 50);

                IDataReader dataReader = await cmd.ExecuteReaderAsync();
                projects_list = Utils.Tools.GetList<Projects>(dataReader);
                return Ok(projects_list);
            }
            catch
            {
                return BadRequest();
            }
        }


        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Projects>> GetSpecificProject(int id)
        {
            List<Projects> projects_list = new();

            try
            {
                _connection.Open();
                string query = ("SELECT * FROM projects WHERE id = @id");
                MySqlConnector.MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);
                IDataReader dataReader = await cmd.ExecuteReaderAsync();
                projects_list = Utils.Tools.GetList<Projects>(dataReader);
                return Ok(projects_list[0]);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
