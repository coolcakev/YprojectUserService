# Use the .NET 8 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

ENV ASPNETCORE_ENVIRONMENT=Production

# Build the application in Release mode
RUN dotnet publish -c Release -o /app/publish

# Use the .NET 8 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the environment to Production to use the correct appsettings

# Set the working directory inside the runtime container
WORKDIR /app

# Copy the published application files from the build stage
COPY --from=build /app/publish .


# Expose the port the app will listen on
EXPOSE 5001

# Set the entry point for the application
ENTRYPOINT ["dotnet", "usdtasia.dll"]