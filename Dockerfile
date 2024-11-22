# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY Pawsome.sln .                   # Copy the solution file
COPY Pawsome.csproj .                # Copy the project file

# Restore dependencies
RUN dotnet restore Pawsome.csproj

# Copy all files and build the app
COPY . .                             # Copy everything in the current directory
RUN dotnet publish -c Release -o /app

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .             # Copy published files from the build stage
ENTRYPOINT ["dotnet", "libwkhtmltox.dll"] # Set the app entry point
