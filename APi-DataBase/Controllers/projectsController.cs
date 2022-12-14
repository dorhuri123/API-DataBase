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
                string query = ("SELECT * FROM projects WHERE id = @project_id;"
                    + "SELECT * FROM keywords WHERE project_id = @project_id;"
                    + "SELECT * FROM versions WHERE project_id = @project_id;"
                    + "SELECT * FROM comments WHERE project_id = @project_id;"
                    + "SELECT * FROM likes WHERE project_id = @project_id;");
                MySqlConnector.MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                int repository_Id;
                using (IDataReader dataReader = await cmd.ExecuteReaderAsync())
                {
                    projectInfo.project = Utils.Tools.GetList<Projects>(dataReader)[0];
                    dataReader.NextResult();
                    repository_Id = projectInfo.project.Repository_Id;
                    projectInfo.keywords = Utils.Tools.GetList<Keywords>(dataReader);
                    dataReader.NextResult();
                    projectInfo.versions = Utils.Tools.GetList<Versions>(dataReader);
                    dataReader.NextResult();
                    projectInfo.comments = Utils.Tools.GetList<Comments>(dataReader);
                    dataReader.NextResult();
                    projectInfo.likes = Utils.Tools.GetList<Likes>(dataReader);
                }
                _connection.Close();
                _connection.Open();
                query = ("SELECT * FROM repositories WHERE id = @repository_Id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@repository_Id", repository_Id);
                IDataReader _dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.repositories = Utils.Tools.GetList<Repositories>(_dataReader);
                return Ok(projectInfo);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }
        [HttpGet("like_project_id")]
        public async Task<ActionResult<ProjectInfo>> GetProjectLikes(int project_id)
        {
            try
            {
                _connection.Open();
                string query = ("SELECT * FROM likes WHERE project_id = @project_id");
                MySqlConnector.MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", project_id);
                IDataReader dataReader = await cmd.ExecuteReaderAsync();
                int num_likes = Utils.Tools.GetList<Likes>(dataReader).Count;
                return Ok(num_likes);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
