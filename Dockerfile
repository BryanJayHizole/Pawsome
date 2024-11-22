# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY *.sln .
COPY Pawsome/*.csproj ./Pawsome/

# Restore dependencies
RUN dotnet restore Pawsome/Pawsome.csproj

# Copy remaining files and build the app
COPY Pawsome/. ./Pawsome/
WORKDIR /src/Pawsome
RUN dotnet publish -c Release -o /app

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "libwkhtmltox.dll"]
