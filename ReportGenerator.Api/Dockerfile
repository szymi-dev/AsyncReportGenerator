FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ReportGenerator.Api/ReportGenerator.Api.csproj", "ReportGenerator.Api/"]
COPY ["ReportGenerator.Application/ReportGenerator.Application.csproj", "ReportGenerator.Application/"]
COPY ["ReportGenerator.Domain/ReportGenerator.Domain.csproj", "ReportGenerator.Domain/"]
COPY ["ReportGenerator.Infrastructure/ReportGenerator.Infrastructure.csproj", "ReportGenerator.Infrastructure/"]

RUN dotnet restore "ReportGenerator.Api/ReportGenerator.Api.csproj"

COPY . .
WORKDIR "/src/ReportGenerator.Api"
RUN dotnet build "ReportGenerator.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportGenerator.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportGenerator.Api.dll"]