FROM mcr.microsoft.com/dotnet/sdk:6.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore
RUN dotnet publish -c Release -o out mark.davison.finance.bff/mark.davison.finance.bff.csproj

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "mark.davison.finance.bff.dll"]