using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        [Required, StringLength(200)]
        public string Title { get; set; }
        [Required, StringLength(2000)]
        public string Description { get; set; }
        [Required]
        public string Category { get; set; } // e.g., Solar, Biogas, Organic
        [Range(0, 20000000)]
        public decimal FundingGoal { get; set; } // Budget in ZAR
        public string? Status { get; set; } // Open, InProgress, Completed (nullable)
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Farmer Farmer { get; set; }
        public List<ProjectCollaborator> Collaborators { get; set; } = new();
    }
}