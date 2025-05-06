using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public int FarmerId { get; set; }

        [Required, StringLength(100)]
        public string? Name { get; set; }

        [Required, StringLength(50)]
        public string? Category { get; set; }

        [Required, StringLength(500)]
        public string? Description { get; set; }

        [Required, Range(0.01, 100000)]
        public decimal Price { get; set; }

        public Farmer Farmer { get; set; }
    }
}