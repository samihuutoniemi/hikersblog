namespace HikersBlog.Domain.Models;

public class ExternalUser
{
    public int Id { get; set; }
    public string Source { get; set; }
    public string ExternalId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string AccessToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; }
}
