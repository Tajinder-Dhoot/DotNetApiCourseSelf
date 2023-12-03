namespace DotNetAPI.Dtos
{
    public partial class UserForRegistrationDto
    {
        string? Email { get; set; }
        string? Password { get; set; }
        string? PasswordConfirm {get; set;}
    }
}