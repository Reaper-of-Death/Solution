using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RoguelikeDbConsole.Models
{
    [Table("items")]
    public class Item
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("item_type")]
        [MaxLength(50)]
        public string? ItemType { get; set; }

        [Column("damage_bonus")]
        [Precision(6, 2)]
        public decimal DamageBonus { get; set; }

        [Column("health_bonus")]
        [Precision(6, 2)]
        public decimal HealthBonus { get; set; }

        [Column("speed_bonus")]
        [Precision(4, 2)]
        public decimal SpeedBonus { get; set; }

        [Column("is_cursed")]
        public bool IsCursed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<RunItem> RunItems { get; set; } = new List<RunItem>();

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Type: {ItemType}, DMG: {DamageBonus:+0.00;-0.00}, HP: {HealthBonus:+0.00;-0.00}, Cursed: {IsCursed}";
        }
    }
}