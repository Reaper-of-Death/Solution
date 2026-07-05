using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RoguelikeDbConsole.Models
{
    [Table("characters")]
    public class Character
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

        [Column("is_unlocked")]
        public bool IsUnlocked { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<Run> Runs { get; set; } = new List<Run>();

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, HP: {Health}, DMG: {Damage}, Speed: {Speed}, Unlocked: {IsUnlocked}";
        }
    }
}