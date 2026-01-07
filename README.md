# 📦 Subscription Management — Micro Domain Project

Projeto criado para **praticar fundamentos de arquitetura de software**, com foco em:

- Domínio rico
- Value Objects (VO)
- Entidades com comportamento
- Result Pattern vs Exceptions
- Separação clara entre Domínio, Aplicação e Infraestrutura
- Entity Framework Core com PostgreSQL

---

## 🧠 Contexto do Domínio

O sistema representa um **micro-domínio de assinaturas**.

Uma empresa oferece **planos**, e um cliente pode possuir **uma única assinatura ativa por vez**.  
A assinatura possui um ciclo de vida bem definido e regras explícitas.

---

## 📐 Regras de Negócio

### Plano

1. Um plano possui:
   - Id (Guid)
   - Nome (obrigatório, não vazio)
   - Duração em dias (obrigatório, maior que 0)
2. Os planos são criados separadamente e podem ser reutilizados em múltiplas assinaturas

### Assinatura

1. Uma assinatura:
   - pertence a um plano (referência por ID)
   - possui data de início e data de término
   - possui um status
2. A data final **sempre deve ser maior** que a data inicial
3. Não pode existir **mais de uma assinatura ativa ao mesmo tempo**
4. Uma assinatura pode estar nos estados:
   - **Active** - Assinatura ativa e válida
   - **Canceled** - Assinatura cancelada manualmente
   - **Expired** - Assinatura expirada automaticamente
5. Cancelar uma assinatura:
   - altera apenas o status para `Canceled`
   - **não altera** o período da assinatura
6. Uma assinatura expira automaticamente quando a data final é ultrapassada
   - A verificação ocorre automaticamente ao consultar assinaturas

---

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas:

```
SubscriptionManagement/
├── SubscriptionManagement.Api/          # Camada de apresentação (Controllers)
├── SubscriptionManagement.Application/  # Camada de aplicação (DTOs)
├── SubscriptionManagement.Domain/       # Camada de domínio (Entidades, VOs, Exceções)
└── SubscriptionManagement.Infrastructure/ # Camada de infraestrutura (DbContext, Repositories, Services)
```

### Camadas

#### **Api**

- Controllers REST
- Endpoints HTTP
- Validação de entrada
- Tratamento de erros HTTP

#### **Application**

- DTOs (Data Transfer Objects)
- Contratos de entrada e saída

#### **Domain**

- Entidades (`Subscription`, `Plan`)
- Value Objects (`SubscriptionPeriod`)
- Exceções de domínio (`DomainException`)
- Result Pattern (`Result<T>`)
- Regras de negócio

#### **Infrastructure**

- Entity Framework Core
- Repositórios
- Serviços de aplicação
- Configuração do banco de dados

---

## 🧱 Conceitos Arquiteturais Utilizados

### 📌 Arquitetura Orientada ao Domínio

- O domínio é o centro do sistema
- Regras vivem no domínio, não em controllers ou services genéricos

### 📌 Value Objects

- Encapsulam validações
- São imutáveis
- Nunca existem em estado inválido
- Exemplo: `SubscriptionPeriod`

### 📌 Entidades

- Possuem identidade (Id)
- Possuem comportamento (métodos de domínio)
- Exemplo: `Subscription` (com métodos `Cancel()`, `Expire()`, `IsActive()`)

### 📌 Result Pattern

- Usado para erros esperados
- Define contrato explícito de sucesso ou falha
- Evita exceções vazando para camadas externas
- Implementação: `Result<T>` com `Success()` e `Failure()`

### 📌 Exceptions de Domínio

- Representam violações de regra
- São semânticas
- Não conhecem HTTP, UI ou infraestrutura
- Implementação: `DomainException` genérica

---

## 🧩 Modelagem do Domínio

### Value Objects

#### **SubscriptionPeriod**

Responsável por:

- Data de início (`StartDate`)
- Data de término (`EndDate`)
- Garantir que `startDate < endDate`

> Se inválido → lança `DomainException`

---

### Entidades

#### **Plan**

Representa um plano disponível:

- `Id` (Guid)
- `Name` (string, privado, setter privado)
- `DurationInDays` (int, privado, setter privado)

Validações:

- Nome não pode ser vazio
- Duração deve ser maior que 0

> A duração do plano define o período da assinatura quando ela é criada.

---

#### **Subscription**

Campos:

- `Id` (Guid)
- `Plan` (referência à entidade Plan)
- `Period` (Value Object SubscriptionPeriod)
- `Status` (enum SubscriptionStatus)

Comportamentos:

- `Cancel()` - Altera status para `Canceled`
- `Expire()` - Altera status para `Expired`
- `IsActive()` - Verifica se a assinatura está ativa

> A entidade **possui comportamento**, não é apenas um conjunto de dados.

---

#### **SubscriptionStatus**

Enum com os estados possíveis:

- `Active = 1` - Assinatura ativa
- `Canceled = 2` - Assinatura cancelada
- `Expired = 3` - Assinatura expirada

---

## 🚨 Exceptions de Domínio

O projeto utiliza uma exceção genérica `DomainException` para todas as violações de regra de negócio:

- Validações de `SubscriptionPeriod`
- Validações de `Plan`
- Outras regras de domínio

📌 Essas exceções:

- pertencem ao domínio
- não retornam status HTTP diretamente
- são capturadas nos serviços e convertidas em `Result.Failure()`

---

## 📦 Result Pattern

O sistema utiliza um `Result<T>` para comunicação entre camadas:

```csharp
Result<T>.Success(value)  // Sucesso
Result<T>.Failure(error)   // Falha com mensagem
```

