﻿global using mark.davison.common.client.abstractions.CQRS;
global using mark.davison.common.client.abstractions.Repository;
global using mark.davison.common.client.abstractions.State;
global using mark.davison.common.CQRS;
global using mark.davison.finance.models.dtos.Commands.CreateCategory;
global using mark.davison.finance.models.dtos.Commands.CreateTag;
global using mark.davison.finance.models.dtos.Commands.CreateTransaction;
global using mark.davison.finance.models.dtos.Commands.UpsertAccount;
global using mark.davison.finance.models.dtos.Queries.AccountDashboardSummaryQuery;
global using mark.davison.finance.models.dtos.Queries.AccountListQuery;
global using mark.davison.finance.models.dtos.Queries.CategoryListQuery;
global using mark.davison.finance.models.dtos.Queries.StartupQuery;
global using mark.davison.finance.models.dtos.Queries.TagListQuery;
global using mark.davison.finance.models.dtos.Queries.TransactionByAccountQuery;
global using mark.davison.finance.models.dtos.Shared;
global using mark.davison.finance.web.features.Account;
global using mark.davison.finance.web.features.Account.List;
global using mark.davison.finance.web.features.Category;
global using mark.davison.finance.web.features.Category.Fetch;
global using mark.davison.finance.web.features.Dashboard;
global using mark.davison.finance.web.features.Dashboard.QueryAccountSummary;
global using mark.davison.finance.web.features.Tag;
global using mark.davison.finance.web.features.Tag.Fetch;
global using mark.davison.finance.web.features.Transaction.QueryByAccount;
global using mark.davison.finance.web.services.AppContext;