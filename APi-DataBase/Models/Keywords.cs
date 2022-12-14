using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APi_DataBase.Models
{
    public class Keywords
    {
        [Key]
        public int Project_Id { get; set; }
        [StringLength(20)]
        public string? Keyword { get; set; }
    }
}
