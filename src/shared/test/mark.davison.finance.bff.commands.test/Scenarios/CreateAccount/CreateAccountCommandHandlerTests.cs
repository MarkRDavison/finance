namespace mark.davison.finance.bff.commands.test.Scenarios.CreateAccount;

[TestClass]
public class CreateAccountCommandHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly CreateAccountCommandHandler _createAccountCommandHandler;
    private readonly Mock<ICreateAccountCommandValidator> _createAccountCommandValidatorMock;
    private readonly Mock<ICommandHandler<CreateTransactionRequest, CreateTransactionResponse>> _createTransactionCommandHandler;

    public CreateAccountCommandHandlerTests()
    {
        _httpRepositoryMock = new(MockBehavior.Strict);
        _currentUserContextMock = new(MockBehavior.Strict);
        _createTransactionCommandHandler = new(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });
        _createAccountCommandValidatorMock = new Mock<ICreateAccountCommandValidator>(MockBehavior.Strict);

        _createAccountCommandHandler = new CreateAccountCommandHandler(
            _httpRepositoryMock.Object,
            _createAccountCommandValidatorMock.Object,
            _createTransactionCommandHandler.Object);

        _createAccountCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateAccountRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateAccountResponse
            {
                Success = true
            });
    }

    [TestMethod]
    public async Task WhenValidationFails_NonSuccessReponseIsReturned()
    {
        var request = new CreateAccountRequest { };
        var validatorResponse = new CreateAccountResponse
        {
            Success = false
        };

        _createAccountCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateAccountRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatorResponse);

        var response = await _createAccountCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.AreEqual(validatorResponse, response);
    }

    [TestMethod]
    public async Task WhenOpeningBalanceNotSpecified_InitialBalanceTransactionNotCreated()
    {
        var request = new CreateAccountRequest
        {
            CreateAccountDto = new CreateAccountDto()
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

        _createTransactionCommandHandler
            .Setup(_ => _.Handle(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse());

        var response = await _createAccountCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        _createTransactionCommandHandler
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionRequest>(),
                    It.IsAny<ICurrentUserContext>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [TestMethod]
    public async Task WhenOpeningBalanceSpecified_InitialBalanceTransactionNotCreated()
    {
        var request = new CreateAccountRequest
        {
            CreateAccountDto = new CreateAccountDto
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

        _createTransactionCommandHandler
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
                Assert.AreEqual(request.CreateAccountDto.OpeningBalance, transaction.Amount);
                Assert.IsNull(transaction.ForeignCurrencyId);
                Assert.IsNull(transaction.ForeignAmount);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.Description));
                Assert.AreEqual(persistedAccount?.Id, transaction.DestinationAccountId);
                Assert.AreEqual(Account.OpeningBalance, transaction.SourceAccountId);
                Assert.AreEqual(request.CreateAccountDto.OpeningBalanceDate, transaction.Date);
                Assert.AreEqual(request.CreateAccountDto.CurrencyId, transaction.CurrencyId);
                Assert.IsNull(transaction.ForeignAmount);
                Assert.IsNull(transaction.ForeignCurrencyId);
                Assert.IsNull(transaction.BudgetId);
                Assert.IsNull(transaction.CategoryId);
                Assert.IsNull(transaction.BillId);
                return new CreateTransactionResponse();
            })
            .Verifiable();

        var response = await _createAccountCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<Account>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        Assert.IsNotNull(persistedAccount);

        _createTransactionCommandHandler
            .Verify(
                _ => _.Handle(
                    It.IsAny<CreateTransactionRequest>(),
                    It.IsAny<ICurrentUserContext>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

