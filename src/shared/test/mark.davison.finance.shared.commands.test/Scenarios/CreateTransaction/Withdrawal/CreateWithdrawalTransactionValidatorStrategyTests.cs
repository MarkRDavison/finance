namespace mark.davison.finance.shared.commands.test.Scenarios.CreateTransaction.Withdrawal;

[TestClass]
public class CreateTransferTransactionValidatorStrategyTests
{
    private readonly Mock<ICreateTransctionValidationContext> _createTransctionValidationContext;
    private readonly CreateWithdrawalTransactionValidatorStrategy _strategy;

    public CreateTransferTransactionValidatorStrategyTests()
    {
        _createTransctionValidationContext = new(MockBehavior.Strict);
        _strategy = new();
    }

    [TestMethod]
    public async Task ValidateTransactionGroup_DoesNothing()
    {
        var request = new CreateTransactionRequest();
        var response = new CreateTransactionResponse();

        await _strategy.ValidateTransactionGroup(request, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Errors.Any());
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountTypeConstants.Expenses_DynamicData), typeof(AccountTypeConstants), DynamicDataSourceType.Property)]
    [DynamicData(nameof(AccountTypeConstants.Liabilities_DynamicData), typeof(AccountTypeConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_PassesForValidDestinationAccount(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountTypeConstants.Asset
        };
        var destinationAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = accountTypeId
        };

        var transaction = new CreateTransactionDto
        {
            DestinationAccountId = destinationAccount.Id,
            SourceAccountId = sourceAccount.Id
        };
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount);
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount)
            .Verifiable();

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Errors.Any(_ => _.Equals(CreateTransactionCommandValidator.VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE)));
        Assert.IsFalse(response.Errors.Any(_ => _.Equals(CreateTransactionCommandValidator.VALIDATION_INVALID_ACCOUNT_PAIR)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountTypeConstants.Revenues_DynamicData), typeof(AccountTypeConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_FailsForInvalidDestinationAccount(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountTypeConstants.Asset
        };
        var destinationAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = accountTypeId
        };

        var transaction = new CreateTransactionDto
        {
            DestinationAccountId = destinationAccount.Id,
            SourceAccountId = sourceAccount.Id
        };
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount);
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount)
            .Verifiable();

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsTrue(response.Errors.Any(_ => _.Equals(CreateTransactionCommandValidator.VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountTypeConstants.Assets_DynamicData), typeof(AccountTypeConstants), DynamicDataSourceType.Property)]
    [DynamicData(nameof(AccountTypeConstants.Liabilities_DynamicData), typeof(AccountTypeConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_PassesForValidSourceAccount(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = accountTypeId
        };
        var destinationAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountTypeConstants.Expense
        };

        var transaction = new CreateTransactionDto
        {
            SourceAccountId = sourceAccount.Id,
            DestinationAccountId = destinationAccount.Id
        };
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount)
            .Verifiable();
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount);

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Errors.Any(_ => _.Equals(CreateTransactionCommandValidator.VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE)));
        Assert.IsFalse(response.Errors.Any(_ => _.Equals(CreateTransactionCommandValidator.VALIDATION_INVALID_ACCOUNT_PAIR)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountTypeConstants.NonAssetsOrLiabilities_DynamicData), typeof(AccountTypeConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_FailsForInvalidSourceAccount(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = accountTypeId
        };
        var destinationAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountTypeConstants.Expense
        };

        var transaction = new CreateTransactionDto
        {
            SourceAccountId = sourceAccount.Id,
            DestinationAccountId = destinationAccount.Id
        };
        var request = new CreateTransactionRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount)
            .Verifiable();
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount);

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsTrue(response.Errors.Any(_ => _.Equals(CreateTransactionCommandValidator.VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

}
