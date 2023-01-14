namespace APi_DataBase.Models
{
    public class ProjectInfo
    {
        //attributes that needed for project info
        public Projects? Project { get; set; }
        public Repositories? Repository { get; set; }
        public List<Versions>? Versions { get; set; }
        public List<Comments>? Comments { get; set; }
    }
}
