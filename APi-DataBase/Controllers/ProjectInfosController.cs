using APi_DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using ProjectInfo = APi_DataBase.Models.ProjectInfo;

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
                    + "(SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count "
                    + "FROM Projects p LEFT JOIN Likes l ON l.Project_Id = p.Id "
                    + "WHERE p.id = @project_id "
                    + "GROUP BY p.Id;"

                    + "SELECT * FROM versions WHERE project_id = @project_id;"
                    + "SELECT * FROM comments WHERE project_id = @project_id "
                    + "ORDER BY comments.time DESC;"
                    );
                MySqlCommand cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@project_id", id);
                int repository_Id;
                using (IDataReader dataReader = await cmd.ExecuteReaderAsync())
                {
                    projectInfo.Project = Utils.Tools.GetList<Projects>(dataReader)[0];
                    dataReader.NextResult();
                    repository_Id = projectInfo.Project.Repository_Id;
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
                projectInfo.Repository = Utils.Tools.GetList<Repositories>(_dataReader)[0];

                return Ok(projectInfo);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> InsertProject([FromBody] ProjectInfo projectInfo)
        {
            _connection.Open();

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Get the highest Repositories ID
                    var selectMaxRepositoryIdCommand = _connection.CreateCommand();
                    selectMaxRepositoryIdCommand.Transaction = transaction;
                    selectMaxRepositoryIdCommand.CommandText = "SELECT MAX(id) FROM repositories";
                    var maxRepositoryId = (int?)selectMaxRepositoryIdCommand.ExecuteScalar();

                    // Insert the Repositories record with an ID one higher than the highest existing ID
                    var insertRepositoriesCommand = _connection.CreateCommand();
                    insertRepositoriesCommand.Transaction = transaction;
                    insertRepositoriesCommand.CommandText = @"INSERT INTO repositories (id, host_type, name_with_owner, size, stars_count, issues_enabled, forks_count)
                                                        VALUES (@id, @host_type, @name_with_owner, @size, @stars_count, @issues_enabled, @forks_count)";
                    insertRepositoriesCommand.Parameters.AddWithValue("@id", maxRepositoryId + 1);
                    insertRepositoriesCommand.Parameters.AddWithValue("@host_type", projectInfo.Repository.Host_Type);
                    insertRepositoriesCommand.Parameters.AddWithValue("@name_with_owner", projectInfo.Repository.Name_With_Owner);
                    insertRepositoriesCommand.Parameters.AddWithValue("@size", projectInfo.Repository.Size);
                    insertRepositoriesCommand.Parameters.AddWithValue("@stars_count", projectInfo.Repository.Stars_count);
                    insertRepositoriesCommand.Parameters.AddWithValue("@issues_enabled", projectInfo.Repository.Issues_Enabled);
                    insertRepositoriesCommand.Parameters.AddWithValue("@forks_count", projectInfo.Repository.Forks_count);
                    insertRepositoriesCommand.ExecuteNonQuery();


                    // Set the ID of the inserted Repositories record to the ID one higher than the highest existing ID
                    var insertedRepositoryId = maxRepositoryId + 1;

                    // Get the highest Repositories ID
                    var selectMaxProjectIdCommand = _connection.CreateCommand();
                    selectMaxProjectIdCommand.Transaction = transaction;
                    selectMaxProjectIdCommand.CommandText = "SELECT MAX(id) FROM projects";
                    var maxProjectId = (int?)selectMaxProjectIdCommand.ExecuteScalar();
                    maxProjectId = maxProjectId + 1;

                    // Insert the Project records
                    var insertProjectCommand = _connection.CreateCommand();
                    insertProjectCommand.Transaction = transaction;
                    insertProjectCommand.CommandText = "INSERT INTO projects (id, name, created_timestamp, description, homepage_url, repository_url, language, repository_id) " +
                        "VALUES (@id, @name, @createdTimestamp, @description, @homepageUrl, @repositoryUrl, @language, @repositoryId)";
                    insertProjectCommand.Parameters.AddWithValue("@id", maxProjectId);
                    insertProjectCommand.Parameters.AddWithValue("@name", projectInfo.Project.Name);
                    insertProjectCommand.Parameters.AddWithValue("@createdTimestamp", projectInfo.Project.Created_Timestamp);
                    insertProjectCommand.Parameters.AddWithValue("@description", projectInfo.Project.Description);
                    insertProjectCommand.Parameters.AddWithValue("@homepageUrl", projectInfo.Project.Homepage_Url);
                    insertProjectCommand.Parameters.AddWithValue("@repositoryUrl", projectInfo.Project.Repository_Url);
                    insertProjectCommand.Parameters.AddWithValue("@language", projectInfo.Project.Language);
                    insertProjectCommand.Parameters.AddWithValue("@repositoryId", insertedRepositoryId);
                    insertProjectCommand.ExecuteNonQuery();

                    // Insert the Versions records
                    if (projectInfo.Versions != null)
                    {
                        foreach (var versions in projectInfo.Versions)
                        {
                            var insertVersionsCommand = _connection.CreateCommand();
                            insertVersionsCommand.Transaction = transaction;
                            insertVersionsCommand.CommandText = "INSERT INTO versions (project_id, number) VALUES (@project_id, @number)";
                            insertVersionsCommand.Parameters.AddWithValue("@project_id", maxProjectId);
                            insertVersionsCommand.Parameters.AddWithValue("@number", versions.Number);
                            insertVersionsCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (MySqlException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }

            }
            return Ok();
        }
     }
}
