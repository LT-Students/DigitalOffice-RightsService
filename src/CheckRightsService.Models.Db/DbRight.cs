using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.CheckRightsService.Models.Db
{
    public class DbRight
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<DbRightUser> RightUsers { get; set; }
    }
}