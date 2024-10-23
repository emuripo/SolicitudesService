FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SolicitudesService/SolicitudesServiceAPI/SolicitudesServiceAPI.csproj", "SolicitudesServiceAPI/"]
COPY ["SolicitudesService/SolicitudesService.Core/SolicitudesService.Core.csproj", "SolicitudesService.Core/"]
COPY ["SolicitudesService/SolicitudesService.Application/SolicitudesService.Application.csproj", "SolicitudesService.Application/"]
COPY ["SolicitudesService/SolicitudesService.Infrastructure/SolicitudesService.Infrastructure.csproj", "SolicitudesService.Infrastructure/"]

RUN dotnet restore "./SolicitudesServiceAPI/SolicitudesServiceAPI.csproj"

COPY . .

WORKDIR "/src"
RUN dotnet build "SolicitudesService/SolicitudesServiceAPI/SolicitudesServiceAPI.csproj" -c Release -o /app/build
RUN dotnet publish "SolicitudesService/SolicitudesServiceAPI/SolicitudesServiceAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SolicitudesServiceAPI.dll"]