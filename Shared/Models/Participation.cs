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

        public int UserId { get; set; }

        public int EventId { get; set; }
    }
}

