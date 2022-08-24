using Moq;

namespace mark.davison.finance.web.ui.test.Helpers;

public class TestBase : TestContextWrapper
{
    protected readonly StateStore _stateStore;
    protected readonly ComponentSubscriptions _componentSubscriptions;
    protected readonly Mock<ICQRSDispatcher> _dispatcher;
    protected readonly IServiceScopeFactory _serviceScopeFactory = null!;

    public TestBase()
    {
        _componentSubscriptions = new();
        _stateStore = new(_componentSubscriptions);
        _dispatcher = new(MockBehavior.Strict);
    }

    [TestInitialize]
    public void Setup()
    {
        TestContext = new();

        JSInterop.SetupVoid("setupOutsideAlerter", _ => true);
        JSInterop.SetupVoid("cleanupOutsideAlerter", _ => true);

        SetupTest();
    }

    protected virtual void SetupTest()
    {
        Services.Add(new ServiceDescriptor(typeof(IStateStore), _stateStore));
        Services.Add(new ServiceDescriptor(typeof(IComponentSubscriptions), _componentSubscriptions));
        Services.Add(new ServiceDescriptor(typeof(ICQRSDispatcher), _dispatcher.Object));
    }

    [TestCleanup]
    public void TearDown() => TestContext?.Dispose();
}
