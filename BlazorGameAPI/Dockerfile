# Phase de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie des fichiers projet
COPY *.sln ./
COPY BlazorGameAPI/*.csproj ./BlazorGameAPI/
COPY SharedModels/*.csproj ./SharedModels/

# Restore
RUN dotnet restore BlazorGameAPI/BlazorGameAPI.csproj

# Copie du reste du code source
COPY . .

# Build & publish (output dans /app/publish)
RUN dotnet publish BlazorGameAPI/BlazorGameAPI.csproj -c Release -o /app/publish

# Phase runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# AspNet expose le port 80 par défaut, adapte si besoin
EXPOSE 80

# Lance l’API
ENTRYPOINT ["dotnet", "BlazorGameAPI.dll"]