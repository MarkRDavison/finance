namespace mark.davison.finance.shared.commands.test.Scenarios.CreateTransaction.Common.Validators;

[TestClass]
public class CreateTransactionCommandValidatorTests
{
    private readonly Mock<ICreateTransctionValidationContext> _createTransctionValidationContext;
    private readonly Mock<ICreateTransactionValidatorStrategyFactory> _createTransactionValidatorStrategyFactory;
    private readonly Mock<ICreateTransactionValidatorStrategy> _createTransactionValidatorStrategy;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTransactionCommandValidator _validator;


    public CreateTransactionCommandValidatorTests()
    {
        _createTransctionValidationContext = new(MockBehavior.Strict);
        _createTransactionValidatorStrategyFactory = new(MockBehavior.Strict);
        _createTransactionValidatorStrategy = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);

        _validator = new(_createTransactionValidatorStrategyFactory.Object, _createTransctionValidationContext.Object);
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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DATE, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_ReturnsMessageWhenCategoryIdIsInvalid()
    {
        var transaction = new CreateTransactionDto { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        _createTransctionValidationContext
            .Setup(_ => _.GetCategoryById(
                transaction.CategoryId.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null)
            .Verifiable();

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        _createTransctionValidationContext
            .Verify(
                _ => _.GetCategoryById(
                    transaction.CategoryId.Value,
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_CATEGORY_ID, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_ReturnsNoMessageWhenCategoryIdIsValid()
    {
        var transaction = new CreateTransactionDto { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                transaction
            }
        };

        _createTransctionValidationContext
            .Setup(_ => _.GetCategoryById(
                transaction.CategoryId.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category())
            .Verifiable();

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        _createTransctionValidationContext
            .Verify(
                _ => _.GetCategoryById(
                    transaction.CategoryId.Value,
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsFalse(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_CATEGORY_ID, transaction.Id)));
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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(CreateTransactionCommandValidator.VALIDATION_GROUP_DESCRIPTION));

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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DUPLICATE_SRC_DEST_ACCOUNT, transaction.Id)));
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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(CreateTransactionCommandValidator.VALIDATION_TRANSACTION_TYPE));
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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_CURRENCY_ID, transaction.Id)));
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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_FOREIGN_CURRENCY_ID, transaction.Id)));
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

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DUPLICATE_CURRENCY, transaction.Id)));
    }

    [TestMethod]
    public async Task Validate_CallsValidateTransactionGroup_OnValidatorStrategy()
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

        await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

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

    [TestMethod]
    public async Task Validate_WhereNoDuplicateTagsArePassed_ReturnsNoMessage()
    {
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                new() { Tags = new() {"1", "2", "3"} }
            }
        };

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Warnings.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DUPLICATE_TAGS, string.Empty)));
    }

    [TestMethod]
    public async Task Validate_WhereDuplicateTagsArePassed_ReturnsWarningMessage()
    {
        var request = new CreateTransactionRequest
        {
            Transactions = new()
            {
                new() { Tags = new() {"1", "1", "3", "3"} }
            }
        };

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Warnings.Contains(string.Format(CreateTransactionCommandValidator.VALIDATION_DUPLICATE_TAGS, "1,3")));
    }
}
