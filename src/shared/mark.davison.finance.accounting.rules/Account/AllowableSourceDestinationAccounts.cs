﻿namespace mark.davison.finance.accounting.rules.Account;

public static class AllowableSourceDestinationAccounts
{
    private static IDictionary<Guid, IDictionary<Guid, HashSet<Guid>>> _allowableSourceDestinations;

    public static bool IsValidSourceAndDestinationAccount(Guid transactionType, Guid sourceAccountId, Guid destinationAccountId)
    {
        if (_allowableSourceDestinations.TryGetValue(transactionType, out var sourceDestinations))
        {
            if (sourceDestinations.TryGetValue(sourceAccountId, out var destinations))
            {
                return destinations.Contains(destinationAccountId);
            }
        }

        return false;
    }

    public static HashSet<Guid> GetSourceAccountTypes(Guid transactionTypeId)
    {
        if (!_allowableSourceDestinations.ContainsKey(transactionTypeId))
        {
            return new HashSet<Guid>();
        }

        return _allowableSourceDestinations[transactionTypeId].Keys.ToHashSet();
    }

    public static HashSet<Guid> GetDestinationAccountTypes(Guid transactionTypeId)
    {
        if (!_allowableSourceDestinations.ContainsKey(transactionTypeId))
        {
            return new HashSet<Guid>();
        }

        return _allowableSourceDestinations[transactionTypeId].Values.SelectMany(_ => _.AsEnumerable()).ToHashSet();
    }
    public static HashSet<Guid> GetDestinationAccountTypesForSource(Guid transactionTypeId, Guid sourceAccountTypeId)
    {
        if (!_allowableSourceDestinations.ContainsKey(transactionTypeId))
        {
            return new HashSet<Guid>();
        }

        return _allowableSourceDestinations[transactionTypeId][sourceAccountTypeId];
    }

    static AllowableSourceDestinationAccounts()
    {
        _allowableSourceDestinations = new Dictionary<Guid, IDictionary<Guid, HashSet<Guid>>>
        {
            {
                TransactionConstants.Withdrawal,
                new Dictionary<Guid, HashSet<Guid>>
                {
                    {
                        AccountConstants.Asset,
                        new HashSet<Guid>
                        {
                            AccountConstants.Expense,
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage,
                            AccountConstants.Cash
                        }
                    },
                    {
                        AccountConstants.Loan,
                        new HashSet<Guid>
                        {
                            AccountConstants.Expense,
                            AccountConstants.Cash
                        }
                    },
                    {
                        AccountConstants.Debt,
                        new HashSet<Guid>
                        {
                            AccountConstants.Expense,
                            AccountConstants.Cash
                        }
                    },
                    {
                        AccountConstants.Mortgage,
                        new HashSet<Guid>
                        {
                            AccountConstants.Expense,
                            AccountConstants.Cash
                        }
                    }
                }
            },
            {
                TransactionConstants.Deposit,
                new Dictionary<Guid, HashSet<Guid>>
                {
                    {
                        AccountConstants.Revenue,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset,
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    },
                    {
                        AccountConstants.Cash,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset,
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    },
                    {
                        AccountConstants.Loan,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset
                        }
                    },
                    {
                        AccountConstants.Debt,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset
                        }
                    },
                    {
                        AccountConstants.Mortgage,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset
                        }
                    }
                }
            },
            {
                TransactionConstants.Transfer,
                new Dictionary<Guid, HashSet<Guid>>
                {
                    {
                        AccountConstants.Asset,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset
                        }
                    },
                    {
                        AccountConstants.Loan,
                        new HashSet<Guid>
                        {
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    },
                    {
                        AccountConstants.Debt,
                        new HashSet<Guid>
                        {
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    },
                    {
                        AccountConstants.Mortgage,
                        new HashSet<Guid>
                        {
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    }
                }
            },
            {
                TransactionConstants.OpeningBalance,
                new Dictionary<Guid, HashSet<Guid>>
                {
                    {
                        AccountConstants.Asset,
                        new HashSet<Guid>
                        {
                            AccountConstants.InitialBalance
                        }
                    },
                    {
                        AccountConstants.Loan,
                        new HashSet<Guid>
                        {
                            AccountConstants.InitialBalance
                        }
                    },
                    {
                        AccountConstants.Debt,
                        new HashSet<Guid>
                        {
                            AccountConstants.InitialBalance
                        }
                    },
                    {
                        AccountConstants.Mortgage,
                        new HashSet<Guid>
                        {
                            AccountConstants.InitialBalance
                        }
                    },
                    {
                        AccountConstants.InitialBalance,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset,
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    }
                }
            },
            {
                TransactionConstants.Reconciliation,
                new Dictionary<Guid, HashSet<Guid>>
                {
                    {
                        AccountConstants.Reconciliation,
                        new HashSet<Guid>
                        {
                            AccountConstants.Asset
                        }
                    },
                    {
                        AccountConstants.Asset,
                        new HashSet<Guid>
                        {
                            AccountConstants.Reconciliation
                        }
                    }
                }
            },
            {
                TransactionConstants.LiabilityCredit,
                new Dictionary<Guid, HashSet<Guid>>
                {
                    {
                        AccountConstants.Loan,
                        new HashSet<Guid>
                        {
                            AccountConstants.LiabilityCredit
                        }
                    },
                    {
                        AccountConstants.Debt,
                        new HashSet<Guid>
                        {
                            AccountConstants.LiabilityCredit
                        }
                    },
                    {
                        AccountConstants.Mortgage,
                        new HashSet<Guid>
                        {
                            AccountConstants.LiabilityCredit
                        }
                    },
                    {
                        AccountConstants.LiabilityCredit,
                        new HashSet<Guid>
                        {
                            AccountConstants.Loan,
                            AccountConstants.Debt,
                            AccountConstants.Mortgage
                        }
                    }
                }
            }
        };
    }

}