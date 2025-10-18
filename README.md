# SafeScribe - JWT Authentication API (skeleton)

Projeto gerado para o CP5: Autenticação e Autorização com JWT (ASP.NET Core Web API).

**Instruções rápidas**
- Requisitos: .NET 8 SDK
- Restaurar pacotes: `dotnet restore`
- Rodar: `dotnet run`
- Atualize `appsettings.json` -> Jwt:Secret para uma chave secreta de pelo menos 32 caracteres.

**Conteúdo**
- Endpoints:
  - POST /api/v1/auth/registrar
  - POST /api/v1/auth/login
  - POST /api/v1/auth/logout
  - CRUD /api/v1/notas (protegido)

**Observações**
- Este projeto é um esqueleto com armazenamento em memória (usuários e notas).
- Dependências: Microsoft.AspNetCore.Authentication.JwtBearer, BCrypt.Net-Next, Microsoft.EntityFrameworkCore.InMemory


**Integrantes**

Fernando Henrique Vilela Aguiar - rm557525
Gabrielly Campo Macedo - rm558962


