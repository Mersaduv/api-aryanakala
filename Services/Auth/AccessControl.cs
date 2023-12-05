using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class AccessControl
{
    public static async Task CheckProductPermissionFlag(HttpContext context, string permissionFlag)
    {
        var userId = Guid.Parse(context.User.Claims.FirstOrDefault(q => q.Type == "userId").Value);
        var permissionService = context.RequestServices.GetRequiredService<IPermissionService>();

        if (!await permissionService.CheckPermission(userId, permissionFlag))
        {
            context.Response.StatusCode = 401;
            _ = new BadRequestObjectResult("Not authorized to perform this action.");
        }
    }
}
