using System.ComponentModel.DataAnnotations;

namespace APi_DataBase.modals
{
    public class projects
    {
        [Key]
        public int Id { get; set; }
        [StringLength(15)]
        public string? Platform { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public string? Description { get; set; }
        [StringLength(15)]
        public string?  Licenses { get; set; }
        [StringLength(15)]
        public string? Homepage_Url { get; set; }
        [StringLength(100)]
        public string? Repository_Url { get; set; }
        [StringLength(15)]
        public string? Language { get; set; }
        public int Repository_Id { get; set; }
    }
}
