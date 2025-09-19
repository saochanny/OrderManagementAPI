# -----------------------------
# Stage 1: Base runtime image
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# -----------------------------
# Stage 2: Build and restore
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the csproj first to leverage Docker layer caching
COPY ["OrderManagementAPI.csproj", "./"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "OrderManagementAPI.csproj"

# Copy the rest of the source code
COPY . .

# -----------------------------
# Stage 3: Publish
# -----------------------------
FROM build AS publish
WORKDIR "/src"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "OrderManagementAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# -----------------------------
# Stage 4: Final image
# -----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagementAPI.dll"]
