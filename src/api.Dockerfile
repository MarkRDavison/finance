FROM mcr.microsoft.com/dotnet/sdk:7.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore
RUN dotnet publish -c Release -o out api/mark.davison.finance.api/mark.davison.finance.api.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "mark.davison.finance.api.dll"]