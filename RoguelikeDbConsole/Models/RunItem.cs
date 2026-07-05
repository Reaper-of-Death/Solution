using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoguelikeDbConsole.Models
{
    [Table("run_items")]
    public class RunItem
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("run_id")]
        [Required]
        public Guid RunId { get; set; }

        [Column("item_id")]
        [Required]
        public Guid ItemId { get; set; }

        [Column("pickup_order")]
        public int? PickupOrder { get; set; }

        [Column("picked_up_at")]
        public DateTime PickedUpAt { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        // Навигационные свойства
        [ForeignKey("RunId")]
        public virtual Run? Run { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }
    }
}