# 🏥 Desafio Técnico Achè - Integração SAP S/4HANA

<div align="center">

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![SAP](https://img.shields.io/badge/SAP-S%2F4HANA-0FAAFF?logo=sap)

**API REST para integração com o módulo SD (Sales & Distribution) do SAP S/4HANA**

[Funcionalidades](#-funcionalidades) •
[Tecnologias](#-tecnologias-utilizadas) •
[Arquitetura](#-arquitetura) •
[Como Usar](#-como-usar) •
[Segurança](#-segurança-owasp) •
[Contribuir](#-como-contribuir)

</div>

---

## 📋 Sobre o Projeto

Esta API foi desenvolvida como solução de integração para a **Aché Laboratórios Farmacêuticos**, uma das maiores indústrias farmacêuticas do Brasil, implementando comunicação robusta e segura com o SAP S/4HANA para gestão de pedidos.

O projeto demonstra uma arquitetura enterprise-grade aplicada ao contexto farmacêutico, integrando processos de vendas e distribuição (módulo SD) do SAP com sistemas externos, respeitando as normas e regulamentações do setor.

### 🎯 Objetivos

- ✅ Integração com SAP S/4HANA via OData/REST (API_SALES_ORDER_SRV)
- ✅ Criação e consulta de pedidos
- ✅ Arquitetura limpa, escalável e testável
- ✅ Segurança implementada seguindo OWASP API Security Top 10

---

## ✨ Funcionalidades

### 📦 Gestão de Pedidos

- **Criar Pedido**: Integração completa com SAP para criação de novos pedidos
- **Consultar Pedido**: Busca de pedidos por número do documento
- **Validação Automática**: Validação de dados com FluentValidation
- **Rastreamento**: Logs detalhados de todas as operações

### 🔐 Segurança Empresarial

- **Autenticação por API Key**: Sistema de autenticação robusto
- **Rate Limiting**: Proteção contra abuso (30 req/min, 500 req/hora)
- **Headers de Segurança**: Implementação de headers recomendados pela OWASP
- **Validação de Entrada**: Proteção contra injeção e ataques

### 📊 Observabilidade

- **Logging Estruturado**: Logs detalhados de todas as operações
- **Health Check**: Endpoint para monitoramento da saúde da aplicação
- **Swagger/OpenAPI**: Documentação interativa completa

---

## 🛠️ Tecnologias Utilizadas

### Core Technologies

<div align="center">

| Tecnologia | Versão | Uso |
|:----------:|:------:|:---:|
| ![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet) | 8.0 | Framework principal |
| ![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp) | 12.0 | Linguagem de programação |
| ![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?logo=dotnet) | 8.0 | Web API |
| ![SAP](https://img.shields.io/badge/SAP-S%2F4HANA-0FAAFF?logo=sap) | - | Sistema ERP |

</div>

### Bibliotecas e Frameworks

- **MediatR** (12.x) - Implementação do padrão CQRS
- **FluentValidation** (11.x) - Validação de dados
- **AspNetCoreRateLimit** (5.x) - Rate limiting e throttling
- **Swashbuckle** (6.x) - Documentação OpenAPI/Swagger

### Patterns & Practices

```
✓ Clean Architecture          ✓ CQRS Pattern
✓ Domain-Driven Design (DDD)  ✓ Repository Pattern
✓ SOLID Principles            ✓ Dependency Injection
✓ Value Objects               ✓ Validation Pattern
```

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e **DDD**, organizados em 4 camadas:

```
┌─────────────────────────────────────────────────────────────┐
│                     🌐 API Layer                            │
│  Controllers, Middlewares, Filters, Program.cs              │
├─────────────────────────────────────────────────────────────┤
│                 📋 Application Layer                        │
│  Commands, Queries, DTOs, Validators, Behaviors             │
├─────────────────────────────────────────────────────────────┤
│                   💼 Domain Layer                           │
│  Entities, Value Objects, Enums, Interfaces, Rules          │
├─────────────────────────────────────────────────────────────┤
│                 🔧 Infrastructure Layer                     │
│  Repositories, SAP Integration, External Services           │
└─────────────────────────────────────────────────────────────┘
```

### 📁 Estrutura de Pastas

```
DesafioTecnico_Ache_DDD_CQRS/
├── 📂 src/
│   ├── 🎯 DesafioTecnico_Ache.API/
│   │   ├── Controllers/          # Endpoints REST
│   │   ├── Middleware/           # Autenticação, Logging, OWASP
│   │   └── Program.cs            # Configuração da aplicação
│   │
│   ├── 📋 DesafioTecnico_Ache.Application/
│   │   ├── Commands/             # Comandos CQRS (escrita)
│   │   ├── Queries/              # Queries CQRS (leitura)
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Validators/           # Validações FluentValidation
│   │   └── Behaviors/            # Pipeline behaviors
│   │
│   ├── 💼 DesafioTecnico_Ache.Domain/
│   │   ├── Entities/             # Entidades de domínio
│   │   ├── ValueObjects/         # Value Objects DDD
│   │   ├── Enums/                # Enumeradores
│   │   ├── Interfaces/           # Contratos do domínio
│   │   └── Common/               # Classes base
│   │
│   └── 🔧 DesafioTecnico_Ache.Infrastructure/
│       ├── Repositories/         # Implementação de repositórios
│       └── SAP/                  # Serviços de integração SAP
│           └── Services/
│
└── 📄 README.md                  # Este arquivo
```

---

## 🚀 Como Usar

### 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- IDE recomendada: [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- Acesso ao SAP S/4HANA (para ambiente de produção)

### 📥 Instalação

1. **Clone o repositório**
   ```bash
   git clone https://github.com/RenanMunizDev/DesafioTecnico_Ache_DDD_CQRS.git
   cd DesafioTecnico_Ache_DDD_CQRS
   ```

2. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

3. **Configure o `appsettings.json`**
   
   Edite o arquivo `src/DesafioTecnico_Ache.API/appsettings.json`:

   ```json
   {
     "Authentication": {
       "ApiKeys": [
         "sua-api-key-aqui"
       ]
     },
     "SapConfiguration": {
       "BaseUrl": "https://seu-sap-server.com/sap/opu/odata/sap/API_SALES_ORDER_SRV",
       "Username": "SEU_USUARIO_SAP",
       "Password": "SUA_SENHA_SAP",
       "ClientId": "100",
       "Language": "PT"
     }
   }
   ```

4. **Execute a aplicação**
   ```bash
   cd src/DesafioTecnico_Ache.API
   dotnet run
   ```

5. **Acesse a documentação Swagger**
   
   Abra o navegador em: `https://localhost:5001` ou `http://localhost:5000`

---

## 📖 Exemplos de Uso

### 🔑 Autenticação

Todas as requisições devem incluir o header de autenticação:

```http
X-API-Key: dev-api-key-12345678-abcd-efgh-ijkl-mnopqrstuvwx
```

### 📝 Criar Pedido de Venda

**Request:**
```http
POST /api/v1/pedidosvenda
Content-Type: application/json
X-API-Key: sua-api-key

{
  "clienteId": "0001234567",
  "nomeCliente": "Farmácia São Paulo LTDA",
  "dataPedido": "2024-01-15T10:30:00",
  "dataEntregaSolicitada": "2024-01-20",
  "enderecoEntrega": {
    "logradouro": "Av. Paulista",
    "numero": "1000",
    "bairro": "Bela Vista",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01310-100"
  },
  "organizacaoVendas": "1000",
  "canalDistribuicao": "10",
  "setor": "00",
  "moeda": "BRL",
  "itens": [
    {
      "materialId": "MAT001",
      "descricaoMaterial": "Medicamento X 500mg",
      "quantidade": 100,
      "unidadeMedida": "CX",
      "precoUnitario": 25.50,
      "centro": "CTR1",
      "deposito": "DEP1"
    }
  ]
}
```

**Response:** `201 Created`
```json
{
  "id": "abc123-def456-ghi789",
  "numeroDocumento": "0012345678",
  "clienteId": "0001234567",
  "nomeCliente": "Farmácia São Paulo LTDA",
  "dataPedido": "2024-01-15T10:30:00",
  "status": "Pendente",
  "valorTotal": 2550.00,
  "moeda": "BRL",
  "itens": [...]
}
```

### 🔍 Consultar Pedido

**Request:**
```http
GET /api/v1/pedidosvenda/0012345678
X-API-Key: sua-api-key
```

**Response:** `200 OK`
```json
{
  "id": "abc123-def456-ghi789",
  "numeroDocumento": "0012345678",
  "clienteId": "0001234567",
  "nomeCliente": "Farmácia São Paulo LTDA",
  "status": "Confirmado",
  "valorTotal": 2550.00,
  "itens": [...]
}
---

## 🔐 Segurança OWASP

Este projeto implementa as recomendações da **OWASP API Security Top 10**:

| # | Vulnerabilidade | Implementação |
|:-:|:---------------|:--------------|
| **API1** | Broken Object Level Authorization | ✅ Validação de acesso e autorização por recurso |
| **API2** | Broken Authentication | ✅ Autenticação via API Key com middleware dedicado |
| **API3** | Broken Object Property Level Authorization | ✅ DTOs controlados, sem mass assignment |
| **API4** | Unrestricted Resource Consumption | ✅ Rate Limiting (30 req/min, 500 req/hora) |
| **API8** | Security Misconfiguration | ✅ Headers de segurança, validação de entrada, tratamento de erros |

### 🛡️ Middlewares de Segurança

- **ApiKeyAuthenticationMiddleware**: Validação de API Key
- **SecurityHeadersMiddleware**: Headers de segurança (HSTS, CSP, X-Frame-Options)
- **GlobalExceptionHandlerMiddleware**: Tratamento seguro de exceções
- **RequestLoggingMiddleware**: Auditoria de requisições

---

## 🧪 Testes

### Executar Testes (quando implementados)

```bash
dotnet test
```

### Cobertura de Código (quando implementada)

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 📊 Status do Projeto

🚧 **Em Desenvolvimento Ativo**

- [x] Estrutura base do projeto (Clean Architecture + DDD)
- [x] Integração com SAP S/4HANA via OData
- [x] CQRS com MediatR
- [x] Autenticação e autorização
- [x] Rate Limiting
- [x] Validação com FluentValidation
- [x] Documentação Swagger/OpenAPI
- [x] Middlewares de segurança OWASP
- [x] Logging estruturado
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Docker/Containerização
- [ ] CI/CD Pipeline

---

## 🤝 Como Contribuir

Contribuições são bem-vindas! Siga os passos abaixo:

1. **Fork** o projeto
2. Crie uma **branch** para sua feature (`git checkout -b feature/MinhaFeature`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. **Push** para a branch (`git push origin feature/MinhaFeature`)
5. Abra um **Pull Request**

### 📝 Padrões de Código

- Siga os princípios **SOLID**
- Utilize **nomenclatura em português** para domínio de negócio
- Documente classes e métodos públicos com **XML Comments**
- Mantenha os testes atualizados
- Siga os padrões de commit: `feat:`, `fix:`, `docs:`, `refactor:`

---

## 👨‍💻 Autor

<div align="center">

**Renan Muniz**

[![GitHub](https://img.shields.io/badge/GitHub-RenanMunizDev-181717?logo=github)](https://github.com/RenanMunizDev)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Conectar-0077B5?logo=linkedin)](https://www.linkedin.com/in/renanmuniz86/)
[![Email](https://img.shields.io/badge/Email-Contato-D14836?logo=gmail)](mailto:renanmuniz@gmail.com)

*Desenvolvedor Backend | .NET | C# *

</div>

---

## 📄 Licença

Este projeto está sob a licença **MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 🙏 Agradecimentos

- **Aché Laboratórios Farmacêuticos** pelo desafio técnico
- **Comunidade .NET** pelas excelentes ferramentas e bibliotecas
---

<div align="center">

**⭐ Se este projeto foi útil para você, considere dar uma estrela!**

Desenvolvido com ❤️ e ☕ por [Renan Muniz](https://github.com/RenanMunizDev)

</div>
