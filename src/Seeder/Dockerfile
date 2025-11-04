FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Seeder/*.csproj ./src/Seeder/
RUN dotnet restore src/Seeder/PerfDemo.Seed.csproj
COPY src/Seeder/. ./src/Seeder/
WORKDIR /src/src/Seeder
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","PerfDemo.Seed.dll"]
