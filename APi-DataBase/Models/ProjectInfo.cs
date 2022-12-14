namespace APi_DataBase.Models
{
    public class ProjectInfo
    {
        public Projects project { get; set; }
        public List<Keywords> keywords { get; set; }
        public List<Repositories> repositories { get; set; }
        public List<Versions> versions { get; set; }
        public List<Comments> comments { get; set; }
        public List<Likes> likes { get; set; }
    }
}
