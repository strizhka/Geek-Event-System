using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fullstack_back.Models
{
    public class Participation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParticipationId { get; set; }

        public bool IsConfirmed { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }

    }
}
