﻿namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Transfer;

[TestClass]
public class CreateTransferTransactionValidatorStrategyTests
{
    private readonly Mock<ICreateTransctionValidationContext> _createTransctionValidationContext;
    private readonly CreateTransferTransactionValidatorStrategy _strategy;

    public CreateTransferTransactionValidatorStrategyTests()
    {
        _createTransctionValidationContext = new(MockBehavior.Strict);
        _strategy = new();
    }

    [TestMethod]
    public async Task ValidateTransactionGroup_DoesNothing()
    {
        var request = new CreateTransactionCommandRequest();
        var response = new CreateTransactionCommandResponse();

        await _strategy.ValidateTransactionGroup(request, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Error.Any());
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountConstants.Assets_DynamicData), typeof(AccountConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_PassesForValidDestinationAccount_ForAsset(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Asset
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
        var request = new CreateTransactionCommandRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionCommandResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount);
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount)
            .Verifiable();

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE)));
        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_ACCOUNT_PAIR)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountConstants.Liabilities_DynamicData), typeof(AccountConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_PassesForValidDestinationAccount_ForLiability(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Mortgage
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
        var request = new CreateTransactionCommandRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionCommandResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount);
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount)
            .Verifiable();

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE)));
        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_ACCOUNT_PAIR)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountConstants.Revenues_DynamicData), typeof(AccountConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_FailsForInvalidDestinationAccount(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Asset
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
        var request = new CreateTransactionCommandRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionCommandResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount);
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount)
            .Verifiable();

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsTrue(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountConstants.Assets_DynamicData), typeof(AccountConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_PassesForValidSourceAccount_ForAsset(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = accountTypeId
        };
        var destinationAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Asset
        };

        var transaction = new CreateTransactionDto
        {
            SourceAccountId = sourceAccount.Id,
            DestinationAccountId = destinationAccount.Id
        };
        var request = new CreateTransactionCommandRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionCommandResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount)
            .Verifiable();
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount);

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE)));
        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_ACCOUNT_PAIR)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountConstants.Liabilities_DynamicData), typeof(AccountConstants), DynamicDataSourceType.Property)]
    public async Task ValidateTransaction_PassesForValidSourceAccount_ForLiability(Guid accountTypeId)
    {
        var sourceAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = accountTypeId
        };
        var destinationAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Debt
        };

        var transaction = new CreateTransactionDto
        {
            SourceAccountId = sourceAccount.Id,
            DestinationAccountId = destinationAccount.Id
        };
        var request = new CreateTransactionCommandRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionCommandResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount)
            .Verifiable();
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount);

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE)));
        Assert.IsFalse(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_ACCOUNT_PAIR)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AccountConstants.NonAssetsOrLiabilities_DynamicData), typeof(AccountConstants), DynamicDataSourceType.Property)]
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
            AccountTypeId = AccountConstants.Expense
        };

        var transaction = new CreateTransactionDto
        {
            SourceAccountId = sourceAccount.Id,
            DestinationAccountId = destinationAccount.Id
        };
        var request = new CreateTransactionCommandRequest
        {
            Transactions =
            {
                transaction
            }
        };

        var response = new CreateTransactionCommandResponse();

        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAccount)
            .Verifiable();
        _createTransctionValidationContext
            .Setup(_ => _.GetAccountById(transaction.DestinationAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destinationAccount);

        await _strategy.ValidateTranasction(transaction, response, _createTransctionValidationContext.Object);

        Assert.IsTrue(response.Error.Any(_ => _.Equals(CreateTransactionValidatorStrategy.VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE)));

        _createTransctionValidationContext
            .Verify(
                _ => _.GetAccountById(transaction.SourceAccountId, It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
