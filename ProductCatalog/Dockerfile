FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProductCatalog.csproj", "./"]
RUN dotnet restore "./ProductCatalog.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ProductCatalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.dll"]
