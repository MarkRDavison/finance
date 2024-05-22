namespace mark.davison.finance.shared.commands.test.Scenarios.UpsertAccount;

[TestClass]
public class UpsertAccountCommandValidatorTests
{
    private readonly UpsertAccountCommandValidator _upsertAccountCommandValidator;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly List<AccountType> _accountTypes;
    private readonly List<Currency> _currencies;
    private readonly User _user;
    private readonly CancellationToken _token;

    public UpsertAccountCommandValidatorTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory(_ => new FinanceDbContext(_));
        _currentUserContext = new(MockBehavior.Strict);

        _accountTypes = new()
        {
            new AccountType { Id = AccountTypeConstants.Asset, Type = "Asset" },
            new AccountType { Id = AccountTypeConstants.Expense, Type = "Expense" },
            new AccountType { Id = AccountTypeConstants.Revenue, Type = "Revenue" },
            new AccountType { Id = AccountTypeConstants.Cash, Type = "Cash" }
        };
        _currencies = new()
        {
            new Currency { Id = Currency.NZD }
        };
        _user = new()
        {
            Id = Guid.NewGuid()
        };
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(_user);
        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);

        _upsertAccountCommandValidator = new UpsertAccountCommandValidator((IFinanceDbContext)_dbContext);
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await _dbContext.UpsertEntitiesAsync(_accountTypes, _token);
        await _dbContext.UpsertEntitiesAsync(_currencies, _token);
        await _dbContext.SaveChangesAsync(_token);
    }

    [TestMethod]
    public async Task Validate_WhereAccountIdIsNotValid_ReturnsError()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = Guid.NewGuid()
            }
        };
        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch($"*{UpsertAccountCommandValidator.VALIDATION_ACCOUNT_TYPE_ID}*");
    }

    [TestMethod]
    public async Task Validate_WhereCurrencyIdIsNotValid_ReturnsError()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountTypeConstants.Asset
            }
        };

        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch($"*{UpsertAccountCommandValidator.VALIDATION_CURRENCY_ID}*");
    }

    [TestMethod]
    public async Task Validate_WhereNameIsNotValid_ReturnsError()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountTypeConstants.Asset,
                CurrencyId = Currency.NZD
            }
        };
        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch($"*{string.Format(UpsertAccountCommandValidator.VALIDATION_MISSING_REQ_FIELD, nameof(Account.Name))}*");
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithoutExpenseOrRevenue_ReturnsError()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        await _dbContext.UpsertEntityAsync(new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountTypeConstants.Asset,
            CurrencyId = Currency.NZD,
            UserId = _user.Id,
            AccountNumber = AccountNumber
        }, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeConstants.Asset,
                CurrencyId = Currency.NZD,
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };

        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch($"*{UpsertAccountCommandValidator.VALIDATION_DUPLICATE_ACC_NUM}*");
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithExpense_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        await _dbContext.UpsertEntityAsync(new Account
        {
            UserId = _user.Id,
            AccountNumber = AccountNumber,
            CurrencyId = Currency.NZD,
            AccountTypeId = AccountTypeConstants.Revenue
        }, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountTypeConstants.Expense,
                CurrencyId = Currency.NZD,
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeTrue();
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithRevenue_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        await _dbContext.UpsertEntityAsync(new Account
        {
            UserId = _user.Id,
            AccountNumber = AccountNumber,
            CurrencyId = Currency.NZD,
            AccountTypeId = AccountTypeConstants.Expense
        }, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountTypeConstants.Revenue,
                CurrencyId = Currency.NZD,
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeTrue();
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicatedAgainstMultiple_WithRevenue_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        await _dbContext.UpsertEntitiesAsync([
            new Account
            {
                Id = Guid.NewGuid(),
                UserId = _user.Id,
                CurrencyId = Currency.NZD,
                AccountNumber = AccountNumber,
                AccountTypeId = AccountTypeConstants.Expense
            },
            new Account
            {
                Id = Guid.NewGuid(),
                UserId = _user.Id,
                CurrencyId = Currency.NZD,
                AccountNumber = AccountNumber,
                AccountTypeId = AccountTypeConstants.Revenue
            }], _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeConstants.Revenue,
                CurrencyId = Currency.NZD,
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch($"*{UpsertAccountCommandValidator.VALIDATION_DUPLICATE_ACC_NUM}*");
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsNotDuplicated_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountTypeConstants.Asset,
                CurrencyId = Currency.NZD,
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeTrue();
    }

    [TestMethod]
    public async Task Validate_WhereOpeningBalanceSpecifiedButNotOpeningBalanceDate_ReturnsError()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountTypeConstants.Asset,
                CurrencyId = Currency.NZD,
                Name = "Name",
                AccountNumber = "AccountNumber",
                OpeningBalance = CurrencyRules.ToPersisted(100.0M)
            }
        };

        var response = await _upsertAccountCommandValidator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch($"*{UpsertAccountCommandValidator.VALIDATION_MISSING_OPENING_BAL_DATE}*");
    }
}

