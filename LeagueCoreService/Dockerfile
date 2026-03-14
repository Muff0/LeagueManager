# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LeagueCoreService/LeagueCoreService.csproj", "LeagueCoreService/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["OGS/OGS.csproj", "OGS/"]
COPY ["Mail/Mail.csproj", "Mail/"]
COPY ["Discord/Discord.csproj", "Discord/"]
COPY ["LeagoService/LeagoService.csproj", "LeagoService/"]
RUN dotnet restore "./LeagueCoreService/LeagueCoreService.csproj"
COPY . .
WORKDIR "/src/LeagueCoreService"
RUN dotnet build "./LeagueCoreService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LeagueCoreService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeagueCoreService.dll"]
