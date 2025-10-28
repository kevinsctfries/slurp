FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["UrlShortener.Api/UrlShortener.Api.csproj", "UrlShortener.Api/"]
RUN dotnet restore "UrlShortener.Api/UrlShortener.Api.csproj"
COPY . .
WORKDIR "/src/UrlShortener.Api"
RUN dotnet build "UrlShortener.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlShortener.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]