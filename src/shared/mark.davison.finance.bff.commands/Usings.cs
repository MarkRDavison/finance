﻿global using mark.davison.common.CQRS;
global using mark.davison.common.Repository;
global using mark.davison.common.server.abstractions.Authentication;
global using mark.davison.common.server.abstractions.CQRS;
global using mark.davison.common.server.abstractions.Repository;
global using mark.davison.finance.accounting.constants;
global using mark.davison.finance.accounting.rules.Account;
global using mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;
global using mark.davison.finance.bff.commands.Scenarios.CreateLocation;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Processors;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Validators;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Deposit;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Transfer;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Validators;
global using mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Withdrawal;
global using mark.davison.finance.models.dtos.Shared;
global using mark.davison.finance.models.Entities;
global using Microsoft.Extensions.DependencyInjection;
global using System.Diagnostics.CodeAnalysis;
