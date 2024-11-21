using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class Participation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParticipationId { get; set; }

        public bool IsConfirmed { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Связь с событием через EventId, без навигационного свойства
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public virtual Event? Event { get; set; }
    }
}

