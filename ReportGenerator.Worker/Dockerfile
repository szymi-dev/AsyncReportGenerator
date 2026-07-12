FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ReportGenerator.Worker/ReportGenerator.Worker.csproj", "ReportGenerator.Worker/"]
COPY ["ReportGenerator.Application/ReportGenerator.Application.csproj", "ReportGenerator.Application/"]
COPY ["ReportGenerator.Domain/ReportGenerator.Domain.csproj", "ReportGenerator.Domain/"]
COPY ["ReportGenerator.Infrastructure/ReportGenerator.Infrastructure.csproj", "ReportGenerator.Infrastructure/"]

RUN dotnet restore "ReportGenerator.Worker/ReportGenerator.Worker.csproj"

COPY . .
WORKDIR "/src/ReportGenerator.Worker"
RUN dotnet build "ReportGenerator.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportGenerator.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportGenerator.Worker.dll"]