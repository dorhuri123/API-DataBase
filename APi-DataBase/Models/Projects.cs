namespace APi_DataBase.Models
{
    public class Projects
    {
        public int Id { get; set; }
        public string? Platform { get; set; }
        public string? Name { get; set; }
        public DateTime? Created_Timestamp { get; set; }
        public string? Description { get; set; }
        public string? Homepage_Url { get; set; }
        public string? Repository_Url { get; set; }
        public string? Language { get; set; }
        public int Repository_Id { get; set; }
        public int? Likes_Count { get; set; }
        public int? Comments_Count { get; set; }
    }
}
