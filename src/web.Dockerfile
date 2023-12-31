FROM mcr.microsoft.com/dotnet/sdk:7.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish/ web/mark.davison.finance.web.ui/mark.davison.finance.web.ui.csproj

FROM nginx:alpine AS FINAL
WORKDIR /usr/share/nginx/html
COPY --from=BUILD /app/publish/wwwroot .
COPY entry.sh /usr/share/nginx/html/entry.sh
COPY nginx.conf /etc/nginx/nginx.conf

RUN chmod +x /usr/share/nginx/html/entry.sh

WORKDIR /usr/share/nginx/html

CMD ["sh", "entry.sh"]