FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Installation de Python
RUN apt-get update && \
    apt-get install -y python3 python3-pip curl bash && \
    rm -rf /var/lib/apt/lists/*

# Copie des fichiers du projet
COPY *.sln ./
COPY BlazorGame.Client/*.csproj BlazorGame.Client/
COPY SharedModels/*.csproj SharedModels/

# Installation des outils Blazor WebAssembly
RUN dotnet workload install wasm-tools

# Restauration des dépendances
RUN dotnet restore BlazorGame.Client/BlazorGame.Client.csproj

# Copie du reste du code source
COPY . .

# Compilation et publication de l’application
RUN dotnet publish BlazorGame.Client/BlazorGame.Client.csproj -c Release -o /app/publish

# Utilisation de serveur Nginx pour héberger l'application
FROM nginx:alpine

# Copie de la config Nginx pour les routes
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copie des fichiers Blazor publiés vers le dossier web de Nginx
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Ouverture du port 80
EXPOSE 80