using mark.davison.finance.models.dtos.Commands.CreateTransaction;

namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common.Validators;

[TestClass]
public class CreateTransactionCommandValidatorTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICreateTransctionValidationContext> _createTransctionValidationContext;
    private readonly Mock<ICreateTransactionValidatorStrategyFactory> _createTransactionValidatorStrategyFactory;
    private readonly Mock<ICreateTransactionValidatorStrategy> _createTransactionValidatorStrategy;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTransactionCommandValidator _validator;

    public CreateTransactionCommandValidatorTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _createTransctionValidationContext = new(MockBehavior.Strict);
        _createTransactionValidatorStrategyFactory = new(MockBehavior.Strict);
        _createTransactionValidatorStrategy = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _validator = new(_httpRepository.Object, _createTransctionValidationContext.Object, _createTransactionValidatorStrategyFactory.Object);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _createTransactionValidatorStrategyFactory.Setup(_ => _.CreateStrategy(It.IsAny<Guid>())).Returns(_createTransactionValidatorStrategy.Object);
        _createTransactionValidatorStrategy.Setup(_ => _.ValidateTransactionGroup(It.IsAny<CreateTransactionRequest>(), It.IsAny<CreateTransactionResponse>(), It.IsAny<ICreateTransctionValidationContext>())).Returns(Task.CompletedTask);
        _createTransactionValidatorStrategy.Setup(_ => _.ValidateTranasction(It.IsAny<CreateTransactionDto>(), It.IsAny<CreateTransactionResponse>(), It.IsAny<ICreateTransctionValidationContext>())).Returns(Task.CompletedTask);
    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenDateIsInvalid()
    {
        var transaction = new CreateTransactionDto { Id = Guid.NewGuid() };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DATE, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_WhereMultipleTransactions_ReturnsMessageWhenSplitDescriptionMissing()
    {
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(CreateTransactionCommandValidator.VALIDATION_GROUP_DESCRIPTION));

    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenSourceAndDestinationAccountAreSame()
    {
        var transaction = new CreateTransactionDto { Id = Guid.NewGuid() };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DUPLICATE_SRC_DEST_ACCOUNT, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenTransactionTypeIsInvalid()
    {
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                new()
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(CreateTransactionCommandValidator.VALIDATION_TRANSACTION_TYPE));
    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenCurrencyIdIsInvalid()
    {
        var transaction = new CreateTransactionDto { Id = Guid.NewGuid() };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_CURRENCY_ID, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenForeginCurrencyIdIsInvalid()
    {
        var transaction = new CreateTransactionDto { Id = Guid.NewGuid(), ForeignCurrencyId = Guid.Empty };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_FOREIGN_CURRENCY_ID, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenCurrencyIdsAreSame()
    {
        var transaction = new CreateTransactionDto
        {
            Id = Guid.NewGuid(),
            CurrencyId = Currency.NZD,
            ForeignCurrencyId = Currency.NZD
        };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DUPLICATE_CURRENCY, transaction.Id)));
    }

    [TestMethod]
    public async Task _Validate_CallsValidateTransactionGroup_OnValidatorStrategy()
    {

        _createTransactionValidatorStrategy
            .Setup(_ => _.ValidateTransactionGroup(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<CreateTransactionResponse>(),
                It.IsAny<ICreateTransctionValidationContext>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        _createTransactionValidatorStrategy
            .Setup(_ => _.ValidateTranasction(
                It.IsAny<CreateTransactionDto>(),
                It.IsAny<CreateTransactionResponse>(),
                It.IsAny<ICreateTransctionValidationContext>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                new()
            }
        };

        await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _createTransactionValidatorStrategy
            .Verify(_ => _.ValidateTransactionGroup(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<CreateTransactionResponse>(),
                It.IsAny<ICreateTransctionValidationContext>()),
            Times.Once);
        _createTransactionValidatorStrategy
            .Verify(_ => _.ValidateTranasction(
                It.IsAny<CreateTransactionDto>(),
                It.IsAny<CreateTransactionResponse>(),
                It.IsAny<ICreateTransctionValidationContext>()),
            Times.Once);
    }
}
