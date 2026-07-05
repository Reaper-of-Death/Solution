using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RoguelikeDbConsole.Models
{
    [Table("enemies")]
    public class Enemy
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("health")]
        [Precision(8, 2)]
        public decimal Health { get; set; }

        [Column("damage")]
        [Precision(6, 2)]
        public decimal Damage { get; set; }

        [Column("speed")]
        [Precision(4, 2)]
        public decimal Speed { get; set; }

        [Column("experience_reward")]
        public int ExperienceReward { get; set; }

        [Column("is_boss")]
        public bool IsBoss { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, HP: {Health}, DMG: {Damage}, XP: {ExperienceReward}, Boss: {IsBoss}";
        }
    }
}