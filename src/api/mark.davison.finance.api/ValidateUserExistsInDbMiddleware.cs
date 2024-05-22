﻿namespace mark.davison.finance.api;

public sealed class ValidateUserExistsInDbMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<AppSettings> _appSettings;

    public ValidateUserExistsInDbMiddleware(
        RequestDelegate next,
        IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings;
    }

    public async Task Invoke(HttpContext context)
    {
        var currentUserContext = context.RequestServices.GetRequiredService<ICurrentUserContext>();
        if (currentUserContext.CurrentUser != null)
        {
            var dbContext = context.RequestServices.GetRequiredService<IFinanceDbContext>();

            var user = await dbContext.GetByIdAsync<User>(currentUserContext.CurrentUser.Id, CancellationToken.None);

            if (user == null)
            {
                await dbContext.UpsertEntityAsync(currentUserContext.CurrentUser, CancellationToken.None);
                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }

        await _next(context);
    }
}