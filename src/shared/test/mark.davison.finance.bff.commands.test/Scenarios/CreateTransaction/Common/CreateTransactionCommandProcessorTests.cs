namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common;

[TestClass]
public class CreateTransactionCommandProcessorTests
{
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly CreateTransactionCommandProcessor _processor;
    private readonly User _user;

    public CreateTransactionCommandProcessorTests()
    {
        _currentUserContext = new(MockBehavior.Strict);
        _httpRepository = new(MockBehavior.Strict);

        _processor = new();

        _user = new() { Id = Guid.NewGuid() };

        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(_user);


        _httpRepository
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<TransactionGroup>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionGroup e, HeaderParameters h, CancellationToken c) => e);

        _httpRepository
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<TransactionJournal>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<TransactionJournal> e, HeaderParameters h, CancellationToken c) => e);

        _httpRepository
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<Transaction>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Transaction> e, HeaderParameters h, CancellationToken c) => e);
    }

    [TestMethod]
    public async Task Process_CreatesTransactionGroup()
    {
        var request = new CreateTransactionRequest
        {
            Description = "Split description",
            Transactions =
            {
                new CreateTransactionDto(),
                new CreateTransactionDto()
            }
        };
        var response = new CreateTransactionResponse();

        _httpRepository
            .Setup(_ => _.UpsertEntityAsync<TransactionGroup>(
                It.IsAny<TransactionGroup>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionGroup e, HeaderParameters h, CancellationToken c) =>
            {
                Assert.AreNotEqual(Guid.Empty, e.Id);
                Assert.AreEqual(request.Description, e.Title);

                return e;
            })
            .Verifiable();

        await _processor.Process(request, response, _currentUserContext.Object, _httpRepository.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntityAsync<TransactionGroup>(
                    It.IsAny<TransactionGroup>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Process_CreatesTransactionGroup_WithoutDescription_IfOnlyOneTransaction()
    {
        var request = new CreateTransactionRequest
        {
            Description = "Split description",
            Transactions =
            {
                new CreateTransactionDto()
            }
        };
        var response = new CreateTransactionResponse();

        _httpRepository
            .Setup(_ => _.UpsertEntityAsync<TransactionGroup>(
                It.IsAny<TransactionGroup>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionGroup e, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(string.IsNullOrEmpty(e.Title));

                return e;
            })
            .Verifiable();

        await _processor.Process(request, response, _currentUserContext.Object, _httpRepository.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntityAsync<TransactionGroup>(
                    It.IsAny<TransactionGroup>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Process_CreatesTransactionJournal()
    {
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                new CreateTransactionDto()
            }
        };
        var response = new CreateTransactionResponse();

        _httpRepository
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<TransactionJournal>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<TransactionJournal> e, HeaderParameters h, CancellationToken c) =>
            {
                return e;
            })
            .Verifiable();

        await _processor.Process(request, response, _currentUserContext.Object, _httpRepository.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntitiesAsync(
                    It.IsAny<List<TransactionJournal>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Process_CreatesTransactionJournals_WithExpectedValues()
    {
        var createTransactionDto = new CreateTransactionDto()
        {
            Description = "Some transaction description",
            BillId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            ForeignCurrencyId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today)
        };
        var request = new CreateTransactionRequest
        {
            TransactionTypeId = TransactionConstants.Deposit,
            Transactions =
            {
                createTransactionDto,
                createTransactionDto
            }
        };
        var response = new CreateTransactionResponse();

        _httpRepository
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<TransactionJournal>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<TransactionJournal> e, HeaderParameters h, CancellationToken c) =>
            {
                for (int i = 0; i < e.Count; ++i)
                {
                    Assert.AreNotEqual(Guid.Empty, e[i].Id);
                    Assert.AreNotEqual(Guid.Empty, e[i].TransactionGroupId);
                    Assert.AreEqual(request.TransactionTypeId, e[i].TransactionTypeId);
                    Assert.AreEqual(createTransactionDto.Description, e[i].Description);
                    Assert.AreEqual(createTransactionDto.BillId, e[i].BillId);
                    Assert.AreEqual(createTransactionDto.CurrencyId, e[i].CurrencyId);
                    Assert.AreEqual(createTransactionDto.ForeignCurrencyId, e[i].ForeignCurrencyId);
                    Assert.AreEqual(createTransactionDto.Date, e[i].Date);
                    Assert.AreEqual(i, e[i].Order);
                    Assert.IsFalse(e[i].Completed);
                }
                return e;
            })
            .Verifiable();

        await _processor.Process(request, response, _currentUserContext.Object, _httpRepository.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntitiesAsync(
                    It.IsAny<List<TransactionJournal>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Process_CreatesTransactions()
    {
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                new CreateTransactionDto()
            }
        };
        var response = new CreateTransactionResponse();

        _httpRepository
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<Transaction>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Transaction> e, HeaderParameters h, CancellationToken c) => e)
            .Verifiable();

        await _processor.Process(request, response, _currentUserContext.Object, _httpRepository.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntitiesAsync(
                    It.IsAny<List<Transaction>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Process_CreatesTransactions_AsExpected()
    {
        var transaction = new CreateTransactionDto
        {
            SourceAccountId = Guid.NewGuid(),
            DestinationAccountId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            ForeignCurrencyId = Guid.NewGuid(),
            Description = "transaction description",
            Amount = 100,
            ForeignAmount = 125
        };
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                transaction,
                transaction
            }
        };
        var response = new CreateTransactionResponse();

        _httpRepository
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<Transaction>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Transaction> e, HeaderParameters h, CancellationToken c) =>
            {

                for (int i = 0; i < request.Transactions.Count; ++i)
                {
                    var index = i * 2;
                    var sourceTransaction = e[index + 0];
                    var destinationTransaction = e[index + 1];

                    Assert.AreNotEqual(Guid.Empty, sourceTransaction.Id);
                    Assert.AreNotEqual(Guid.Empty, sourceTransaction.TransactionJournalId);
                    Assert.AreEqual(request.Transactions[i].SourceAccountId, sourceTransaction.AccountId);
                    Assert.AreEqual(request.Transactions[i].CurrencyId, sourceTransaction.CurrencyId);
                    Assert.AreEqual(request.Transactions[i].ForeignCurrencyId, sourceTransaction.ForeignCurrencyId);
                    Assert.AreEqual(request.Transactions[i].Description, sourceTransaction.Description);
                    Assert.AreEqual(-request.Transactions[i].Amount, sourceTransaction.Amount);
                    Assert.AreEqual(-request.Transactions[i].ForeignAmount, sourceTransaction.ForeignAmount);
                    Assert.IsFalse(sourceTransaction.Reconciled);

                    Assert.AreNotEqual(Guid.Empty, destinationTransaction.Id);
                    Assert.AreNotEqual(Guid.Empty, destinationTransaction.TransactionJournalId);
                    Assert.AreEqual(request.Transactions[i].DestinationAccountId, destinationTransaction.AccountId);
                    Assert.AreEqual(request.Transactions[i].CurrencyId, destinationTransaction.CurrencyId);
                    Assert.AreEqual(request.Transactions[i].ForeignCurrencyId, destinationTransaction.ForeignCurrencyId);
                    Assert.AreEqual(request.Transactions[i].Description, destinationTransaction.Description);
                    Assert.AreEqual(request.Transactions[i].Amount, destinationTransaction.Amount);
                    Assert.AreEqual(request.Transactions[i].ForeignAmount, destinationTransaction.ForeignAmount);
                    Assert.IsFalse(destinationTransaction.Reconciled);
                }
                return e;
            })
            .Verifiable();

        await _processor.Process(request, response, _currentUserContext.Object, _httpRepository.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntitiesAsync(
                    It.IsAny<List<Transaction>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
