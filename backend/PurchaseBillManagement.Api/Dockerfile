FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PurchaseBillManagement.Api.csproj", "."]
RUN dotnet restore "PurchaseBillManagement.Api.csproj"

COPY . .
RUN dotnet publish "PurchaseBillManagement.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    --property:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PurchaseBillManagement.Api.dll"]
