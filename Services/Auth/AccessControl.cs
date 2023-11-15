using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class AccessControl
{
    public static async Task CheckProductAddPermission(HttpContext context)
    {
        var userId = Guid.Parse(context.User.Claims.FirstOrDefault(q => q.Type == "userId").Value);
        var permissionService = context.RequestServices.GetRequiredService<IPermissionService>();

        if (!await permissionService.CheckPermission(userId, "product-add"))
        {
            context.Response.StatusCode = 401;
            _ = new BadRequestObjectResult("Not authorized to perform this action.");
        }
    }
}
