# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root
COPY Certificates/localhost.pfx /usr/local/share/ca-certificates/localhost.pfx
COPY Certificates/rootCA.pem /usr/local/share/ca-certificates/rootCA.crt
RUN update-ca-certificates
USER app

# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectronicVoting.Validator.API/ElectronicVoting.Validator.API.csproj", "ElectronicVoting.Validator.API/"]
COPY ["ElectronicVoting.Validator.Application/ElectronicVoting.Validator.Application.csproj", "ElectronicVoting.Validator.Application/"]
COPY ["ElectronicVoting.Validator.Infrastructure/ElectronicVoting.Validator.Infrastructure.csproj", "ElectronicVoting.Validator.Infrastructure/"]
COPY ["ElectronicVoting.Validator.Domain/ElectronicVoting.Validator.Domain.csproj", "ElectronicVoting.Validator.Domain/"]
RUN dotnet restore "./ElectronicVoting.Validator.API/ElectronicVoting.Validator.API.csproj"
COPY . .
WORKDIR "/src/ElectronicVoting.Validator.API"
RUN dotnet build "./ElectronicVoting.Validator.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ElectronicVoting.Validator.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectronicVoting.Validator.API.dll"]