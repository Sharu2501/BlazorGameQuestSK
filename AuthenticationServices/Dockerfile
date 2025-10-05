FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.sln ./
COPY AuthenticationServices/*.csproj AuthenticationServices/
COPY SharedModels/*.csproj SharedModels/

RUN dotnet restore AuthenticationServices/AuthenticationServices.csproj
COPY . .

RUN dotnet publish AuthenticationServices/AuthenticationServices.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationServices.dll"]