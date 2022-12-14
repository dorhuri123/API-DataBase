namespace APi_DataBase.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public int Project_Id { get; set; }
        public int Text { get; set; }
        public String Time { get; set; }
    }
}
