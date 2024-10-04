using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models
{
    [Table("SongsPerformers")]
    public class SongPerformer
    {
        public int SongId { get; set; }
        [ForeignKey(nameof(SongId))]
        public Song Song { get; set; }

        public int PerformerId { get; set; }
        [ForeignKey(nameof(PerformerId))]
        public Performer Performer { get; set; }
    }
}
