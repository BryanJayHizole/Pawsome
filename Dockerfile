# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ../*.sln .             # Adjust path to locate the .sln file outside the Pawsome folder
COPY ./*.csproj ./           # Adjust path for csproj files

# Restore dependencies
RUN dotnet restore Pawsome.csproj

# Copy remaining files and build the app
COPY . .                    # Copy everything under Pawsome
WORKDIR /src                # Set working directory
RUN dotnet publish -c Release -o /app

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .    # Copy published output from the build stage
ENTRYPOINT ["dotnet", "libwkhtmltox.dll"]  # Replace 'libwkhtmltox.dll' with your app's DLL name
