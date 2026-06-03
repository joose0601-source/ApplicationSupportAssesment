# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar primero usando solo los .csproj (mejor cacheo de capas).
COPY src/MercadoVerde.Domain/MercadoVerde.Domain.csproj src/MercadoVerde.Domain/
COPY src/MercadoVerde.Application/MercadoVerde.Application.csproj src/MercadoVerde.Application/
COPY src/MercadoVerde.Infrastructure/MercadoVerde.Infrastructure.csproj src/MercadoVerde.Infrastructure/
COPY src/MercadoVerde.WebApi/MercadoVerde.WebApi.csproj src/MercadoVerde.WebApi/
RUN dotnet restore src/MercadoVerde.WebApi/MercadoVerde.WebApi.csproj

# Copiar el resto del código y publicar.
COPY src/ src/
RUN dotnet publish src/MercadoVerde.WebApi/MercadoVerde.WebApi.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Base de datos de zonas horarias (para que DateTime.Now use la zona del servidor,
# definida con TZ en docker-compose). El servidor de producción es UTC-6.
RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

# Kestrel escucha en 8080 dentro del contenedor.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MercadoVerde.WebApi.dll"]
