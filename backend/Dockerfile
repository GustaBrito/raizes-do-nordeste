FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar dependências (camada cacheável)
COPY src/RaizesDoNordeste.Dominio/RaizesDoNordeste.Dominio.csproj src/RaizesDoNordeste.Dominio/
COPY src/RaizesDoNordeste.Aplicacao/RaizesDoNordeste.Aplicacao.csproj src/RaizesDoNordeste.Aplicacao/
COPY src/RaizesDoNordeste.Infraestrutura/RaizesDoNordeste.Infraestrutura.csproj src/RaizesDoNordeste.Infraestrutura/
COPY src/RaizesDoNordeste.API/RaizesDoNordeste.API.csproj src/RaizesDoNordeste.API/

RUN dotnet restore src/RaizesDoNordeste.API/RaizesDoNordeste.API.csproj

# Copiar e publicar
COPY src/ src/
RUN dotnet publish src/RaizesDoNordeste.API/RaizesDoNordeste.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Imagem de runtime (menor)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Railway injeta PORT automaticamente — CMD usa shell para expandir a variável
EXPOSE 8080
CMD ["sh", "-c", "dotnet RaizesDoNordeste.API.dll --urls http://+:${PORT:-8080}"]
