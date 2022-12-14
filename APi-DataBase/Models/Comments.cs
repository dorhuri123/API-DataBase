using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APi_DataBase.Models
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }
        [StringLength(15)]
        public string? User_Name { get; set; }
        public int Project_Id { get; set; }
        [StringLength(100)]
        public int Text { get; set; }
        public DateTime Time { get; set; }
    }
}
