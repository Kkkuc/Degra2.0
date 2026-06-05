FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Degra2.0.sln ./
COPY WebApplication/WebApplication.csproj ./WebApplication/
RUN dotnet restore ./WebApplication/WebApplication.csproj

COPY . .
WORKDIR /src/WebApplication
RUN dotnet publish WebApplication.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication.dll"]
