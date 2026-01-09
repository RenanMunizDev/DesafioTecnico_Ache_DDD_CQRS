# ?? SAP S/4HANA SD Integration API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp)
![SAP](https://img.shields.io/badge/SAP-S%2F4HANA-0FAAFF?style=for-the-badge&logo=sap)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

> **API REST de integração com o módulo SD (Sales & Distribution) do SAP S/4HANA, implementando as melhores práticas de Clean Architecture, DDD e CQRS.**

---

## ?? Sobre o Projeto

Esta API REST foi desenvolvida para integração com o módulo SD (Sales & Distribution) do SAP S/4HANA utilizando o protocolo OData/REST. O projeto implementa as melhores práticas de desenvolvimento de software e segurança, incluindo:

- **Clean Architecture** - Separação clara de responsabilidades em camadas
- **Domain-Driven Design (DDD)** - Modelagem rica de domínio com Entities e Value Objects
- **CQRS Pattern** - Separação entre comandos e consultas usando MediatR
- **SOLID Principles** - Código extensível e de fácil manutenção
- **OWASP API Security Top 10** - Implementação de práticas de segurança recomendadas

### ?? Funcionalidades Principais

- ? **Criação de Pedidos de Venda** no SAP S/4HANA
- ?? **Consulta de Pedidos** por número de documento
- ?? **Autenticação via API Key** (OWASP API2)
- ?? **Rate Limiting** para proteção contra abuso (OWASP API4)
- ?? **Validação completa de dados** com FluentValidation
- ??? **Security Headers** conforme OWASP
- ?? **Logging estruturado** de todas as operações
- ?? **Documentação Swagger/OpenAPI** completa

---

## ??? Arquitetura

O projeto segue os princípios da **Clean Architecture** com 4 camadas bem definidas:

```
???????????????????????????????????????????????????
?           API Layer (Presentation)              ?
?   Controllers, Middlewares, DTOs               ?
???????????????????????????????????????????????????
                 ?
???????????????????????????????????????????????????
?        Application Layer (Use Cases)            ?
?   Commands, Queries, Handlers, Validators      ?
???????????????????????????????????????????????????
                 ?
???????????????????????????????????????????????????
?           Domain Layer (Core)                   ?
?   Entities, Value Objects, Interfaces          ?
???????????????????????????????????????????????????
                 ?
???????????????????????????????????????????????????
?      Infrastructure Layer (External)            ?
?   Repositories, SAP Services, Persistence       ?
???????????????????????????????????????????????????
```

### ?? Estrutura de Pastas

```
DesafioTecnico_Ache_DDD_CQRS/
??? src/
?   ??? DesafioTecnico_Ache.API/          # Camada de Apresentação
?   ?   ??? Controllers/                   # Endpoints REST
?   ?   ??? Middleware/                    # Middlewares customizados
?   ?   ??? Program.cs                     # Configuração da aplicação
?   ?
?   ??? DesafioTecnico_Ache.Application/  # Camada de Aplicação
?   ?   ??? Commands/                      # Comandos CQRS
?   ?   ??? Queries/                       # Consultas CQRS
?   ?   ??? Handlers/                      # Command/Query Handlers
?   ?   ??? Validators/                    # FluentValidation
?   ?   ??? DTOs/                          # Data Transfer Objects
?   ?   ??? Behaviors/                     # Pipeline Behaviors
?   ?
?   ??? DesafioTecnico_Ache.Domain/       # Camada de Domínio
?   ?   ??? Entities/                      # Entidades de Domínio
?   ?   ??? ValueObjects/                  # Value Objects
?   ?   ??? Interfaces/                    # Contratos
?   ?   ??? Enums/                         # Enumerações
?   ?   ??? Common/                        # Classes base
?   ?
?   ??? DesafioTecnico_Ache.Infrastructure/ # Camada de Infraestrutura
?       ??? Repositories/                   # Implementação de repositórios
?       ??? SAP/                           # Integração com SAP
?           ??? Services/                   # OData Service
?
??? README.md
```

---

## ??? Segurança OWASP

A API implementa as seguintes proteções do **OWASP API Security Top 10**:

| Vulnerabilidade | Implementação | Localização |
|----------------|---------------|-------------|
| **API1** - Broken Object Level Authorization | Validação de acesso e propriedade | Controllers, Handlers |
| **API2** - Broken Authentication | API Key Authentication | `ApiKeyAuthenticationMiddleware` |
| **API3** - Broken Object Property Level Authorization | DTOs controlados, sem mass assignment | Application Layer |
| **API4** - Unrestricted Resource Consumption | Rate Limiting (30/min, 500/h) | `IpRateLimitOptions` |
| **API8** - Security Misconfiguration | Security Headers, validações | `SecurityHeadersMiddleware` |

### ?? Headers de Segurança Implementados

- `X-Content-Type-Options: nosniff` - Previne MIME sniffing
- `X-Frame-Options: DENY` - Previne Clickjacking
- `Strict-Transport-Security` - Force HTTPS (HSTS)
- `Content-Security-Policy` - Previne XSS e injection
- `Referrer-Policy: no-referrer` - Controla informações de referência
- `Permissions-Policy` - Desabilita recursos desnecessários

---

## ?? Como Começar

### ?? Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- Acesso ao SAP S/4HANA (para integração real)

### ?? Instalação

1. **Clone o repositório**
```bash
git clone https://github.com/RenanMunizDev/DesafioTecnico_Ache_DDD_CQRS.git
cd DesafioTecnico_Ache_DDD_CQRS
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Configure o appsettings.json**

Edite o arquivo `src/DesafioTecnico_Ache.API/appsettings.json`:

```json
{
  "Authentication": {
    "ApiKeys": [
      "sua-api-key-aqui"
    ]
  },
  "SapConfiguration": {
    "BaseUrl": "https://seu-servidor-sap.com/sap/opu/odata/sap/API_SALES_ORDER_SRV",
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

## ?? Como Usar

### ?? Autenticação

Todas as requisições (exceto Swagger e Health Check) requerem autenticação via **API Key**.

Adicione o header em todas as requisições:
```http
X-API-Key: dev-api-key-12345678-abcd-efgh-ijkl-mnopqrstuvwx
```

### ?? Exemplos de Uso

#### 1?? Criar um Pedido de Venda

**Request:**
```http
POST /api/v1/PedidosVenda
Content-Type: application/json
X-API-Key: sua-api-key-aqui

{
  "clienteId": "0000100001",
  "tipoDocumentoVenda": "OR",
  "organizacaoVendas": "1000",
  "canalDistribuicao": "10",
  "setorAtividade": "00",
  "escritorioVendas": "1000",
  "grupoVendedores": "100",
  "itens": [
    {
      "material": "MAT-00001",
      "quantidade": 10,
      "unidadeMedida": "UN",
      "precoUnitario": 150.50
    }
  ],
  "enderecoEntrega": {
    "logradouro": "Av. Brigadeiro Faria Lima",
    "numero": "1000",
    "complemento": "Andar 10",
    "bairro": "Jardim Paulistano",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01452-000",
    "pais": "BR"
  }
}
```

**Response: 201 Created**
```json
{
  "numeroDocumento": "0010001234",
  "clienteId": "0000100001",
  "tipoDocumentoVenda": "OR",
  "dataCriacao": "2024-01-15T10:30:00Z",
  "valorTotal": 1505.00,
  "moeda": "BRL",
  "status": "Criado",
  "itens": [
    {
      "numeroItem": "000010",
      "material": "MAT-00001",
      "descricaoMaterial": "Produto Exemplo",
      "quantidade": 10,
      "precoUnitario": 150.50,
      "valorTotal": 1505.00
    }
  ]
}
```

#### 2?? Consultar um Pedido de Venda

**Request:**
```http
GET /api/v1/PedidosVenda/0010001234
X-API-Key: sua-api-key-aqui
```

**Response: 200 OK**
```json
{
  "numeroDocumento": "0010001234",
  "clienteId": "0000100001",
  "nomeCliente": "Empresa Exemplo Ltda",
  "tipoDocumentoVenda": "OR",
  "dataCriacao": "2024-01-15T10:30:00Z",
  "valorTotal": 1505.00,
  "moeda": "BRL",
  "status": "Criado"
}
```

---

## ?? Testes

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Executar testes de uma categoria específica
dotnet test --filter Category=Integration
```

---

## ??? Tecnologias Utilizadas

### Core Framework
![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23_12-239120?style=flat&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=flat&logo=dotnet&logoColor=white)

### Arquitetura & Patterns
![Clean Architecture](https://img.shields.io/badge/Clean_Architecture-000000?style=flat)
![DDD](https://img.shields.io/badge/DDD-FF6B6B?style=flat)
![CQRS](https://img.shields.io/badge/CQRS-4ECDC4?style=flat)

### Bibliotecas Principais
- **MediatR** (v12.2.0) - CQRS Pattern e Mediator
- **FluentValidation** (v11.9.0) - Validação de dados
- **AspNetCoreRateLimit** (v5.0.0) - Rate Limiting
- **Swashbuckle (Swagger)** (v6.5.0) - Documentação da API

### Integração
![SAP](https://img.shields.io/badge/SAP_S%2F4HANA-0FAAFF?style=flat&logo=sap&logoColor=white)
![OData](https://img.shields.io/badge/OData-ED8B00?style=flat)
![REST](https://img.shields.io/badge/REST_API-009688?style=flat)

### Segurança
![OWASP](https://img.shields.io/badge/OWASP_Top_10-000000?style=flat&logo=owasp&logoColor=white)
- API Key Authentication
- Rate Limiting
- Security Headers
- Input Validation

---

## ?? Status do Projeto

?? **Em desenvolvimento ativo**

### ? Funcionalidades Implementadas
- [x] Criação de pedidos de venda
- [x] Consulta de pedidos por número
- [x] Autenticação via API Key
- [x] Rate Limiting
- [x] Validação com FluentValidation
- [x] Logging estruturado
- [x] Documentação Swagger
- [x] Security Headers OWASP
- [x] Global Exception Handler
- [x] CQRS Pattern

### ?? Roadmap
- [ ] Atualização de pedidos de venda
- [ ] Cancelamento de pedidos
- [ ] Consulta com filtros avançados
- [ ] Paginação de resultados
- [ ] Cache distribuído (Redis)
- [ ] Testes unitários e de integração
- [ ] CI/CD Pipeline
- [ ] Containerização (Docker)
- [ ] Monitoramento (Application Insights)

---

## ?? Como Contribuir

Contribuições são sempre bem-vindas! Para contribuir:

1. **Fork** o projeto
2. Crie uma **branch** para sua feature (`git checkout -b feature/MinhaFeature`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. **Push** para a branch (`git push origin feature/MinhaFeature`)
5. Abra um **Pull Request**

### ?? Diretrizes de Contribuição

- Siga os padrões de código existentes
- Escreva testes para novas funcionalidades
- Atualize a documentação quando necessário
- Use commits semânticos (feat, fix, docs, refactor, etc.)
- Mantenha o código em português para consistência

---

## ?? Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## ?? Autor

**Renan Muniz**

- ?? GitHub: [@RenanMunizDev](https://github.com/RenanMunizDev)
- ?? Email: dev@ache.com.br
- ?? LinkedIn: [Renan Muniz](https://linkedin.com/in/renanmuniz)

---

## ?? Suporte

Se você tiver alguma dúvida ou problema, por favor:

1. Verifique a [documentação Swagger](https://localhost:5001)
2. Consulte as [Issues abertas](https://github.com/RenanMunizDev/DesafioTecnico_Ache_DDD_CQRS/issues)
3. Crie uma [nova Issue](https://github.com/RenanMunizDev/DesafioTecnico_Ache_DDD_CQRS/issues/new)

---

## ?? Agradecimentos

- Laboratório Aché pela oportunidade do desafio técnico
- Comunidade .NET pelo excelente ecossistema
- SAP pela documentação da API OData

---

<div align="center">

**? Se este projeto foi útil para você, considere dar uma estrela!**

Made with ?? and ? by [Renan Muniz](https://github.com/RenanMunizDev)

</div>
