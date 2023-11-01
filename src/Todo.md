https://mudblazor.com/components/grid#grid-builder
https://mudblazor.com/features/breakpoints#breakpoints

Make sure breakpoints work well.
 - The sidebar needs to collapse when we hit the first one, i.e. on view transaction page half width goes to full, sidebar should collapse then

In web features/web ui we have commands that wrap bff/api commands, so we dispatch web command and in the handler we call the clienthttprepository and submit the api command.  This seems wasteful???

numeric/decimal input not triggering form validation

Replace guids in urls with snippets/words?

Rename shared command/query projects to shared/remove bff reference

Review uses if IRepository and verify if we need to grab it from the IServiceScopeFactory, or whether the cqrs pipeline service provider will work

create helper class for RepositoryPredicate<T>, RepositoryInclude<T> etc

Account > Account Role

CreateTransaction > EditTransaction ???

Using state helpers, if you are going to be querying, set an in progress flag?  So the ui can show loading
 - example bad scenario, create transaction, populates local cache with 1 item, open that account, shows just the one transaction while it loads the rest
 - maybe desired but check ...

Audit log when we share accounts/whatever between people???

IStateHelper.FetchAccountList(bool showActive) -> Need some config value for showing active etc???

https://github.com/firefly-iii/firefly-iii/blob/main/database/migrations/2020_06_07_063612_changes_for_v530.php
https://github.com/firefly-iii/firefly-iii/blob/main/database/migrations/2021_08_28_073733_user_groups.php

Dont validate audience/authority in integration tests, appsettings option?


Per user options
	-	Default currency
	-	Edit User object
	-	Home screen/dashboard layout
	-	Per account options?  What summaries to show?
	-	First day of the month/week

https://playwright.dev/dotnet/docs/intro

https://github.com/firefly-iii/firefly-iii/blob/main/config/firefly.php

dotnet test --filter "TestCategory!=UI"


docker build -t zeno15/zeno-finance-api -f .\api.Dockerfile .
docker build -t zeno15/zeno-finance-bff -f .\bff.Dockerfile .
docker build -t zeno15/zeno-finance-web -f .\web.Dockerfile .
docker build -t zeno15/zeno-finance-e2e -f .\e2e.Dockerfile .

charts:
https://github.com/apexcharts/Blazor-ApexCharts
^^ so long as you can make thinner line graphs etc


https://blazor.radzen.com/
https://docs.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/?view=aspnetcore-6.0#load-a-script-from-an-external-javascript-file-js-collocated-with-a-component
https://antblazor.com/en-US/charts/introduce

signalR/websocket to notify when state cache needs to be busted

zui: date range control, given two DateOnly(?) etc
zui: tag input on blur should submit in progress tag
zui: auto complete on tag input
	-	https://stackoverflow.com/questions/33985130/create-a-custom-autocomplete-list-for-an-input-field

# Migrations

Still need to create another ci stage that starts with a blank database and applies all migrations one by one, do this for sqlite and postgres, make this something in the cicd repo

dotnet ef migrations add RemoveBank --project .\api\mark.davison.finance.migrations.sqlite\mark.davison.finance.migrations.sqlite.csproj
dotnet ef migrations add RemoveBank --project .\api\mark.davison.finance.migrations.postgresql\mark.davison.finance.migrations.postgresql.csproj