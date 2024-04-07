FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/awesome-app/awesome-app.csproj", "awesome-app/"]
RUN dotnet restore "src/awesome-app/awesome-app.csproj"
COPY . .
WORKDIR "/src/awesome-app"
RUN dotnet build "awesome-app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "awesome-app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "awesome-app.dll"]
