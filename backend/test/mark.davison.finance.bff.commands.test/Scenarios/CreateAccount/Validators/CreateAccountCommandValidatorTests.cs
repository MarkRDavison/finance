namespace mark.davison.finance.bff.commands.test.Scenarios.CreateAccount.Validators;

[TestClass]
public class CreateAccountCommandValidatorTests
{
    private readonly CreateAccountCommandValidator _createAccountCommandValidator;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly List<Bank> _banks;
    private readonly List<AccountType> _accountTypes;
    private readonly List<Currency> _currencies;
    private readonly User _user;

    public CreateAccountCommandValidatorTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);

        _banks = new()
        {
            new Bank { Id = Bank.KiwibankId, Name = "Kiwibank" },
            new Bank { Id = Bank.BnzId, Name = "BNZ" }
        };
        _accountTypes = new()
        {
            new AccountType { Id = AccountType.Asset, Type = "Asset" },
            new AccountType { Id = AccountType.Expense, Type = "Expense" },
            new AccountType { Id = AccountType.Revenue, Type = "Revenue" },
            new AccountType { Id = AccountType.Cash, Type = "Cash" }
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

        _createAccountCommandValidator = new CreateAccountCommandValidator(_httpRepository.Object);
    }

    [TestMethod]
    public async Task Validate_WhereBankIdIsNotValid_ReturnsError()
    {
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bank?)null);

        var request = new CreateAccountRequest { BankId = Guid.NewGuid() };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Contains(CreateAccountCommandValidator.VALIDATION_BANK_ID));
    }

    [TestMethod]
    public async Task Validate_WhereAccountIdIsNotValid_ReturnsError()
    {
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<AccountType>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountType?)null);

        var request = new CreateAccountRequest { BankId = Guid.NewGuid(), AccountTypeId = Guid.NewGuid() };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Contains(CreateAccountCommandValidator.VALIDATION_ACCOUNT_TYPE_ID));
    }

    [TestMethod]
    public async Task Validate_WhereCurrencyIdIsNotValid_ReturnsError()
    {
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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
            .ReturnsAsync((Currency?)null);

        var request = new CreateAccountRequest { BankId = Guid.NewGuid(), AccountTypeId = Guid.NewGuid() };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Contains(CreateAccountCommandValidator.VALIDATION_CURRENCY_ID));
    }

    [TestMethod]
    public async Task Validate_WhereNameIsNotValid_ReturnsError()
    {
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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

        var request = new CreateAccountRequest { BankId = Guid.NewGuid(), AccountTypeId = Guid.NewGuid(), CurrencyId = Guid.NewGuid() };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(string.Format(
                CreateAccountCommandValidator.VALIDATION_MISSING_REQ_FIELD,
                nameof(Account.Name)))));
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithoutExpenseOrRevenue_ReturnsError()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber
                }
            });

        var request = new CreateAccountRequest
        {
            BankId = Guid.NewGuid(),
            AccountTypeId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Name = "Name",
            AccountNumber = AccountNumber
        };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(CreateAccountCommandValidator.VALIDATION_DUPLICATE_ACC_NUM)));
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsDuplicated_WithExpense_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountType.Revenue
                }
            });

        var request = new CreateAccountRequest
        {
            BankId = Guid.NewGuid(),
            AccountTypeId = AccountType.Expense,
            CurrencyId = Guid.NewGuid(),
            Name = "Name",
            AccountNumber = AccountNumber
        };
        var response = await _createAccountCommandValidator.Validate(
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
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountType.Expense
                }
            });

        var request = new CreateAccountRequest
        {
            BankId = Guid.NewGuid(),
            AccountTypeId = AccountType.Revenue,
            CurrencyId = Guid.NewGuid(),
            Name = "Name",
            AccountNumber = AccountNumber
        };
        var response = await _createAccountCommandValidator.Validate(
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
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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
            .ReturnsAsync(new List<Account> {
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountType.Expense
                },
                new Account
                {
                    UserId = _user.Id,
                    AccountNumber = AccountNumber,
                    AccountTypeId = AccountType.Revenue
                }
            });

        var request = new CreateAccountRequest
        {
            BankId = Guid.NewGuid(),
            AccountTypeId = AccountType.Revenue,
            CurrencyId = Guid.NewGuid(),
            Name = "Name",
            AccountNumber = AccountNumber
        };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Error.Any(_ =>
            _.Contains(CreateAccountCommandValidator.VALIDATION_DUPLICATE_ACC_NUM)));
    }

    [TestMethod]
    public async Task Validate_WhereAccountNumberIsNotDuplicated_ReturnsSuccess()
    {
        const string AccountNumber = "DUPLICATE_NUMBER";
        _httpRepository
            .Setup(_ => _
                .GetEntityAsync<Bank>(
                    It.IsAny<Guid>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(_banks[0]);

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

        var request = new CreateAccountRequest
        {
            BankId = Guid.NewGuid(),
            AccountTypeId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Name = "Name",
            AccountNumber = AccountNumber
        };
        var response = await _createAccountCommandValidator.Validate(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        Assert.IsTrue(response.Success);
    }

}

