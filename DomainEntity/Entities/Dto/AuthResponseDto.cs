namespace DomainEntity.Entities.Dto;
public sealed class AuthResponseDto
{
    public string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? RefreshToken { get; set; }
}
