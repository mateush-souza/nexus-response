# Nexus Response API - Projeto DevOps & Cloud Computing

Este projeto contém a API Nexus Response, containerizada com Docker para a disciplina de DevOps Tools & Cloud Computing.

## 1. Pré-requisitos

-   [Docker](https://docs.docker.com/get-docker/) instalado e em execução.
-   Um terminal com suporte a PowerShell (o Terminal do Windows é recomendado).

## 2. Visão Geral da Arquitetura

A solução é composta por dois containers que se comunicam através de uma rede Docker dedicada:
-   **`nexus-api`**: O container da aplicação .NET 8.
-   **`nexus-db`**: O container do banco de dados PostgreSQL.

Os dados do banco são persistidos em um volume Docker para não serem perdidos quando o container for removido.

## 3. Instruções de Execução (via Terminal do Windows/PowerShell)

Siga os passos abaixo para construir e executar a aplicação. Todos os comandos devem ser executados no PowerShell, a partir da raiz deste projeto. Os comandos de múltiplas linhas usam o caractere de acento grave (` `) como continuação de linha.

### Passo 1: Criar a Rede Docker

Os containers precisam de uma rede comum para se comunicarem.

```powershell
docker network create nexus-network
```

### Passo 2: Iniciar o Container do Banco de Dados

Este comando irá baixar a imagem do PostgreSQL e iniciar um container.

-   **`--name nexus-db`**: Define o nome do container.
-   **`-e POSTGRES_...`**: Define as variáveis de ambiente para criar o banco, usuário e senha.
-   **`-p 5432:5432`**: Mapeia a porta do container para a porta do seu computador (útil para debug).
-   **`--network nexus-network`**: Conecta o container à rede que criamos.
-   **`-v nexus-db-data:/var/lib/postgresql/data`**: Cria e utiliza um volume para persistir os dados.
-   **`-d`**: Executa o container em modo background (desanexado).

```powershell
docker run --name nexus-db `
-e POSTGRES_DB=nexusdb `
-e POSTGRES_USER=user `
-e POSTGRES_PASSWORD=password `
-p 5432:5432 `
--network nexus-network `
-v nexus-db-data:/var/lib/postgresql/data `
-d postgres:14
```

### Passo 3: Construir a Imagem da API

Use o `Dockerfile` para construir a imagem da sua aplicação.

-   **`-t nexus-api`**: Define o nome e a tag da imagem.
-   **`.`**: Indica que o `Dockerfile` está no diretório atual.

```powershell
docker build -t nexus-api .
```

### Passo 4: Executar o Container da API

Agora, execute a API, conectando-a ao banco de dados.

-   **`--name nexus-api-container`**: Define o nome do container.
-   **`-e ASPNETCORE_ENVIRONMENT=Development`**: **Importante para desenvolvimento.** Garante que a API rode em modo de desenvolvimento, habilitando o Swagger UI para testes.
-   **`-e ConnectionStrings__DefaultConnection=...`**: **Esta é a parte mais importante.** Passamos a string de conexão como uma variável de ambiente. Note que o host é `nexus-db`, o nome do container do banco na rede Docker.
-   **`-p 8080:8080`**: Mapeia a porta exposta pelo container para a porta da sua máquina.
-   **`--network nexus-network`**: Conecta o container à mesma rede do banco.
-   **`-d`**: Executa em modo background.

```powershell
docker run --name nexus-api-container `
-e "ASPNETCORE_ENVIRONMENT=Development" `
-e "ConnectionStrings__DefaultConnection=Server=nexus-db;Port=5432;Database=nexusdb;User Id=user;Password=password;" `
-p 8080:8080 `
--network nexus-network `
-d nexus-api
```

### Passo 5: Verificar os Logs dos Containers

Para garantir que tudo subiu corretamente, verifique os logs de ambos os containers.

```powershell
# Logs do Banco de Dados
docker logs nexus-db

# Logs da API
docker logs nexus-api-container
```

A API pode levar alguns segundos para iniciar e aplicar as migrations do banco. Se tudo estiver correto, você verá logs indicando que a aplicação está ouvindo na porta 8080.

## 4. Testando o CRUD

Após a execução, a API estará disponível em `http://localhost:8080`. Você pode usar o Swagger UI em `http://localhost:8080/swagger` ou usar `curl` para testar os endpoints.

Aqui está um exemplo de como criar um novo usuário (execute no PowerShell):

```powershell
curl.exe -X POST "http://localhost:8080/api/Users" -H "Content-Type: application/json" -d '{"name": "Lucas", "email": "lucas@email.com", "password": "senha_segura", "userType": "Admin"}'
```

## 5. Como Parar e Remover o Ambiente

Para parar e limpar os recursos, execute os seguintes comandos:

```powershell
# Parar e remover os containers
docker stop nexus-api-container nexus-db
docker rm nexus-api-container nexus-db

# Remover a rede
docker network rm nexus-network

# Opcional: remover o volume do banco (CUIDADO: isso apaga todos os dados)
# docker volume rm nexus-db-data

# Opcional: remover a imagem da API
# docker rmi nexus-api
``` 