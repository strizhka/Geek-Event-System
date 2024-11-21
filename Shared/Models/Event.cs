using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fullstack_back.Models
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
        public DateTime Date { get; set; }

        [Required]
        public string? Adress { get; set; }

        public virtual ICollection<Notification>? Notification { get; set; }

        [ForeignKey("Community")]
        public int? CommunityID { get; set; }
        public virtual Community? Community { get; set; }
    }
}
