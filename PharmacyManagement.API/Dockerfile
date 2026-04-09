FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY PharmacyManagement.API.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "PharmacyManagement.API.dll"]
