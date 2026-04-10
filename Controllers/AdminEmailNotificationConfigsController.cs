using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers;

[ApiController]
[Route("api/admin-email-notification-configs")]
[Authorize(Roles = "admin,superadmin,sysadmin")]
public class AdminEmailNotificationConfigsController : ControllerBase
{
    private readonly IAdminEmailNotificationConfigService _service;

    public AdminEmailNotificationConfigsController(IAdminEmailNotificationConfigService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetConfig()
    {
        var result = await _service.GetConfigAsync();
        return Ok(new { success = true, message = "Config retrieved", data = result });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateAdminEmailNotificationConfigDto dto)
    {
        try
        {
            await _service.UpdateConfigAsync(dto);
            return Ok(new { success = true, message = "Config updated successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
