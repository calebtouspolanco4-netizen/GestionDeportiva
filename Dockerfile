# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# SDK build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["GestionDeportiva/GestionDeportiva.csproj", "GestionDeportiva/"]
RUN dotnet restore "GestionDeportiva/GestionDeportiva.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/GestionDeportiva"
RUN dotnet build "GestionDeportiva.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GestionDeportiva.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestionDeportiva.dll"]

