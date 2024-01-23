namespace API.DTOs;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
}

public class GroupListDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Status { get; set; }
    public required string UserId { get; set; }
    public List<string>? Files { get; set; }
}

public class JoinGroupDto
{
    public required string GroupAccesId { get; set; }
}