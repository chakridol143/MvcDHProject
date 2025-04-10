# Use the official .NET 9.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy everything into the build container
COPY . .

# Publish the application to the /app/publish folder
RUN dotnet publish -c Release -o /app/publish

# Use the .NET 9.0 ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app/publish .

# Expose the port your app runs on (usually 80 or 5000 for ASP.NET Core apps)
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "MvcDHProject.dll"]
