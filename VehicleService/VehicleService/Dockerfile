#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["VehicleService/CarAuto.VehicleService.csproj", "VehicleService/"]
RUN dotnet nuget add source "http://host.docker.internal:8196/v3/index.json" --name "local_publish"
RUN dotnet restore "VehicleService/CarAuto.VehicleService.csproj"
COPY . .
WORKDIR "/src/VehicleService"
RUN dotnet build "CarAuto.VehicleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarAuto.VehicleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarAuto.VehicleService.dll"]