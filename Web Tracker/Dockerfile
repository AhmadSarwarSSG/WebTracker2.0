#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Web Tracker/Web Tracker.csproj", "Web Tracker/"]
RUN dotnet restore "Web Tracker/Web Tracker.csproj"
COPY . .
WORKDIR "/src/Web Tracker"
RUN dotnet build "Web Tracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web Tracker.dll", "migrate"]