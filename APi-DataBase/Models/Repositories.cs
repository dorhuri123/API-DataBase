
namespace APi_DataBase.Models
{
    public class Repositories
    {
        public int Id { get; set; }
        public string Host_Type { get; set; }
        public string Name_With_Owner { get; set; }
        public string Fork { get; set; }
        public String Created_Timestamp { get; set; }
        public String Last_Pushed_Timestamp { get; set; }
        public int Size { get; set; }
        public string Stars_count { get; set; }
        public string? Language { get; set; }
        public bool? Issues_Enabled { get; set; }
        public int Forks_count { get; set; }
        public int Open_Issues_Count { get; set; }
        public string? Uuid { get; set; }
    }
}
