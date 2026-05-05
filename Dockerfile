# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["KnockProject.API/KnockProject.API.csproj", "KnockProject.API/"]
COPY ["KnockProject.Core/KnockProject.Core.csproj", "KnockProject.Core/"]
COPY ["KnockProject.Infrastructure/KnockProject.Infrastructure.csproj", "KnockProject.Infrastructure/"]
RUN dotnet restore "KnockProject.API/KnockProject.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/KnockProject.API"
RUN dotnet build "KnockProject.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "KnockProject.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment variables for production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "KnockProject.API.dll"]
