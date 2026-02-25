FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy the rest of the source code
COPY . .
RUN dotnet restore "src/YACTR.Api/YACTR.Api.csproj"
RUN dotnet build "src/YACTR.Api/YACTR.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "src/YACTR.Api/YACTR.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN ls -la /

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runner
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Expose port 8080 (you can change this if your app uses a different port)
EXPOSE 8080

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "YACTR.Api.dll"]
