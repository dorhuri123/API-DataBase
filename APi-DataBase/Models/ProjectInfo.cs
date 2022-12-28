namespace APi_DataBase.Models
{
    public class ProjectInfo
    {
        public Projects? Project { get; set; }
        public List<Keywords>? Keywords { get; set; }
        public Repositories? Repository { get; set; }
        public List<Versions>? Versions { get; set; }
        public List<Comments>? Comments { get; set; }
    }
}
