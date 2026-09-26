# syntax=docker/dockerfile:1

ARG DOTNET_SDK_VERSION=11.0.100-rc.1
ARG DOTNET_ASPNET_VERSION=11.0.0-rc.1

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Restore in a separate cached layer. global.json and the central MSBuild files
# are required to reproduce the same toolchain and package graph as local/CI.
COPY --link Directory.Build.props Directory.Packages.props global.json ./
COPY --link Money2Word/Money2Word.csproj ./Money2Word/

RUN dotnet restore Money2Word/Money2Word.csproj

COPY --link Money2Word/ ./Money2Word/

RUN dotnet publish Money2Word/Money2Word.csproj \
        --configuration $BUILD_CONFIGURATION \
        --no-restore \
        /p:UseAppHost=false \
        -o /app/publish

# Keep the final image runtime-only. curl is installed for the Compose health
# check before switching to the non-root user supplied by official .NET images.
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_ASPNET_VERSION} AS final

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --link --from=build /app/publish ./

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "Money2Word.dll"]
