using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class DiscussionPost
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int FarmerId { get; set; }
        public Farmer? Farmer { get; set; }
        public List<Comment> Comments { get; set; } = new();
    }
}