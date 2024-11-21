using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Title { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public string? Address { get; set; }

        public ICollection<Notification>? Notifications { get; set; }

        public virtual ICollection<Participation>? Participations { get; set; }

        [ForeignKey("Community")]
        public int? CommunityId { get; set; }
    }
}

