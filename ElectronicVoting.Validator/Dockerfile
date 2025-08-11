FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectronicVoting.Validator/ElectronicVoting.Validator.csproj", "ElectronicVoting.Validator/"]
COPY ["ElectronicVoting.Validator.Infrastructure/ElectronicVoting.Validator.Infrastructure.csproj", "ElectronicVoting.Validator.Infrastructure/"]
COPY ["ElectronicVoting.Validator.Domain/ElectronicVoting.Validator.Domain.csproj", "ElectronicVoting.Validator.Domain/"]
RUN dotnet restore "ElectronicVoting.Validator/ElectronicVoting.Validator.csproj"
COPY . .
WORKDIR "/src/ElectronicVoting.Validator"
RUN dotnet build "./ElectronicVoting.Validator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ElectronicVoting.Validator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectronicVoting.Validator.dll"]
