using System.ComponentModel.DataAnnotations;

namespace APi_DataBase.modals
{
    public class repositories
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string? Host_Type { get; set; }
        [StringLength(50)]
        public string? Name_With_Owner { get; set; }
        [StringLength(50)]
        public string? Fork { get; set; }
        public DateTime? Created_Timestamp { get; set; }
        public DateTime? Last_Pushed_Timestamp { get; set; }
        public int Size { get; set; }
        public string? Star_count { get; set; }
        [StringLength(15)]
        public string? Language { get; set; }
        public bool? Issues_Enabled { get; set; }
        public int Forks_count { get; set; }
        public int Open_Issues_Count { get; set; }
        [StringLength(15)]
        public string? Uuid { get; set; }
    }
}
