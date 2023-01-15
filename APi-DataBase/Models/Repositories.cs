
namespace APi_DataBase.Models
{
    public class Repositories
    {
        //all attributes are according to the db scheme
        public int Id { get; set; }
        public string? Host_Type { get; set; }
        public string? Name_With_Owner { get; set; }
        public int Size { get; set; }
        public int Stars_count { get; set; }
        public bool Issues_Enabled { get; set; }
        public int Forks_count { get; set; }
    }
}
