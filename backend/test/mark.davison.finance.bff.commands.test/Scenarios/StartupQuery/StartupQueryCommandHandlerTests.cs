using System.Linq;

namespace mark.davison.finance.bff.commands.test.Scenarios.StartupQuery;

[TestClass]
public class StartupQueryCommandHandlerTests
{
    private readonly StartupQueryCommandHandler _handler;
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly User user;

    public StartupQueryCommandHandlerTests()
    {
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _httpRepository = new Mock<IHttpRepository>(MockBehavior.Strict);
        _handler = new StartupQueryCommandHandler(_httpRepository.Object);

        user = new User
        {

        };

        _currentUserContext.Setup(_ => _.CurrentUser).Returns(() => user);
        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
    }

    [TestMethod]
    public async Task Handle_ReturnsExpectedData()
    {
        var request = new StartupQueryRequest { };
        var currencies = new List<Currency> {
            new Currency{ Code = "NZD", Name = "New Zealand Dollar", Symbol = "NZ$", DecimalPlaces = 2 }
        };

        _httpRepository
            .Setup(_ => _
                .GetEntitiesAsync<Currency>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies)
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        _httpRepository
            .Verify(_ => _
                .GetEntitiesAsync<Currency>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.AreEqual(currencies.Count, response.Currencies.Count);
        foreach (var (first, second) in response.Currencies.Zip(currencies))
        {
            Assert.AreEqual(first.Code, second.Code);
            Assert.AreEqual(first.Name, second.Name);
            Assert.AreEqual(first.Symbol, second.Symbol);
            Assert.AreEqual(first.DecimalPlaces, second.DecimalPlaces);
        }
    }

}

