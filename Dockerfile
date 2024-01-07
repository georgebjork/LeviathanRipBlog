# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Create a user 'appuser' with specific UID and GID
RUN groupadd -r -g 1001 appuser && useradd -r -u 1001 -g appuser appuser
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LeviathanRipBlog.csproj", "./"]
RUN dotnet restore "LeviathanRipBlog.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "LeviathanRipBlog.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish Image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LeviathanRipBlog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeviathanRipBlog.dll"]
