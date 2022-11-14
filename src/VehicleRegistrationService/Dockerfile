ARG NET_IMAGE=6.0-bullseye-slim
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
WORKDIR /app

# Expose ports
EXPOSE 6002/tcp
ENV ASPNETCORE_URLS http://*:6002


FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR /src

# Copy necessary files and restore as distinct layer
COPY src/VehicleRegistrationService/VehicleRegistrationService.csproj ./src/VehicleRegistrationService/
RUN dotnet restore "src/VehicleRegistrationService/VehicleRegistrationService.csproj"

# Copy everything else and build
COPY . ./

WORKDIR "/src/src/VehicleRegistrationService/"
RUN dotnet build "VehicleRegistrationService.csproj" -c Release -o /app/build

# Build runtime image
FROM build AS publish
RUN dotnet publish --no-restore "VehicleRegistrationService.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Start
ENTRYPOINT ["dotnet", "VehicleRegistrationService.dll"]
