namespace mark.davison.finance.bff.commands.test.Scenarios.UpsertAccount.Processors;

[TestClass]
public class UpsertAccountCommandProcessorTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<IDateService> _dateService;
    private readonly Mock<ICommandHandler<CreateTransactionCommandRequest, CreateTransactionCommandResponse>> _createTransactionHandlerMock;
    private readonly UpsertAccountCommandProcessor _upsertAccountCommandProcessor;

    public UpsertAccountCommandProcessorTests()
    {
        _httpRepositoryMock = new(MockBehavior.Strict);
        _currentUserContextMock = new(MockBehavior.Strict);
        _dateService = new(MockBehavior.Strict);
        _createTransactionHandlerMock = new(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });

        _upsertAccountCommandProcessor = new(_createTransactionHandlerMock.Object, _dateService.Object);

        _dateService.Setup(_ => _.Now).Returns(DateTime.Now);

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<Account>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account a, HeaderParameters h, CancellationToken c) => a);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Account>(
                It.IsAny<Guid>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);
    }

    [TestMethod]
    public async Task FetchesExistingAccountAndOpeningBalance()
    {
        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(q.ContainsKey("TransactionJournal.TransactionTypeId"));
                Assert.IsTrue(q.ContainsKey(nameof(Transaction.AccountId)));
                return null;
            })
            .Verifiable();

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Account>(
                It.IsAny<Guid>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, HeaderParameters h, CancellationToken c) =>
            {
                return null;
            })
            .Verifiable();

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto()
        };

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.GetEntityAsync<Transaction>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _httpRepositoryMock
            .Verify(
                _ => _.GetEntityAsync<Account>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task WhenOpeningBalanceSpecified_InitialBalanceTransactionAreCreated()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.Today),
                CurrencyId = Currency.NZD
            }
        };

        Account? persistedAccount = null;

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<Account>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account a, HeaderParameters h, CancellationToken c) =>
            {
                persistedAccount = a;
                return a;
            })
            .Verifiable();

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionCommandRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTransactionCommandRequest r, ICurrentUserContext uc, CancellationToken c) =>
            {
                Assert.AreEqual(TransactionConstants.OpeningBalance, r.TransactionTypeId);
                Assert.AreEqual(1, r.Transactions.Count);
                var transaction = r.Transactions[0];
                Assert.AreNotEqual(Guid.Empty, transaction.Id);
                Assert.AreEqual(request.UpsertAccountDto.OpeningBalance, transaction.Amount);
                Assert.IsNull(transaction.ForeignCurrencyId);
                Assert.IsNull(transaction.ForeignAmount);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.Description));
                Assert.AreEqual(persistedAccount?.Id, transaction.DestinationAccountId);
                Assert.AreEqual(Account.OpeningBalance, transaction.SourceAccountId);
                Assert.AreEqual(request.UpsertAccountDto.OpeningBalanceDate, transaction.Date);
                Assert.AreEqual(request.UpsertAccountDto.CurrencyId, transaction.CurrencyId);
                Assert.IsNull(transaction.ForeignAmount);
                Assert.IsNull(transaction.ForeignCurrencyId);
                Assert.IsNull(transaction.BudgetId);
                Assert.IsNull(transaction.CategoryId);
                Assert.IsNull(transaction.BillId);
                return new CreateTransactionCommandResponse();
            })
            .Verifiable();

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<Account>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        Assert.IsNotNull(persistedAccount);

        _createTransactionHandlerMock
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionCommandRequest>(),
                    It.IsAny<ICurrentUserContext>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task WhenOpeningBalanceNotSpecified_InitialBalanceTransactionNotCreated()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                CurrencyId = Currency.NZD
            }
        };

        Account? persistedAccount = null;

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<Account>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account a, HeaderParameters h, CancellationToken c) =>
            {
                persistedAccount = a;
                return a;
            })
            .Verifiable();

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionCommandRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .Verifiable();

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<Account>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        Assert.IsNotNull(persistedAccount);

        _createTransactionHandlerMock
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionCommandRequest>(),
                    It.IsAny<ICurrentUserContext>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [TestMethod]
    public async Task WhenOpeningBalanceSpecified_ForExistingAccountWithNonExistantOpeningBalance_InitialBalanceTransactionAreCreated()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CurrencyId = Currency.NZD
        };
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = account.Id,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.Today)
            }
        };

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Account>(
                It.IsAny<Guid>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<Account>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account a, HeaderParameters h, CancellationToken c) =>
            {
                return a;
            })
            .Verifiable();

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionCommandRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTransactionCommandRequest r, ICurrentUserContext uc, CancellationToken c) => new CreateTransactionCommandResponse())
            .Verifiable();

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<Account>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _createTransactionHandlerMock
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionCommandRequest>(),
                    It.IsAny<ICurrentUserContext>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task ExistingAccount_ExistingOpeningBalance_RequestRemovesOpeningBalance()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CurrencyId = Currency.NZD
        };

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id
        };
        transaction.TransactionJournal = new TransactionJournal
        {
            Id = Guid.NewGuid(),
            Transactions = new List<Transaction>
            {
                transaction,
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = Account.OpeningBalance
                }
            },
            TransactionGroup = new TransactionGroup
            {
                Id = Guid.NewGuid()
            }
        };

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = account.Id
            }
        };

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Account>(
                It.IsAny<Guid>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<TransactionJournal>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction.TransactionJournal);

        _httpRepositoryMock
            .Setup(_ => _.DeleteEntitiesAsync<Transaction>(
                It.IsAny<List<Guid>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        _httpRepositoryMock
            .Setup(_ => _.DeleteEntityAsync<TransactionJournal>(
                transaction.TransactionJournal.Id,
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        _httpRepositoryMock
            .Setup(_ => _.DeleteEntityAsync<TransactionGroup>(
                transaction.TransactionJournal.TransactionGroupId,
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);


        _httpRepositoryMock
            .Verify(
                _ => _.DeleteEntitiesAsync<Transaction>(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _httpRepositoryMock
            .Verify(
                _ => _.DeleteEntityAsync<TransactionJournal>(
                    transaction.TransactionJournal.Id,
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _httpRepositoryMock
            .Verify(
                _ => _.DeleteEntityAsync<TransactionGroup>(
                    transaction.TransactionJournal.TransactionGroupId,
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task ExistingAccount_ExistingOpeningBalance_RequestEditsOpeningBalance()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CurrencyId = Currency.NZD
        };

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id
        };
        transaction.TransactionJournal = new TransactionJournal
        {
            Id = Guid.NewGuid(),
            Transactions = new List<Transaction>
            {
                transaction,
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = Account.OpeningBalance
                }
            },
            TransactionGroup = new TransactionGroup
            {
                Id = Guid.NewGuid()
            }
        };

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = account.Id,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.Today)
            }
        };

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Account>(
                It.IsAny<Guid>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<TransactionJournal>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction.TransactionJournal);

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<Transaction>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Transaction> e, HeaderParameters h, CancellationToken c) =>
            {
                var sourceAccountTransaction = e.First(_ => _.AccountId == Account.OpeningBalance);
                var destinationAccountTransaction = e.First(_ => _.AccountId == account.Id);

                Assert.AreEqual(-request.UpsertAccountDto.OpeningBalance, sourceAccountTransaction.Amount);
                Assert.AreEqual(+request.UpsertAccountDto.OpeningBalance, destinationAccountTransaction.Amount);

                return new List<Transaction>();
            })
            .Verifiable();

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<TransactionJournal>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionJournal tj, HeaderParameters h, CancellationToken c) =>
            {
                Assert.AreEqual(request.UpsertAccountDto.OpeningBalanceDate, tj.Date);
                return new TransactionJournal();
            })
            .Verifiable();

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntitiesAsync(
                    It.IsAny<List<Transaction>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<TransactionJournal>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task ExistingAccount_ExistingOpeningBalance_RequestDoesNotChangeOpeningBalance()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CurrencyId = Currency.NZD
        };

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = +CurrencyRules.ToPersisted(100.0M)
        };
        transaction.TransactionJournal = new TransactionJournal
        {
            Id = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            Transactions = new List<Transaction>
            {
                transaction,
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = Account.OpeningBalance,
                    Amount = -CurrencyRules.ToPersisted(100.0M)
                }
            },
            TransactionGroup = new TransactionGroup
            {
                Id = Guid.NewGuid()
            }
        };

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = account.Id,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = transaction.TransactionJournal.Date
            }
        };

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Account>(
                It.IsAny<Guid>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        _httpRepositoryMock
            .Setup(_ => _.GetEntityAsync<TransactionJournal>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction.TransactionJournal);

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntitiesAsync(
                It.IsAny<List<Transaction>>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Transaction>())
            .Verifiable();

        _httpRepositoryMock
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<TransactionJournal>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionJournal())
            .Verifiable();

        await _upsertAccountCommandProcessor.Process(request, new UpsertAccountCommandResponse(), _currentUserContextMock.Object, _httpRepositoryMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntitiesAsync(
                    It.IsAny<List<Transaction>>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<TransactionJournal>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }
}
