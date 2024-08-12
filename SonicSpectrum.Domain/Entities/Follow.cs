namespace SonicSpectrum.Domain.Entities
{
    public class Follow
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FollowerId { get; set; } = null!;
        public string FolloweeId { get; set; } = null!;
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedDate { get; set; }
        public string RequestStatus { get; set; } = "Pending"; // "Pending", "Accepted", "Rejected"

        public virtual User Follower { get; set; } = null!;
        public virtual User Followee { get; set; } = null!;
    }
}
