FROM mcr.microsoft.com/dotnet/sdk:8.0.414 AS build
WORKDIR /app

RUN apt-get update && \
    apt-get install -y python3 python3-pip curl bash && \
    rm -rf /var/lib/apt/lists/*

COPY *.sln ./
COPY BlazorGame.Client/*.csproj BlazorGame.Client/
COPY SharedModels/*.csproj SharedModels/

RUN dotnet workload install wasm-tools

RUN dotnet restore BlazorGame.Client/BlazorGame.Client.csproj

COPY . .

RUN dotnet publish BlazorGame.Client/BlazorGame.Client.csproj -c Release -o /app/publish

FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80