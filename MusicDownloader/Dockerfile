FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MusicDownloader/MusicDownloader.csproj", "MusicDownloader/"]
RUN dotnet restore "MusicDownloader/MusicDownloader.csproj"
COPY . .
WORKDIR "/src/MusicDownloader"
RUN dotnet build "MusicDownloader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MusicDownloader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicDownloader.dll"]
