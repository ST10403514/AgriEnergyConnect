using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class FundingOpportunity
    {
        public int Id { get; set; }
        [Required, StringLength(200)]
        public string Title { get; set; }
        [Required, StringLength(1000)]
        public string Description { get; set; }
        [Range(0, 20000000)] // Max R20M 
        public decimal Amount { get; set; } // Funding amount in ZAR
        [Required, StringLength(100)]
        public string Source { get; set; } // e.g., Government, NGO
        [StringLength(500)]
        public string ApplicationUrl { get; set; } // Link to apply
    }
}