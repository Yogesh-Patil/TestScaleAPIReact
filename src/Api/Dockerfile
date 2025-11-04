FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Api/*.csproj ./src/Api/
RUN dotnet restore src/Api/PerfDemo.Api.csproj
COPY src/Api/. ./src/Api/
WORKDIR /src/src/Api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","PerfDemo.Api.dll"]
