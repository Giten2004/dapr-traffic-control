ARG NET_IMAGE=6.0-bullseye-slim
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
WORKDIR /app
# Expose ports
EXPOSE 6001/tcp
ENV ASPNETCORE_URLS http://*:6001


FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR /src

# Copy necessary files and restore as distinct layer
COPY src/FineCollectionService/FineCollectionService.csproj ./src/FineCollectionService/
RUN dotnet restore "src/FineCollectionService/FineCollectionService.csproj"

# Copy everything else and build
COPY . ./

WORKDIR "/src/src/FineCollectionService/"
RUN dotnet publish -c Release -o out FineCollectionService.csproj

# Build runtime image
FROM build AS publish
RUN dotnet publish --no-restore "FineCollectionService.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FineCollectionService.dll"]
