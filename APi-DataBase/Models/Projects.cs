namespace APi_DataBase.Models
{
    public class Projects
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public string Name { get; set; }
        public string Created_Timestamp { get; set; }
        public string Updated_Timestamp { get; set; }
        public string Description { get; set; }
        public string Homepage_Url { get; set; }
        public string Licenses { get; set; }
        public string Repository_Url { get; set; }
        public string Language { get; set; }
        public int Repository_Id { get; set; }
    }
}
