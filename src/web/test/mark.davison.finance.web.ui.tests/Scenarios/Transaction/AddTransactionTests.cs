namespace mark.davison.finance.web.ui.tests.Scenarios.Transaction;

[TestClass]
public sealed class AddTransactionTests : LoggedInTest
{
    [DataRow(TransactionType.Deposit, AccountTestConstants.RevenueAccount1Name, AccountTestConstants.AssetAccount1Name)]
    [DataRow(TransactionType.Transfer, AccountTestConstants.AssetAccount1Name, AccountTestConstants.AssetAccount2Name)]
    [DataRow(TransactionType.Withdrawal, AccountTestConstants.AssetAccount1Name, AccountTestConstants.ExpenseAccount1Name)]
    [DataTestMethod]
    public async Task AddTransactionWorks(TransactionType transactionType, string sourceAccountName, string destinationAccountName)
    {
        var description = GetSentence();

        var createTransaction = await Dashboard.OpenQuickCreate()
            .ThenAsync(_ => _.CreateTransaction(transactionType));

        var newTransaction = await createTransaction
            .Submit(new NewTransactionInfo(
                GetSentence(),
                sourceAccountName,
                destinationAccountName,
                100.0M,
                DateOnly.FromDateTime(DateTime.Today)));

        await newTransaction.ExpectTransactionType(transactionType);
    }

    [TestMethod]
    public async Task AddSplitTransactionWorks()
    {
        var splitDescription = GetSentence();

        var transactionInfos = new List<NewTransactionInfo>
        {
            new(GetSentence(),
                AccountTestConstants.RevenueAccount1Name,
                AccountTestConstants.AssetAccount1Name,
                100.0M,
                DateOnly.FromDateTime(DateTime.Today)),
            new(GetSentence(),
                AccountTestConstants.RevenueAccount2Name,
                AccountTestConstants.AssetAccount2Name,
                100.0M,
                DateOnly.FromDateTime(DateTime.Today))
        };

        var createTransaction = await Dashboard.OpenQuickCreate()
            .ThenAsync(_ => _.CreateTransaction(TransactionType.Deposit));

        var newTransaction = await createTransaction
            .Submit(splitDescription, transactionInfos);

        await newTransaction
            .ExpectGroupDescription(splitDescription);
    }
}