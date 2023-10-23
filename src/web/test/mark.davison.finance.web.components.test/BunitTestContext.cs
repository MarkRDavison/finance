﻿using Bunit;
using mark.davison.common.client;
using mark.davison.common.client.abstractions.State;
using mark.davison.finance.web.components.Ignition;
using mark.davison.finance.web.features;
using mark.davison.finance.web.services;
using Microsoft.Extensions.DependencyInjection;

namespace mark.davison.finance.web.components.test;

// TODO: move to mark.davison.common.client.test
public abstract class BunitTestContext : TestContextWrapper
{
    [TestInitialize]
    public void Setup()
    {
        TestContext = new Bunit.TestContext();

        var services = new ServiceCollection();

        JSInterop
            .SetupVoid("mudPopover.initialize", _ => true);
        JSInterop
            .SetupVoid("mudKeyInterceptor.connect", _ => true);

        services
            .AddLogging()
            .UseState()
            .UseFinanceWebServices()
            .UseFinanceComponents()
            .UseCQRS(typeof(FeaturesRootType))
            .AddSingleton(_ => JSInterop.JSRuntime);

        SetupTest(services);

        Services.AddFallbackServiceProvider(services.BuildServiceProvider());

    }

    protected virtual void SetupTest(IServiceCollection services)
    {

    }

    protected void SetState<TState>(TState state) where TState : class, IState, new()
    {
        var stateStore = Services.GetRequiredService<IStateStore>();
        stateStore.SetState<TState>(state);
    }

    [TestCleanup]
    public void TearDown() => TestContext?.Dispose();
}