namespace APi_DataBase.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public int Project_Id { get; set; }
        public string? Text { get; set; }
        public string? Time { get; set; }
    }
}
