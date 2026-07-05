using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RoguelikeDbConsole.Models
{
    [Table("runs")]
    public class Run
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("character_id")]
        [Required]
        public Guid CharacterId { get; set; }

        [Column("run_seed")]
        [MaxLength(50)]
        public string? RunSeed { get; set; }

        [Column("floor_number")]
        public int FloorNumber { get; set; }

        [Column("score")]
        [Precision(10, 2)]
        public decimal Score { get; set; }

        [Column("is_completed")]
        public bool IsCompleted { get; set; }

        [Column("started_at")]
        public DateTime StartedAt { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("total_time_seconds")]
        public int TotalTimeSeconds { get; set; }

        // Навигационные свойства
        [ForeignKey("CharacterId")]
        public virtual Character? Character { get; set; }

        public virtual ICollection<RunItem> RunItems { get; set; } = new List<RunItem>();

        public override string ToString()
        {
            return $"ID: {Id}, Seed: {RunSeed}, Floor: {FloorNumber}, Score: {Score}, Completed: {IsCompleted}";
        }
    }
}