# Use Microsoft ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy solution and restore as distinct layers
COPY ["appsettings.local.json", "."]
COPY ["LibraryRestApi/LibraryRestApi.csproj", "LibraryRestApi/"]
COPY ["LibrarySystemModels/LibrarySystemModels.csproj", "LibrarySystemModels/"]
RUN dotnet restore "LibraryRestApi/LibraryRestApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/LibraryRestApi"
RUN dotnet publish -c Release -o /app/publish

# App image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Tell ASP.NET to listen on all network interfaces
ENV ASPNETCORE_URLS="http://+:5000"
# If you want only HTTP, set: ENV ASPNETCORE_URLS="http://+:5000"

ENTRYPOINT ["dotnet", "LibraryRestApi.dll"]
