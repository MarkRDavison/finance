namespace mark.davison.finance.web.ui.test.Pages.Transactions.AddTransaction;

[TestClass]
public class AddTransactionPageTests : TestBase
{
	private readonly AddTransactionPageViewModel _viewModel;
	private readonly Mock<IStateHelper> _stateHelper;

	public AddTransactionPageTests()
	{
		_viewModel = new(_dispatcher.Object, _stateStore, _clientNavigationManager.Object);
		_stateHelper = new(MockBehavior.Strict);
	}

	protected override void SetupTest()
	{
		base.SetupTest();
		Services.Add(new ServiceDescriptor(typeof(AddTransactionPageViewModel), _viewModel));
		Services.Add(new ServiceDescriptor(typeof(IStateHelper), _stateHelper.Object));

		_stateHelper.Setup(_ => _.FetchAccountList(It.IsAny<bool>())).Returns(Task.CompletedTask);
		_stateHelper.Setup(_ => _.FetchCategoryList()).Returns(Task.CompletedTask);
		_stateHelper.Setup(_ => _.FetchTagList()).Returns(Task.CompletedTask);
	}

	[TestMethod]
	public void InitialForm_RendersWithOneTransactionFormAndAssociatedComponents()
	{
		Guid transactionTypeId = TransactionConstants.Deposit;
		_stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
		_stateStore.SetState(LookupStateHelpers.CreateStandardState());
		_stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

		var cut = RenderComponent<AddTransactionPage>(_ => _
			.Add(_ => _.Type, transactionTypeId));

		var forms = cut.FindAll("form");
		Assert.AreEqual(1, forms.Count);
		Assert.AreEqual($"AddTransactionForm_{0}", forms[0].Id);

		var title = cut.Find("h3");
		Assert.IsNotNull(title);
		Assert.AreEqual("Transaction info", title.TextContent);

		var buttons = cut.FindAll("button");
		Assert.AreEqual(2, buttons.Count);
		Assert.IsNotNull(buttons.FirstOrDefault(_ => _.TextContent == "Submit"));
		Assert.IsNotNull(buttons.FirstOrDefault(_ => _.TextContent == "Add split transaction"));
	}

	[DataTestMethod]
	[DataRow(1, 0)]
	[DataRow(2, 0)]
	[DataRow(2, 1)]
	[DataRow(3, 0)]
	[DataRow(3, 2)]
	[DataRow(5, 0)]
	[DataRow(8, 0)]
	[DataRow(8, 6)]
	[DataRow(13, 0)]
	[DataRow(13, 11)]
	public void AddingAndRemovingSplitTransaction_RendersMultipleForms(int additionalRows, int removedRows)
	{
		Guid transactionTypeId = TransactionConstants.Deposit;
		_stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
		_stateStore.SetState(LookupStateHelpers.CreateStandardState());
		_stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

		var cut = RenderComponent<AddTransactionPage>(_ => _
			.Add(_ => _.Type, transactionTypeId));

		Assert.AreNotEqual(additionalRows, removedRows);

		var totalRows = additionalRows - removedRows + 1;

		for (int i = 0; i < additionalRows; i++)
		{
			cut
				.FindAll("button")
				.First(_ => _.TextContent == "Add split transaction")
				.Click();
		}

		for (int i = 0; i < removedRows; i++)
		{
			cut
				.FindAll("button")
				.First(_ => _.TextContent == "Remove")
				.Click();
		}

		var forms = cut.FindAll("form");
		Assert.AreEqual(totalRows, forms.Count);

		var titles = cut.FindAll("h3");
		Assert.AreEqual(totalRows, titles.Count);
		for (int i = 1; i <= totalRows; ++i)
		{
			var expectedText = $"Split {i} / {totalRows}";
			Assert.IsNotNull(titles.FirstOrDefault(_ => _.TextContent == expectedText));
		}

		var buttons = cut.FindAll("button");
		Assert.AreEqual(totalRows + 2, buttons.Count);
		Assert.AreEqual(totalRows, buttons.Count(_ => _.TextContent == "Remove"));
	}

	[TestMethod]
	public async Task SubmittingSingleTransactionInvokesDispatcher()
	{
		Guid sourceAccountId = Guid.NewGuid();
		Guid destinationAccountId = Guid.NewGuid();
		Guid transactionTypeId = TransactionConstants.Deposit;
		_stateStore.SetState(AccountListStateHelpers.CreateAccountListState(
			new AccountListItemDto { Id = sourceAccountId, AccountTypeId = AccountConstants.Revenue, CurrencyId = Currency.NZD },
			new AccountListItemDto { Id = destinationAccountId, AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD }));
		_stateStore.SetState(LookupStateHelpers.CreateStandardState());
		_stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

		_clientNavigationManager
			.Setup(_ => _
				.NavigateTo(Routes.Root))
			.Verifiable();

		_dispatcher
			.Setup(_ => _.Dispatch<TransactionCreateCommand, TransactionCreateCommandResponse>(
				It.IsAny<TransactionCreateCommand>(),
				It.IsAny<CancellationToken>()))
			.Returns((TransactionCreateCommand command, CancellationToken cancellationToken) =>
			{
				Assert.AreEqual(transactionTypeId, command.TransactionTypeId);
				Assert.IsTrue(string.IsNullOrEmpty(command.Description));
				Assert.AreEqual(1, command.CreateTransactionDtos.Count);

				Assert.AreEqual(sourceAccountId, command.CreateTransactionDtos[0].SourceAccountId);
				Assert.AreEqual(destinationAccountId, command.CreateTransactionDtos[0].DestinationAccountId);
				Assert.AreEqual(1235600, command.CreateTransactionDtos[0].Amount);
				Assert.AreEqual(Currency.NZD, command.CreateTransactionDtos[0].CurrencyId);

				return Task.FromResult(new TransactionCreateCommandResponse
				{
					Success = true
				});
			})
			.Verifiable();

		var cut = RenderComponent<AddTransactionPage>(_ => _
			.Add(_ => _.Type, transactionTypeId));

		_viewModel.AddTransactionFormViewModels[0].Model.SourceAccountId = sourceAccountId;
		_viewModel.AddTransactionFormViewModels[0].Model.DestinationAccountId = destinationAccountId;
		_viewModel.AddTransactionFormViewModels[0].Model.Amount = 123.56M;

		await cut
			.FindAll("button")
			.First(_ => _.TextContent == "Submit")
			.ClickAsync(new MouseEventArgs());

		_clientNavigationManager
			.Verify(
				_ => _
					.NavigateTo(Routes.Root),
				Times.Once);

		_dispatcher
			.Verify(_ =>
				_.Dispatch<TransactionCreateCommand, TransactionCreateCommandResponse>(
					It.IsAny<TransactionCreateCommand>(),
					It.IsAny<CancellationToken>()),
				Times.Once);
	}
}
