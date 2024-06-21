# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official ASP.NET Core SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FintaCharts.csproj", ""]
RUN dotnet restore "./FintaCharts.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FintaCharts.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FintaCharts.csproj" -c Release -o /app/publish

# Final stage/image: Use the ASP.NET Core runtime image for smallest runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FintaCharts.dll"]
