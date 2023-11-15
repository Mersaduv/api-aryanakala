namespace ApiAryanakala.Interfaces;

public interface IPermissionService
{
    Task<bool> CheckPermission(Guid userId, string permissionFlag);

}