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
            //creating new ProjectInfo
            ProjectInfo projectInfo = new();
            try
            {
                //open the sql connection
                _connection.Open();
                //query for getting the project info(versions,comments,Likes)
                string query = (
                     "SELECT p.*, " +
                     "(SELECT COUNT(*) FROM Likes WHERE Project_Id = p.Id) AS Likes_Count, " +
                     "(SELECT COUNT(*) FROM Comments WHERE Project_Id = p.Id) AS Comments_Count " +
                     "FROM Projects p " +
                     "WHERE p.id = @project_id; "

                    + "SELECT * FROM versions WHERE project_id = @project_id;"
                    + "SELECT * FROM comments WHERE project_id = @project_id "
                    + "ORDER BY comments.time DESC;"
                    );
                //making an sql commend with connection and the query
                MySqlCommand cmd = new(query, _connection);
                //adding query parameter
                cmd.Parameters.AddWithValue("@project_id", id);
                //repository_Id for the next query
                int repository_Id;
                //executing query
                using (IDataReader dataReader = await cmd.ExecuteReaderAsync())
                {
                    //setting the project
                    projectInfo.Project = Utils.Tools.GetList<Projects>(dataReader)[0];
                    dataReader.NextResult();
                    repository_Id = projectInfo.Project.Repository_Id;
                    //setting the Versions list
                    projectInfo.Versions = Utils.Tools.GetList<Versions>(dataReader);
                    dataReader.NextResult();
                    //setting the Comments
                    projectInfo.Comments = Utils.Tools.GetList<Comments>(dataReader);
                }
                //closing connection
                _connection.Close();

                //open connection
                _connection.Open();
                query = ("SELECT * FROM repositories WHERE id = @repository_Id");
                cmd = new(query, _connection);
                cmd.Parameters.AddWithValue("@repository_Id", repository_Id);
                IDataReader _dataReader = await cmd.ExecuteReaderAsync();
                //setting the repository
                projectInfo.Repository = Utils.Tools.GetList<Repositories>(_dataReader)[0];
                //returning ok and projectInfo 
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
                    //setting command transaction
                    insertRepositoriesCommand.Transaction = transaction;
                    //setting string of the query
                    insertRepositoriesCommand.CommandText = @"INSERT INTO repositories (id, host_type, name_with_owner, size, stars_count, issues_enabled, forks_count)
                                                        VALUES (@id, @host_type, @name_with_owner, @size, @stars_count, @issues_enabled, @forks_count)";
                    //adding the query parameter
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
                    //adding the query parameter
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
                            if(versions.Number != "")
                            {
                                var insertVersionsCommand = _connection.CreateCommand();
                                //setting command transaction
                                insertVersionsCommand.Transaction = transaction;
                                //setting the string of the query
                                insertVersionsCommand.CommandText = "INSERT INTO versions (project_id, number) VALUES (@project_id, @number)";
                                //adding the query parameter
                                insertVersionsCommand.Parameters.AddWithValue("@project_id", maxProjectId);
                                insertVersionsCommand.Parameters.AddWithValue("@number", versions.Number);
                                insertVersionsCommand.ExecuteNonQuery();
                            }
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
