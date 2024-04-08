FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "awesome-app.sln"
RUN dotnet build "awesome-app.sln" 

FROM build AS publish
WORKDIR "/src/src/awesome.app"

RUN dotnet publish "awesome.app.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "awesome.app.dll"]
