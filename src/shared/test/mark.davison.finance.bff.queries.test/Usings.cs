﻿global using mark.davison.common.Repository;
global using mark.davison.common.server.abstractions.Authentication;
global using mark.davison.common.server.abstractions.Identification;
global using mark.davison.common.server.abstractions.Repository;
global using mark.davison.finance.accounting.constants;
global using mark.davison.finance.bff.queries.Scenarios.AccountDashboardSummaryQuery;
global using mark.davison.finance.bff.queries.Scenarios.AccountListQuery;
global using mark.davison.finance.bff.queries.Scenarios.CategoryListQuery;
global using mark.davison.finance.bff.queries.Scenarios.StartupQuery;
global using mark.davison.finance.bff.queries.Scenarios.TagListQuery;
global using mark.davison.finance.bff.queries.Scenarios.TransactionByAccountQuery;
global using mark.davison.finance.models.dtos.Queries.AccountDashboardSummaryQuery;
global using mark.davison.finance.models.dtos.Queries.AccountListQuery;
global using mark.davison.finance.models.dtos.Queries.CategoryListQuery;
global using mark.davison.finance.models.dtos.Queries.StartupQuery;
global using mark.davison.finance.models.dtos.Queries.TagListQuery;
global using mark.davison.finance.models.dtos.Queries.TransactionByAccountQuery;
global using mark.davison.finance.models.Entities;
global using mark.davison.finance.models.Models;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using Remote.Linq;
global using Remote.Linq.Text.Json;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Threading;
global using System.Threading.Tasks;
