namespace SearchService.Api.Authentication;

public static class PermissionNames
{
    public const string DispatchesCreate = "dispatches:create";
    public const string DispatchesRead = "dispatches:read";
    public const string DispatchesReadAll = "sync:read-all";
    public const string DispatchesSyncJobUpdate = "sync:update-all";
    public const string DispatchesUpdate = "dispatches:update";
    public const string DispatchesDelete = "dispatches:delete";
    public const string UsersCreate = "users:create";
    public const string UsersRead = "users:read";
    public const string UsersUpdate = "users:update";
    public const string UsersDelete = "users:delete";
    public const string DriversRead = "drivers:read";
    public const string CompaniesRead = "companies:read";
    public const string CompaniesUpdate = "companies:update";

    public static readonly string[] All =
    [
     DispatchesCreate,
        DispatchesRead,
        DispatchesReadAll,
        DispatchesUpdate,
        DispatchesDelete,
        UsersCreate,
        UsersRead,
        UsersUpdate,
        UsersDelete,
        CompaniesRead,
        CompaniesUpdate,
        DriversRead,
        DispatchesSyncJobUpdate
    ];
}
