FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia solution inteira
COPY . .

# restaura solution
RUN dotnet restore backend.sln

# publica API
RUN dotnet publish backend/backend.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "backend.dll"]
