# syntax=docker/dockerfile:1

# ============================================================
# Stage 1 — restore
# Separate restore step so NuGet packages are cached as their
# own layer and only re-downloaded when *.csproj / *.props
# files change.
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore

WORKDIR /src

# Copy the files that define the dependency graph first.
# Docker will cache this layer until any of these files change.
COPY Directory.Build.props        ./
COPY Directory.Packages.props     ./
COPY global.json                  ./
COPY Money2WordCore.sln           ./
COPY Money2Word/Money2Word.csproj ./Money2Word/

# Restore only the web project (the tests are not needed at runtime).
RUN dotnet restore Money2Word/Money2Word.csproj

# ============================================================
# Stage 2 — publish
# Builds and publishes a self-contained Release artifact.
# ============================================================
FROM restore AS publish

# Copy the rest of the source that the web project needs.
COPY Money2Word/ ./Money2Word/

RUN dotnet publish Money2Word/Money2Word.csproj \
        -c Release \
        --no-restore \
        -o /app/publish

# ============================================================
# Stage 3 — runtime (final image)
# Uses the smaller ASP.NET runtime image — no SDK included.
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Create and switch to a non-root user (uid 1000) for security.
RUN addgroup --system --gid 1000 app \
 && adduser  --system --uid 1000 --ingroup app --no-create-home app

WORKDIR /app

# Copy only the published output from the publish stage.
COPY --from=publish --chown=app:app /app/publish ./

# ASP.NET Core 10 defaults to port 8080 in container environments.
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "Money2Word.dll"]
