using API.DTOs;

namespace Api.DTOs;
public class ResultLike {
    public int Id { get; set; }

    public int Count { get; set; }

    public DateTime CreatedAt { get; set; }

    public FeedResultDto? Feed {get; set;}

    public ResultUserDto? User { get; set; }
}

public class ResultFeedLike {
    public int Id { get; set; }

    public int Count { get; set; }

    public DateTime CreatedAt { get; set; }
}