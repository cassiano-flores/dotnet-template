# ==========================================
# BUILD
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY api/ ./api/
WORKDIR /src/api/DotnetTemplate
RUN dotnet restore
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==========================================
# RUNTIME
# ==========================================
WORKDIR /app

# Instala o runtime ASP.NET Core 10.0 (a imagem só vem com .NET 8)
RUN apt-get update && apt-get install -y wget \
    && wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh \
    && chmod +x /tmp/dotnet-install.sh \
    && /tmp/dotnet-install.sh --channel 10.0 --runtime aspnetcore --install-dir /usr/share/dotnet \
    && rm /tmp/dotnet-install.sh

COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "DotnetTemplate.dll"]