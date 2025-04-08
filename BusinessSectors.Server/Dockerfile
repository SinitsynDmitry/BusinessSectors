# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js in the build container
RUN apt-get update && apt-get install -y curl
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install -y nodejs

WORKDIR /src
COPY ["BusinessSectors.Server/BusinessSectors.Server.csproj", "BusinessSectors.Server/"]
COPY ["businesssectors.client/businesssectors.client.esproj", "businesssectors.client/"]
RUN dotnet restore "./BusinessSectors.Server/BusinessSectors.Server.csproj"

# Copy all source code
COPY . .

# Build the Angular app
WORKDIR "/src/businesssectors.client"
RUN npm install
RUN npm run build -- --configuration production

# Create the wwwroot directory and copy the Angular build output
WORKDIR "/src"
RUN mkdir -p /src/BusinessSectors.Server/wwwroot
RUN cp -r /src/businesssectors.client/dist/businesssectors.client/* /src/BusinessSectors.Server/wwwroot/

# Build the .NET app
WORKDIR "/src/BusinessSectors.Server"
RUN dotnet build "./BusinessSectors.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BusinessSectors.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
# Make sure the wwwroot directory is included in the publish output
RUN mkdir -p /app/publish/wwwroot
RUN cp -r /src/BusinessSectors.Server/wwwroot/* /app/publish/wwwroot/ || true

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
# Create wwwroot directory in the final image
RUN mkdir -p /app/wwwroot
# Copy the entire publish output including wwwroot
COPY --from=publish /app/publish .
# For debugging, list the contents of directories
RUN ls -la /app
RUN ls -la /app/wwwroot || echo "wwwroot is empty or not found"
ENTRYPOINT ["dotnet", "BusinessSectors.Server.dll"]