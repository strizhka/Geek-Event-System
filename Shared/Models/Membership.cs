using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class Membership
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MembershipId { get; set; }

        public bool IsConfirmed { get; set; }

        public int UserId { get; set; }

        public int CommunityId { get; set; }
    }
}

