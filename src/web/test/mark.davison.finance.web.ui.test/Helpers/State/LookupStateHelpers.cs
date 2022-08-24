namespace mark.davison.finance.web.ui.test.Helpers.State;

public static class LookupStateHelpers
{
    public static LookupState CreateStandardState() => new LookupState(
        new BankDto[] {
            new BankDto{ Id = Bank.KiwibankId, Name = "Kiwibank" },
            new BankDto{ Id = Bank.BnzId, Name = "BNZ" }
        },
        new AccountTypeDto[] {
            new AccountTypeDto{ Id = AccountConstants.Asset, Type = "Asset" },
            new AccountTypeDto{ Id = AccountConstants.Revenue, Type = "Revenue" }
        },
        new CurrencyDto[] {
            new CurrencyDto{ Code = "NZD", DecimalPlaces = 2, Id = Currency.NZD, Name = "New Zealand Dollar", Symbol = "NZ$" },
            new CurrencyDto{ Code = "USD", DecimalPlaces = 2, Id = Currency.USD, Name = "US Dollar", Symbol = "US$" }
        },
        new TransactionTypeDto[] {
            new TransactionTypeDto { Id = TransactionConstants.Deposit, Type = "Deposit" }
        });
}
