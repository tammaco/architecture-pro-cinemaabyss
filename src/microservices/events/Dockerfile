FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY EventsApi.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8082
ENTRYPOINT ["dotnet", "EventsApi.dll"]