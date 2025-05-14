using System;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int FarmerId { get; set; }
        public Farmer? Farmer { get; set; }
        public int DiscussionPostId { get; set; }
        public DiscussionPost? DiscussionPost { get; set; }
    }
}