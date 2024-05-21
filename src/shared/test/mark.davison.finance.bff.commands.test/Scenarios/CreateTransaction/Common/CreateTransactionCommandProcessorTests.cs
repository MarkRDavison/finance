using Microsoft.EntityFrameworkCore;

namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common;

[TestClass]
public class CreateTransactionCommandProcessorTests
{
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly CreateTransactionCommandProcessor _processor;
    private readonly User _user;
    private readonly CancellationToken _token;

    public CreateTransactionCommandProcessorTests()
    {
        _token = CancellationToken.None;
        _currentUserContext = new(MockBehavior.Strict);
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _processor = new((IFinanceDbContext)_dbContext);

        _user = new() { Id = Guid.NewGuid() };

        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(_user);
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

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var transactionGroupExists = await _dbContext
            .Set<TransactionGroup>()
            .Where(_ => _.Id == response.Group.Id && _.Title == request.Description)
            .AnyAsync(_token);

        transactionGroupExists.Should().BeTrue();
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

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var transactionGroup = await _dbContext
            .Set<TransactionGroup>()
            .Where(_ => _.Id == response.Group.Id)
            .FirstOrDefaultAsync(_token);

        transactionGroup.Should().NotBeNull();
        transactionGroup!.Title.Should().BeNullOrEmpty();
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

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var tranasctionJournalIds = response.Journals.Select(_ => _.Id).ToList();

        var transactionJournals = await _dbContext
            .Set<TransactionJournal>()
            .Where(_ => tranasctionJournalIds.Contains(_.Id))
            .ToListAsync(_token);

        for (int i = 0; i < transactionJournals.Count; ++i)
        {
            Assert.AreNotEqual(Guid.Empty, transactionJournals[i].Id);
            Assert.AreNotEqual(Guid.Empty, transactionJournals[i].TransactionGroupId);
            Assert.AreEqual(request.TransactionTypeId, transactionJournals[i].TransactionTypeId);
            Assert.AreEqual(createTransactionDto.Description, transactionJournals[i].Description);
            Assert.AreEqual(createTransactionDto.BillId, transactionJournals[i].BillId);
            Assert.AreEqual(createTransactionDto.CurrencyId, transactionJournals[i].CurrencyId);
            Assert.AreEqual(createTransactionDto.ForeignCurrencyId, transactionJournals[i].ForeignCurrencyId);
            Assert.AreEqual(createTransactionDto.Date, transactionJournals[i].Date);
            Assert.AreEqual(i, transactionJournals[i].Order);
            Assert.IsFalse(transactionJournals[i].Completed);
        }
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

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var tranasctionIds = response.Transactions.Select(_ => _.Id).ToList();

        var transactions = await _dbContext
            .Set<Transaction>()
            .Where(_ => tranasctionIds.Contains(_.Id))
            .ToListAsync(_token);

        for (int i = 0; i < request.Transactions.Count; ++i)
        {
            var index = i * 2;
            var sourceTransaction = transactions[index + 0];
            var destinationTransaction = transactions[index + 1];

            Assert.AreNotEqual(Guid.Empty, sourceTransaction.Id);
            Assert.AreNotEqual(Guid.Empty, sourceTransaction.TransactionJournalId);
            Assert.AreEqual(request.Transactions[i].SourceAccountId, sourceTransaction.AccountId);
            Assert.AreEqual(request.Transactions[i].CurrencyId, sourceTransaction.CurrencyId);
            Assert.AreEqual(request.Transactions[i].ForeignCurrencyId, sourceTransaction.ForeignCurrencyId);
            Assert.AreEqual(request.Transactions[i].Description, sourceTransaction.Description);
            Assert.AreEqual(-request.Transactions[i].Amount, sourceTransaction.Amount);
            Assert.AreEqual(-request.Transactions[i].ForeignAmount, sourceTransaction.ForeignAmount);
            Assert.IsFalse(sourceTransaction.Reconciled);
            Assert.IsTrue(sourceTransaction.IsSource);

            Assert.AreNotEqual(Guid.Empty, destinationTransaction.Id);
            Assert.AreNotEqual(Guid.Empty, destinationTransaction.TransactionJournalId);
            Assert.AreEqual(request.Transactions[i].DestinationAccountId, destinationTransaction.AccountId);
            Assert.AreEqual(request.Transactions[i].CurrencyId, destinationTransaction.CurrencyId);
            Assert.AreEqual(request.Transactions[i].ForeignCurrencyId, destinationTransaction.ForeignCurrencyId);
            Assert.AreEqual(request.Transactions[i].Description, destinationTransaction.Description);
            Assert.AreEqual(request.Transactions[i].Amount, destinationTransaction.Amount);
            Assert.AreEqual(request.Transactions[i].ForeignAmount, destinationTransaction.ForeignAmount);
            Assert.IsFalse(destinationTransaction.Reconciled);
            Assert.IsFalse(destinationTransaction.IsSource);
        }
    }

    [TestMethod]
    public async Task Process_CreatesTags_IfAnyUsedInNewTransactionCreation()
    {
        var transaction = new CreateTransactionDto
        {
            SourceAccountId = Guid.NewGuid(),
            DestinationAccountId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            ForeignCurrencyId = Guid.NewGuid(),
            Description = "transaction description",
            Amount = 100,
            ForeignAmount = 125,
            Tags = new() { "Tag1", "Tag2" }
        };
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var tags = await _dbContext.Set<Tag>().ToListAsync(_token);

        foreach (var tag in transaction.Tags)
        {
            var t = tags.Find(_ => _.Name == tag);

            t.Should().NotBeNull();
        }
    }
}
