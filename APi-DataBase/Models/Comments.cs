namespace APi_DataBase.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string? User_Name { get; set; }
        public int Project_Id { get; set; }
        public int Text { get; set; }
        public String Time { get; set; }
    }
}
