using mark.davison.common.client.Authentication;
using mark.davison.common.client.Ignition;
using mark.davison.finance.web.features;
using mark.davison.finance.web.services.Injection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace mark.davison.finance.web.components.test;

// TODO: move version to mark.davison.common.client.test
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
            .SetupVoid("mudPopover.connect", _ => true);
        JSInterop
            .SetupVoid("mudKeyInterceptor.connect", _ => true);

        services
            .AddLogging()
            .UseFinanceWebServices()
            .UseFinanceComponents(new AuthenticationConfig())
            .UseFluxorState(_ => { }, typeof(Program), typeof(FeaturesRootType))
            .AddSingleton(_ => JSInterop.JSRuntime);

        SetupTest(services);

        Services.AddFallbackServiceProvider(services.BuildServiceProvider());

    }

    protected virtual void SetupTest(IServiceCollection services)
    {

    }

    [TestCleanup]
    public void TearDown() => TestContext?.Dispose();
}