using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class DiscussionPost
    {
        public int Id { get; set; }

        [Required]
        public int FarmerId { get; set; }

        [Required, StringLength(200)]
        public string? Title { get; set; }

        [Required, StringLength(2000)]
        public string? Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public Farmer Farmer { get; set; }
    }
}