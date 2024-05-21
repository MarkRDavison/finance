namespace mark.davison.finance.models.EntityConfiguration;

public partial class TransactionTypeEntityConfiguration : FinanceEntityConfiguration<TransactionType>
{
    public override void ConfigureEntity(EntityTypeBuilder<TransactionType> builder)
    {
        builder
            .Property(_ => _.Type)
            .HasMaxLength(50);

        builder.HasData(
            new TransactionType { Id = TransactionConstants.Withdrawal, Type = "Withdrawal", UserId = Guid.Empty },
            new TransactionType { Id = TransactionConstants.Deposit, Type = "Deposit", UserId = Guid.Empty },
            new TransactionType { Id = TransactionConstants.Transfer, Type = "Transfer", UserId = Guid.Empty },
            new TransactionType { Id = TransactionConstants.OpeningBalance, Type = "Opening balance", UserId = Guid.Empty },
            new TransactionType { Id = TransactionConstants.Reconciliation, Type = "Reconciliation", UserId = Guid.Empty },
            new TransactionType { Id = TransactionConstants.Invalid, Type = "Invalid", UserId = Guid.Empty },
            new TransactionType { Id = TransactionConstants.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty }
        );
    }
}

