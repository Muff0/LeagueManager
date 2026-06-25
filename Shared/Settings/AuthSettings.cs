namespace Shared.Settings;

public class AuthSettings
{
    public List<AdminCredential> Admins { get; set; } = [];
}

public class AdminCredential
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}