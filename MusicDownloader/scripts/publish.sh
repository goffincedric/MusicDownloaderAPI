#!/bin/bash
cd /src/MusicDownloaderAPI/MusicDownloader
# Install potential new dependencies
dotnet restore "MusicDownloader.csproj"
# Build and publish
dotnet build "MusicDownloader.csproj" -c Release -o /app/build
mkdir -p /app/publish
dotnet publish "MusicDownloader.csproj" -c Release -o /app/publish