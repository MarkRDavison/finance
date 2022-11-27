FROM mcr.microsoft.com/dotnet/sdk:7.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore
RUN dotnet publish -c Release -o out bff/mark.davison.finance.bff/mark.davison.finance.bff.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "mark.davison.finance.bff.dll"]