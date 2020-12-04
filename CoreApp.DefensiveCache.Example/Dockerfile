#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CoreApp.DefensiveCache.Example/CoreApp.DefensiveCache.Example.csproj", "CoreApp.DefensiveCache.Example/"]
COPY ["CoreApp.DefensiveCache.Tests.Contracts/CoreApp.DefensiveCache.Tests.Contracts.csproj", "CoreApp.DefensiveCache.Tests.Contracts/"]
COPY ["CoreApp.DefensiveCache.Tests.Implementations/CoreApp.DefensiveCache.Tests.Implementations.csproj", "CoreApp.DefensiveCache.Tests.Implementations/"]
COPY ["CoreApp.DefensiveCache.Core/CoreApp.DefensiveCache.Core.csproj", "CoreApp.DefensiveCache.Core/"]
RUN dotnet restore "CoreApp.DefensiveCache.Example/CoreApp.DefensiveCache.Example.csproj"
COPY . .
WORKDIR "/src/CoreApp.DefensiveCache.Example"
RUN dotnet build "CoreApp.DefensiveCache.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreApp.DefensiveCache.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreApp.DefensiveCache.Example.dll"]