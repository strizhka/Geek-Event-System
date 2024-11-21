﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fullstack_back.Models
{
    public class Membership
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MembershipId { get; set; }

        public bool IsConfirmed { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("Community")]
        public int? CommunityID { get; set; }
        public virtual Community? Community { get; set; }

    }
}