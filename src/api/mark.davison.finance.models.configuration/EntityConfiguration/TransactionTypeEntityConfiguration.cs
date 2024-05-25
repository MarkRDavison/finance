namespace mark.davison.finance.models.EntityConfiguration;

public partial class TransactionTypeEntityConfiguration : FinanceEntityConfiguration<TransactionType>
{
    public override void ConfigureEntity(EntityTypeBuilder<TransactionType> builder)
    {
        builder
            .Property(_ => _.Type)
            .HasMaxLength(50);

        builder.HasData(
            new TransactionType { Id = TransactionTypeConstants.Withdrawal, Type = "Withdrawal", UserId = Guid.Empty },
            new TransactionType { Id = TransactionTypeConstants.Deposit, Type = "Deposit", UserId = Guid.Empty },
            new TransactionType { Id = TransactionTypeConstants.Transfer, Type = "Transfer", UserId = Guid.Empty },
            new TransactionType { Id = TransactionTypeConstants.OpeningBalance, Type = "Opening balance", UserId = Guid.Empty },
            new TransactionType { Id = TransactionTypeConstants.Reconciliation, Type = "Reconciliation", UserId = Guid.Empty },
            new TransactionType { Id = TransactionTypeConstants.Invalid, Type = "Invalid", UserId = Guid.Empty },
            new TransactionType { Id = TransactionTypeConstants.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty }
        );
    }
}

