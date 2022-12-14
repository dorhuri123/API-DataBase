using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APi_DataBase.Models
{
    public class Versions
    {
       
        [Key]
        public int Id { get; set; }
        public int Project_Id { get; set; }
        public int Number { get; set; }
    }
}
