using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("feedlikes")]
public class FeedLike
{
    public int Id { get; set; }

    public int Count { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public int FeedId { get; set; }

    public Feed? Feed {get; set;}

    public User? User { get; set; }
}