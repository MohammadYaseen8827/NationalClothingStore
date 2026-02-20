FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/API/NationalClothingStore.API.csproj ./src/API/
COPY src/Domain/NationalClothingStore.Domain.csproj ./src/Domain/
COPY src/Application/NationalClothingStore.Application.csproj ./src/Application/
COPY src/Infrastructure/NationalClothingStore.Infrastructure.csproj ./src/Infrastructure/
COPY src/Shared/NationalClothingStore.Shared.csproj ./src/Shared/
RUN dotnet restore "./src/API/NationalClothingStore.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR /app/src/API
RUN dotnet publish "NationalClothingStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=base /app/publish .
ENTRYPOINT ["dotnet", "NationalClothingStore.API.dll"]
