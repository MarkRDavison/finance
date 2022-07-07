https://github.com/firefly-iii/firefly-iii/blob/main/database/migrations/2020_06_07_063612_changes_for_v530.php
https://github.com/firefly-iii/firefly-iii/blob/main/database/migrations/2021_08_28_073733_user_groups.php

Per user options
	-	Default currency
	-	Edit User object
	-	Home screen/dashboard layout

https://timewarpengineering.github.io/blazor-state/Tutorial.html
https://playwright.dev/dotnet/docs/intro


Stupid no pack type thing > Referencing bff commands from client fucks the world
Move away from using mediatR & blazor state - roll your own?
Move self written CQRS to common - not server - re-use request/response 
	-	Client > BFF > API > BFF > Client > State
CQRS - Create/recreate and use ICommand and IQuery instead of only ICommand with different naming convention



dotnet test --filter "TestCategory!=UI"


docker build -t zeno15/zeno-finance-api -f .\api.Dockerfile .
docker build -t zeno15/zeno-finance-bff -f .\bff.Dockerfile .
docker build -t zeno15/zeno-finance-web -f .\web.Dockerfile .
docker build -t zeno15/zeno-finance-e2e -f .\e2e.Dockerfile .

=========

e2e tests needs to be a container that ends, not nginx
bff redirect is doing localhost:40000 things