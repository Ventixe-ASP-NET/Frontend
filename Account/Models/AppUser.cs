namespace Account.Models;

public class AppUser
{
    public string Id { get; set; } = null!;
    public string? Role { get; set; }
    public AppUserAddress AppUserAddress { get; set; } = new();
    public AppUserProfile AppUserProfile { get; set; } = new();
}
