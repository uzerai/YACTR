# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["src/YACTR/YACTR.csproj", "src/YACTR/"]
RUN dotnet restore "src/YACTR/YACTR.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/YACTR"
RUN dotnet build "YACTR.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "YACTR.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser

# Copy the published application
COPY --from=publish /app/publish .

# Change ownership to the non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Expose port 8080 (you can change this if your app uses a different port)
EXPOSE 8080

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "YACTR.dll"]
