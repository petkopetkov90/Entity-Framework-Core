using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    [Table("Countries")]
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        public virtual ICollection<Town> Towns { get; set; }

    }
}
