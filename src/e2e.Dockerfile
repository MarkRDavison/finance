FROM mcr.microsoft.com/dotnet/sdk:6.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet build web/test/mark.davison.finance.web.ui.test/mark.davison.finance.web.ui.test.csproj -v m -o out -c Debug

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "vstest", "*.test.dll", "--logger:console;verbosity=detailed"]