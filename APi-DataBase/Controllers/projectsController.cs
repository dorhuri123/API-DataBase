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
        [HttpGet("project_id")]
        public async Task<ActionResult<ProjectInfo>> GetInfoProject(int project_id)
        {
            ProjectInfo projectInfo = new();
            try
            {
                _connection.Open();
                //setting prop project
                string query = ("SELECT * FROM projects WHERE id = @project_id");
                MySqlConnector.MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                IDataReader dataReader = await cmd.ExecuteReaderAsync();;
                projectInfo.project = Utils.Tools.GetList<Projects>(dataReader)[0];
                //setting prop repositories
                int repository_Id = projectInfo.project.Repository_Id;
                query = ("SELECT * FROM repositories WHERE id = @repository_Id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@repository_Id", repository_Id);
                dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.repositories = Utils.Tools.GetList<Repositories>(dataReader);
                //setting prop keywords
                query = ("SELECT * FROM keywords WHERE project_id = @project_id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.keywords = Utils.Tools.GetList<Keywords>(dataReader);
                //setting prop versions
                query = ("SELECT * FROM versions WHERE project_id = @project_id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.versions = Utils.Tools.GetList<Versions>(dataReader);
                //setting prop comments
                query = ("SELECT * FROM comments WHERE project_id = @project_id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.comments = Utils.Tools.GetList<Comments>(dataReader);
                //setting prop likes
                query = ("SELECT * FROM likes WHERE project_id = @project_id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.likes = Utils.Tools.GetList<Likes>(dataReader);
                return Ok(projectInfo);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }


    }
}
