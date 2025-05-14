namespace AgriEnergyConnect.Models
{
    public class ProjectCollaborator
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int FarmerId { get; set; }
        public Farmer Farmer { get; set; }
    }
}