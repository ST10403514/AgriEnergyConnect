using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Required, StringLength(2000)]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; } // e.g., Crops, Equipment, Green Energy

        [Range(0, 10000000)]
        public decimal Price { get; set; } // Price in ZAR

        public string? ImageUrl { get; set; } // Optional image

        public DateTime ProductionDate { get; set; } = DateTime.UtcNow; // Renamed from CreatedDate to match existing

        public Farmer Farmer { get; set; }
        public List<ProductReview> Reviews { get; set; } = new();
    }
}