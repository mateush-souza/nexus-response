# Estágio 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY ["nexus-response/nexus response.csproj", "nexus-response/"]
RUN dotnet restore "nexus-response/nexus response.csproj"

# Copia todo o código fonte e publica a aplicação
COPY . .
WORKDIR "/app/nexus-response"
RUN dotnet publish "nexus response.csproj" -c Release -o /app/publish

# Estágio 2: Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Cria um usuário não-root
RUN adduser --system --group appuser
USER appuser

# Define a variável de ambiente para a porta
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "nexus response.dll"] 