Propriedades:

- `Value` - Valor em caso de sucesso
- `Error` - Mensagem de erro em caso de falha
- `IsSuccess` - Indica sucesso
- `IsFailure` - Indica falha

📌 Utilizado para:

- erros previsíveis
- regras de negócio violadas
- falhas esperadas

Exceções ficam restritas ao domínio e são convertidas em `Result.Failure()` nos serviços.

---

## 🗄️ Banco de Dados

### PostgreSQL

O projeto utiliza **PostgreSQL** como banco de dados, configurado através do Entity Framework Core.

### Configuração

A connection string deve ser configurada em `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SubscriptionManagement;Username=postgres;Password=sua_senha"
  }
}
```

### Migrations

O projeto utiliza Entity Framework Core Migrations para gerenciar o schema do banco:

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project src/SubscriptionManagement.Infrastructure --startup-project src/SubscriptionManagement.Api

# Aplicar migrations
dotnet ef database update --project src/SubscriptionManagement.Infrastructure --startup-project src/SubscriptionManagement.Api
```

---

## 🌐 API Endpoints

### Subscriptions

#### `GET /api/subscriptions`

Lista todas as assinaturas com filtros opcionais.

#### `POST /api/subscriptions`

Cria uma nova assinatura.

#### `PATCH /api/subscriptions/{id}/cancel`

Cancela uma assinatura pelo ID.

---

### Plans

#### `GET /api/plans`

Lista todos os planos cadastrados.

#### `POST /api/plans`

Cria um novo plano.

---

## 🚀 Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL instalado e rodando
- Visual Studio 2022 ou VS Code

### Passos

1. **Clone o repositório**

```bash
git clone <url-do-repositorio>
cd SubscriptionManagement
```

2. **Configure a connection string**

Edite `src/SubscriptionManagement.Api/appsettings.Development.json` com suas credenciais do PostgreSQL.

3. **Aplique as migrations**

```bash
dotnet ef database update --project src/SubscriptionManagement.Infrastructure --startup-project src/SubscriptionManagement.Api
```

4. **Execute a aplicação**

```bash
cd src/SubscriptionManagement.Api
dotnet run
```

5. **Acesse o Swagger**

Abra o navegador em `https://localhost:5001/swagger` (ou a porta configurada)

---

## 🎯 Objetivo do Projeto

Este projeto foi criado para:

- treinar modelagem de domínio
- reforçar fundamentos arquiteturais
- praticar separação de responsabilidades
- evitar domínio anêmico
- evitar services genéricos
- praticar DDD (Domain-Driven Design)
- implementar padrões como Result Pattern
- trabalhar com Entity Framework Core e PostgreSQL

---

## 📝 Tecnologias Utilizadas

- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **Npgsql.EntityFrameworkCore.PostgreSQL**
- **Swagger/OpenAPI**

---

## 📚 Estrutura de Pastas

```
SubscriptionManagement/
├── src/
│   ├── SubscriptionManagement.Api/
│   │   ├── Controllers/
│   │   │   ├── PlansController.cs
│   │   │   └── SubscriptionsController.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── SubscriptionManagement.Application/
│   │   └── DTOs/
│   │       ├── CreatePlanRequest.cs
│   │       ├── CreateSubscriptionRequest.cs
│   │       └── GetSubscriptionsRequest.cs
│   ├── SubscriptionManagement.Domain/
│   │   ├── Common/
│   │   │   └── Result.cs
│   │   ├── Exceptions/
│   │   │   └── DomainException.cs
│   │   ├── Models/
│   │   │   ├── Plan.cs
│   │   │   ├── Subscription.cs
│   │   │   └── SubscriptionStatus.cs
│   │   └── ValueObjects/
│   │       └── SubscriptionPeriod.cs
│   └── SubscriptionManagement.Infrastructure/
│       ├── Data/
│       │   └── ApplicationDbContext.cs
│       ├── Migrations/
│       ├── Repositories/
│       │   ├── PlanRepository.cs
│       │   └── SubscriptionRepository.cs
│       └── Services/
│           ├── PlanService.cs
│           └── SubscriptionService.cs
└── README.md
```

---

## 🔄 Fluxo de Criação de Assinatura

1. Cliente faz `POST /api/subscriptions` com `planId`
2. Controller chama `SubscriptionService.CreateAsync()`
3. Service verifica se já existe assinatura ativa
4. Service busca o plano pelo ID
5. Service cria `SubscriptionPeriod` com data atual e duração do plano
6. Service cria entidade `Subscription` com status `Active`
7. Repository persiste no banco
8. Service retorna `Result<Subscription>`
9. Controller retorna `200 OK` ou `400 Bad Request`

---

## 🔄 Fluxo de Expiração Automática

1. Cliente faz `GET /api/subscriptions`
2. Controller chama `SubscriptionService.GetAllAsync()`
3. Service chama `SubscriptionRepository.ExpireSubscriptionsAsync()`
4. Repository busca todas as assinaturas com status `Active`
5. Para cada assinatura, verifica se `EndDate < DateTime.UtcNow`
6. Se expirada, chama `subscription.Expire()`
7. Repository salva as mudanças
8. Service retorna lista de assinaturas (já atualizadas)

---

## 📌 Observações Importantes

- Todas as datas são armazenadas em **UTC** no banco de dados
- A expiração de assinaturas ocorre **automaticamente** ao consultar a lista
- Não é permitido criar nova assinatura se já existe uma **ativa**
- O status é armazenado como **integer** no banco (enum)
- O projeto utiliza **Dependency Injection** para gerenciar dependências
- Swagger está habilitado para documentação da API
