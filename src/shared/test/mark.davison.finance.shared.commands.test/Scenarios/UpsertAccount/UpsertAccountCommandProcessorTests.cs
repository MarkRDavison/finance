namespace mark.davison.finance.shared.commands.test.Scenarios.UpsertAccount;

[TestClass]
public class UpsertAccountCommandProcessorTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<IDateService> _dateService;
    private readonly Mock<ICommandHandler<CreateTransactionRequest, CreateTransactionResponse>> _createTransactionHandlerMock;
    private readonly UpsertAccountCommandProcessor _upsertAccountCommandProcessor;
    private readonly CancellationToken _token;

    public UpsertAccountCommandProcessorTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory(_ => new FinanceDbContext(_));
        _currentUserContextMock = new(MockBehavior.Strict);
        _dateService = new(MockBehavior.Strict);
        _createTransactionHandlerMock = new(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });

        _upsertAccountCommandProcessor = new(_createTransactionHandlerMock.Object, _dateService.Object, (IFinanceDbContext)_dbContext);

        _dateService.Setup(_ => _.Now).Returns(DateTime.Now);

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTransactionRequest r, ICurrentUserContext uc, CancellationToken c) => new CreateTransactionResponse());
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

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTransactionRequest r, ICurrentUserContext uc, CancellationToken c) =>
            {
                Assert.AreEqual(TransactionConstants.OpeningBalance, r.TransactionTypeId);
                Assert.AreEqual(1, r.Transactions.Count);
                var transaction = r.Transactions[0];
                Assert.AreNotEqual(Guid.Empty, transaction.Id);
                Assert.AreEqual(request.UpsertAccountDto.OpeningBalance, transaction.Amount);
                Assert.IsNull(transaction.ForeignCurrencyId);
                Assert.IsNull(transaction.ForeignAmount);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.Description));
                Assert.AreEqual(AccountConstants.OpeningBalance, transaction.SourceAccountId);
                Assert.AreEqual(request.UpsertAccountDto.OpeningBalanceDate, transaction.Date);
                Assert.AreEqual(request.UpsertAccountDto.CurrencyId, transaction.CurrencyId);
                Assert.IsNull(transaction.ForeignAmount);
                Assert.IsNull(transaction.ForeignCurrencyId);
                Assert.IsNull(transaction.BudgetId);
                Assert.IsNull(transaction.CategoryId);
                Assert.IsNull(transaction.BillId);
                return new CreateTransactionResponse();
            })
            .Verifiable();

        await _upsertAccountCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, _token);

        var persistedAccount = await _dbContext.GetByIdAsync<Account>(request.UpsertAccountDto.Id, _token);

        persistedAccount.Should().NotBeNull();

        _createTransactionHandlerMock
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionRequest>(),
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

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .Verifiable();

        await _upsertAccountCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, _token);

        _createTransactionHandlerMock
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionRequest>(),
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

        await _dbContext.AddAsync(account, _token);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        _createTransactionHandlerMock
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTransactionRequest r, ICurrentUserContext uc, CancellationToken c) => new CreateTransactionResponse())
            .Verifiable();

        await _upsertAccountCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, _token);

        _createTransactionHandlerMock
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionRequest>(),
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
            TransactionTypeId = TransactionConstants.OpeningBalance,
            Transactions = new List<Transaction>
            {
                transaction,
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = AccountConstants.OpeningBalance
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

        await _dbContext.AddAsync(account, _token);
        await _dbContext.AddAsync(transaction, _token);
        await _dbContext.AddAsync(transaction.TransactionJournal, _token);
        await _dbContext.AddAsync(transaction.TransactionJournal.TransactionGroup, _token);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        await _upsertAccountCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, _token);

        var fetchedTransaction = await _dbContext.GetByIdAsync<Transaction>(transaction.Id, _token);
        var fetchedTransactionJournal = await _dbContext.GetByIdAsync<Transaction>(transaction.TransactionJournal.Id, _token);
        var fetchedTransactionGroup = await _dbContext.GetByIdAsync<Transaction>(transaction.TransactionJournal.TransactionGroup.Id, _token);

        fetchedTransaction.Should().BeNull();
        fetchedTransactionJournal.Should().BeNull();
        fetchedTransactionGroup.Should().BeNull();
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
            TransactionTypeId = TransactionConstants.OpeningBalance,
            Transactions = new List<Transaction>
            {
                transaction,
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = AccountConstants.OpeningBalance
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

        await _dbContext.AddAsync(account, _token);
        await _dbContext.AddAsync(transaction, _token);
        await _dbContext.AddAsync(transaction.TransactionJournal, _token);
        await _dbContext.AddAsync(transaction.TransactionJournal.TransactionGroup, _token);
        await _dbContext.SaveChangesAsync(_token);

        await _upsertAccountCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, _token);

        {
            var persitedTransactions = await _dbContext
                .Set<Transaction>()
                .Where(_ => _.TransactionJournal!.Id == transaction.TransactionJournal.Id)
                .ToListAsync(_token);

            var sourceAccountTransaction = persitedTransactions.First(_ => _.AccountId == AccountConstants.OpeningBalance);
            var destinationAccountTransaction = persitedTransactions.First(_ => _.AccountId == account.Id);

            sourceAccountTransaction.Amount.Should().Be(-request.UpsertAccountDto.OpeningBalance);
            destinationAccountTransaction.Amount.Should().Be(+request.UpsertAccountDto.OpeningBalance);

            var persitedTransactionJournal = await _dbContext
                .Set<TransactionJournal>()
                .Where(_ => _.Id == transaction.TransactionJournal.Id)
                .FirstOrDefaultAsync(_token);

            persitedTransactionJournal.Should().NotBeNull();
            persitedTransactionJournal!.Date.Should().Be(request.UpsertAccountDto.OpeningBalanceDate);
        }
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
            TransactionTypeId = TransactionConstants.OpeningBalance,
            Transactions = new List<Transaction>
            {
                transaction,
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = AccountConstants.OpeningBalance,
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

        await _dbContext.AddAsync(account, _token);
        await _dbContext.AddAsync(transaction, _token);
        await _dbContext.AddAsync(transaction.TransactionJournal, _token);
        await _dbContext.AddAsync(transaction.TransactionJournal.TransactionGroup, _token);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        await _upsertAccountCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, _token);

        {
            var persitedTransactions = await _dbContext
                .Set<Transaction>()
                .Where(_ => _.TransactionJournal!.Id == transaction.TransactionJournal.Id)
                .ToListAsync(_token);

            var sourceAccountTransaction = persitedTransactions.First(_ => _.AccountId == AccountConstants.OpeningBalance);
            var destinationAccountTransaction = persitedTransactions.First(_ => _.AccountId == account.Id);

            sourceAccountTransaction.Amount.Should().Be(-request.UpsertAccountDto.OpeningBalance);
            destinationAccountTransaction.Amount.Should().Be(+request.UpsertAccountDto.OpeningBalance);

            var persitedTransactionJournal = await _dbContext
                .Set<TransactionJournal>()
                .Where(_ => _.Id == transaction.TransactionJournal.Id)
                .FirstOrDefaultAsync(_token);

            persitedTransactionJournal.Should().NotBeNull();
            persitedTransactionJournal!.Date.Should().Be(request.UpsertAccountDto.OpeningBalanceDate);
        }
    }
}
