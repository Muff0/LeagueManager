﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enum;

namespace Data.Model
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string EmailAddress { get; set; } = String.Empty;
        public string DiscordHandle { get; set; } = String.Empty;
        public string OGSHandle { get; set; } = String.Empty;
        public string LeagoMemberId { get; set; } = String.Empty;
        public int GoMagicUserId { get; set; }
        public PlayerRank Rank { get; set; }
        public ICollection<PlayerSeason> PlayerSeasons { get; set; } = new List<PlayerSeason>();
        public string LeagoKey { get; set; } = string.Empty;
        public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ulong? DiscordId { get; set; }
    }
}