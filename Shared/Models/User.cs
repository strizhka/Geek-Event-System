using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fullstack_back.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string? UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? HashedPassword { get; set; }
        public string? UserRole { get; set; }
        public virtual ICollection<Membership>? Memberships { get; set; }
        public virtual ICollection<Participation>? Participations { get; set; }
    }
}
