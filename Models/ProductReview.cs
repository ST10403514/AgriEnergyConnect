using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        public int FarmerId { get; set; }
        public Farmer? Farmer { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}