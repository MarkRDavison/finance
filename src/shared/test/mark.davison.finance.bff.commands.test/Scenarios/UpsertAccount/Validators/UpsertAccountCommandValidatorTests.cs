namespace mark.davison.finance.bff.commands.test.Scenarios.UpsertAccount.Validators;

[TestClass]
public class UpsertAccountCommandValidatorTests
{
    private readonly UpsertAccountCommandValidator _upsertAccountCommandValidator;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly List<AccountType> _accountTypes;
    private readonly List<Currency> _currencies;
    private readonly User _user;

    public UpsertAccountCommandValidatorTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);

        _accountTypes = new()
        {
            new AccountType { Id = AccountConstants.Asset, Type = "Asset" },
            new AccountType { Id = AccountConstants.Expense, Type = "Expense" },
            new AccountType { Id = AccountConstants.Revenue, Type = "Revenue" },
            new AccountType { Id = AccountConstants.Cash, Type = "Cash" }
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

        _upsertAccountCommandValidator = new UpsertAccountCommandValidator(_httpRepository.Object);


        _httpRepository
                    .Setup(_ => _
                        .GetEntityAsync<AccountType>(
                            It.IsAny<Guid>(),
                            It.IsAny<HeaderParameters>(),
                            It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_accountTypes[0]);

        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Currency>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_currencies[0]);

        _httpRepository
            .Setup(_ => _
                .GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>());
    }

    [TestMethod]
    public async Task Validate_WhereAccountIdIsNotValid_ReturnsError()
    {
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<AccountType>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountType?)null);

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = Guid.NewGuid()
            }
        };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Contains(UpsertAccountCommandValidator.VALIDATION_ACCOUNT_TYPE_ID));
    }

    [TestMethod]
    public async Task Validate_WhereCurrencyIdIsNotValid_ReturnsError()
    {

        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Currency>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);

        var request = new UpsertAccountCommandRequest { UpsertAccountDto = new UpsertAccountDto { AccountTypeId = Guid.NewGuid() } };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Contains(UpsertAccountCommandValidator.VALIDATION_CURRENCY_ID));
    }

    [TestMethod]
    public async Task Validate_WhereNameIsNotValid_ReturnsError()
    {

        var request = new UpsertAccountCommandRequest { UpsertAccountDto = new UpsertAccountDto { AccountTypeId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() } };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(string.Format(
                UpsertAccountCommandValidator.VALIDATION_MISSING_REQ_FIELD,
                nameof(Account.Name)))));
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithoutExpenseOrRevenue_ReturnsError()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        _httpRepository
            .Setup(_ => _
                .GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = _user.Id,
                    AccountNumber = AccountNumber
                }
            });

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = Guid.NewGuid(),
                AccountTypeId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(UpsertAccountCommandValidator.VALIDATION_DUPLICATE_ACC_NUM)));
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithExpense_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        _httpRepository
            .Setup(_ => _
                .GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountConstants.Revenue
                }
            });

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountConstants.Expense,
                CurrencyId = Guid.NewGuid(),
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithRevenue_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        _httpRepository
            .Setup(_ => _
                .GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountConstants.Expense
                }
            });

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = AccountConstants.Revenue,
                CurrencyId = Guid.NewGuid(),
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicatedAgainstMultiple_WithRevenue_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        _httpRepository
            .Setup(_ => _
                .GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountConstants.Expense
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountConstants.Revenue
                }
            });

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = Guid.NewGuid(),
                AccountTypeId = AccountConstants.Revenue,
                CurrencyId = Guid.NewGuid(),
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(UpsertAccountCommandValidator.VALIDATION_DUPLICATE_ACC_NUM)));
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsNotDuplicated_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                Name = "Name",
                AccountNumber = AccountNumber
            }
        };
        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task Validate_WhereOpeningBalanceSpecifiedButNotOpeningBalanceDate_ReturnsError()
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountTypeId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                Name = "Name",
                AccountNumber = "AccountNumber",
                OpeningBalance = CurrencyRules.ToPersisted(100.0M)
            }
        };

        var response = await _upsertAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(UpsertAccountCommandValidator.VALIDATION_MISSING_OPENING_BAL_DATE)));
    }
}

