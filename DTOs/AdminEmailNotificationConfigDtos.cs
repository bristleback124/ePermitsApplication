namespace ePermitsApp.DTOs;

public class AdminEmailNotificationConfigResponseDto
{
    public List<AdminEmailNotificationConfigGroupDto> Configs { get; set; } = new();
    public List<AdminUserDto> AvailableAdmins { get; set; } = new();
}

public class AdminEmailNotificationConfigGroupDto
{
    public string ApplicationType { get; set; } = string.Empty;
    public string ApplicationTypeLabel { get; set; } = string.Empty;
    public List<int> UserIds { get; set; } = new();
}

public class AdminUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateAdminEmailNotificationConfigDto
{
    public string ApplicationType { get; set; } = string.Empty;
    public List<int> UserIds { get; set; } = new();
}
