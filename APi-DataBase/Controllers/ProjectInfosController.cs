using APi_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace APi_DataBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectInfosController : ControllerBase
    {
        private readonly MySqlConnection _connection;

        public ProjectInfosController(MySqlConnection connection)
        {
            _connection = connection;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectInfo>> GetInfoProject(int id)
        {
            ProjectInfo projectInfo = new();
            try
            {
                _connection.Open();
                string query = (
                     "SELECT p.*, COUNT(l.Project_Id) AS Likes_Count, "
                    +"(SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count "
                    +"FROM Projects p LEFT JOIN Likes l ON l.Project_Id = p.Id "
                    +"WHERE p.id = @project_id "
                    +"GROUP BY p.Id;" 

                    + "SELECT * FROM keywords WHERE project_id = @project_id;"
                    + "SELECT * FROM versions WHERE project_id = @project_id;"
                    + "SELECT * FROM comments WHERE project_id = @project_id;"
                    );
                MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", id);
                int repository_Id;
                using (IDataReader dataReader = await cmd.ExecuteReaderAsync())
                {
                    projectInfo.Project = Utils.Tools.GetList<Projects>(dataReader)[0];
                    dataReader.NextResult();
                    repository_Id = projectInfo.Project.Repository_Id;
                    projectInfo.Keywords = Utils.Tools.GetList<Keywords>(dataReader);
                    dataReader.NextResult();
                    projectInfo.Versions = Utils.Tools.GetList<Versions>(dataReader);
                    dataReader.NextResult();
                    projectInfo.Comments = Utils.Tools.GetList<Comments>(dataReader);
                }
                _connection.Close();


                _connection.Open();
                query = ("SELECT * FROM repositories WHERE id = @repository_Id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@repository_Id", repository_Id);
                IDataReader _dataReader = await cmd.ExecuteReaderAsync();
                projectInfo.Repositories = Utils.Tools.GetList<Repositories>(_dataReader)[0];

                return Ok(projectInfo);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
