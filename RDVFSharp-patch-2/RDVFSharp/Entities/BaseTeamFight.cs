﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RDVFSharp.Entities
{
    public class BaseTeamFight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long TeamFightId { get; set; }

        [Required]
        public string Room { get; set; }
        [Required]
        public string Winner1Id { get; set; }
        [Required]
        public string Winner2Id { get; set; }
        [Required]
        public string Loser1Id { get; set; }
        [Required]
        public string Loser2Id { get; set; }
        [Required]
        public DateTime FinishDate { get; set; }

        [ForeignKey(nameof(Winner1Id))]
        public BaseFighter Winner { get; set; }
        [ForeignKey(nameof(Loser1Id))]
        public BaseFighter Loser { get; set; }
    }
}
