﻿namespace mark.davison.finance.common.server.Health.Checks;

public class LiveHealthCheck : IHealthCheck
{
    public static string Name = "LiveHealthCheck";

    private readonly IApplicationHealthState _applicationHealthState;

    public LiveHealthCheck(IApplicationHealthState applicationHealthState)
    {
        _applicationHealthState = applicationHealthState;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_applicationHealthState.Ready.GetValueOrDefault())
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
        return Task.FromResult(HealthCheckResult.Unhealthy());
    }
}

