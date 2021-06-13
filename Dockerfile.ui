FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
WORKDIR /source

COPY . .

# Change the Directory
WORKDIR /source/

# aspnet-core
RUN dotnet restore src/EvenTransit.UI/EvenTransit.UI.csproj
RUN dotnet publish src/EvenTransit.UI/EvenTransit.UI.csproj --output /eventransitui/ --configuration Release

## Runtime
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# Change the Directory
WORKDIR /eventransitui

COPY --from=builder /eventransitui .
ENTRYPOINT ["dotnet", "EvenTransit.UI.dll"]