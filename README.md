# 📦 Subscription Management — Micro Domain Project

Projeto pequeno e intencional criado para **praticar fundamentos de arquitetura de software**, com foco em:

- Domínio rico
- Value Objects (VO)
- Casos de Uso (Use Cases / OC)
- Result vs Exceptions
- Separação clara entre Domínio, Aplicação e Infraestrutura

> ⚠️ Este projeto **não tem foco em UI, banco real ou framework**.  
> O objetivo é **modelagem de domínio e regras de negócio**.

---

## 🧠 Contexto do Domínio

O sistema representa um **micro-domínio de assinaturas**.

Uma empresa oferece **planos**, e um cliente pode possuir **uma única assinatura ativa por vez**.  
A assinatura possui um ciclo de vida bem definido e regras explícitas.

---

## 📐 Regras de Negócio

### Assinatura
1. Uma assinatura:
   - pertence a um plano
   - possui data de início e data de término
   - possui um status
2. A data final **sempre deve ser maior** que a data inicial
3. Não pode existir **mais de uma assinatura ativa ao mesmo tempo**
4. Uma assinatura pode estar nos estados:
   - **Active**
   - **Canceled**
   - **Expired**
5. Cancelar uma assinatura:
   - altera apenas o status
   - **não altera** o período da assinatura
6. Uma assinatura expira automaticamente quando a data final é ultrapassada

---

## 🧱 Conceitos Arquiteturais Utilizados

### 📌 Arquitetura Orientada ao Domínio
- O domínio é o centro do sistema
- Regras vivem no domínio, não em controllers ou services genéricos

### 📌 Value Objects
- Encapsulam validações
- São imutáveis
- Nunca existem em estado inválido

### 📌 Use Cases (OC)
- Orquestram o fluxo
- Não contêm regras de negócio
- Traduzem exceções do domínio em `Result`

### 📌 Result Pattern
- Usado para erros esperados
- Define contrato explícito de sucesso ou falha
- Evita exceções vazando para camadas externas

### 📌 Exceptions de Domínio
- Representam violações de regra
- São semânticas
- Não conhecem HTTP, UI ou infraestrutura

---

## 🧩 Modelagem do Domínio

### Value Objects

#### **SubscriptionPeriod**
Responsável por:
- Data de início
- Data de término
- Garantir que `startDate < endDate`

> Se inválido → lança `InvalidSubscriptionPeriodException`

---

#### **Plan**
Representa um plano disponível:
- Id
- Nome
- Duração em dias

> A duração do plano define o período da assinatura.

---

#### **SubscriptionStatus**
Estados possíveis:
- Active
- Canceled
- Expired

Pode ser implementado como:
- Enum (mais simples)
- ou Value Object (mais expressivo)

---

### Entidade

#### **Subscription**
Campos:
- Id
- Plan
- SubscriptionPeriod
- Status

Comportamentos:
- `cancel()`
- `expire()`
- `isActive()`

> A entidade **possui comportamento**, não é apenas um conjunto de dados.

---

## 🚨 Exceptions de Domínio

Exemplos de exceções:

- `InvalidSubscriptionPeriodException`
- `ActiveSubscriptionAlreadyExistsException`
- `SubscriptionAlreadyCanceledException`

📌 Essas exceções:
- pertencem ao domínio
- não retornam status HTTP
- não devem vazar para fora do caso de uso

---

## 🔁 Casos de Uso (Use Cases)

### **CreateSubscription**
Responsabilidades:
1. Receber dados primitivos
2. Criar Value Objects
3. Verificar se já existe assinatura ativa
4. Criar a entidade
5. Persistir
6. Retornar `Result`

---

### **CancelSubscription**
Responsabilidades:
1. Buscar assinatura
2. Solicitar cancelamento à entidade
3. Persistir
4. Retornar `Result`

---

### (Opcional) **ExpireSubscriptions**
- Percorre assinaturas existentes
- Expira as que ultrapassaram a data final
- Não depende de UI ou framework

---

## 📦 Result Pattern

O sistema utiliza um `Result<T>` para comunicação entre camadas:

- `Success(value)`
- `Failure(error)`

📌 Utilizado para:
- erros previsíveis
- regras de negócio violadas
- falhas esperadas

Exceções ficam restritas ao domínio.

---

## 🗂 Estrutura de Pastas (Sugestão)

