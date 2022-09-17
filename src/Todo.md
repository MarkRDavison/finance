https://github.com/firefly-iii/firefly-iii/blob/main/database/migrations/2020_06_07_063612_changes_for_v530.php
https://github.com/firefly-iii/firefly-iii/blob/main/database/migrations/2021_08_28_073733_user_groups.php

FinanceRepository : BaseRepository <-- the base is defined in finance, not in mark.davison.common
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

# Migrations

dotnet ef migrations add RemoveBank --project .\api\mark.davison.finance.migrations.sqlite\mark.davison.finance.migrations.sqlite.csproj
dotnet ef migrations add RemoveBank --project .\api\mark.davison.finance.migrations.postgresql\mark.davison.finance.migrations.postgresql.csproj