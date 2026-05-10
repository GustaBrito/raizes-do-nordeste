# Raízes do Nordeste — Plataforma de Qualidade de Software

Projeto multidisciplinar UNINTER 2026 — trilha **Qualidade de Software**.
Sistema digital integrado para a rede de franquias alimentícias **Raízes do Nordeste**, com foco em demonstração de práticas de QA: requisitos, plano de testes, métricas, LGPD e arquitetura.

> **Importante:** o objetivo deste repositório é entregar uma estratégia completa de QA com evidências de implementação. O sistema é funcional, mas alguns serviços externos (gateway de pagamento) são simulados via mock.

---

## Sumário

1. [Arquitetura](#arquitetura)
2. [Pré-requisitos](#pré-requisitos)
3. [Como executar — Backend (.NET 8)](#como-executar--backend-net-8)
4. [Como executar — Frontend (Vue 3)](#como-executar--frontend-vue-3)
5. [Como executar os testes](#como-executar-os-testes)
6. [Estrutura de pastas](#estrutura-de-pastas)
7. [Documentação principal](#documentação-principal)
8. [LGPD e Privacidade](#lgpd-e-privacidade)
9. [Declaração de uso de IA](#declaração-de-uso-de-ia)

---

## Arquitetura

- **Backend:** .NET 8 + ASP.NET Core, Clean Architecture (4 camadas: `Dominio`, `Aplicacao`, `Infraestrutura`, `API`)
- **Frontend:** Vue 3 (Composition API) + Pinia + Vue Router 4 + Vite
- **Banco de dados:** Entity Framework Core com provedor InMemory (para testes e demonstração)
- **Testes backend:** xUnit + Moq + FluentAssertions + WebApplicationFactory
- **Testes frontend:** Vitest + @vue/test-utils
- **Autenticação:** JWT Bearer + hash de senhas com bcrypt
- **Padrão de código:** 100% em português (variáveis, métodos, classes, comentários e testes)

Diagramas detalhados em [`docs/diagramas/`](docs/diagramas/).

---

## Pré-requisitos

| Ferramenta | Versão mínima | Verificar com |
|---|---|---|
| .NET SDK | 8.0 | `dotnet --version` |
| Node.js | 18.x | `node --version` |
| npm | 9.x | `npm --version` |

---

## Como executar — Backend (.NET 8)

```bash
cd backend
dotnet restore
dotnet build
cd src/RaizesDoNordeste.API
dotnet run
```

A API sobe em `https://localhost:5001` (ou `http://localhost:5000`).
Documentação Swagger disponível em `https://localhost:5001/swagger`.

### Endpoints principais

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/clientes` | Cadastrar cliente (com consentimento LGPD obrigatório) |
| `POST` | `/api/auth/login` | Autenticar cliente e obter token JWT |
| `POST` | `/api/pedidos` | Realizar pedido (requer autenticação) |
| `GET` | `/api/fidelizacao/{clienteId}` | Consultar saldo de pontos |
| `POST` | `/api/fidelizacao/resgatar` | Resgatar pontos de fidelização |

---

## Como executar — Frontend (Vue 3)

```bash
cd frontend
npm install
npm run dev
```

A aplicação abre em `http://localhost:5173`.

### Build de produção

```bash
npm run build
npm run preview
```

---

## Como executar os testes

### Backend (unitários + integração)

```bash
cd backend
dotnet test
```

Para somente uma categoria:

```bash
dotnet test --filter "Categoria=Regressao"
```

Relatório de cobertura (requer `coverlet.collector`, já incluído nos `.csproj`):

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Frontend (Vitest)

```bash
cd frontend
npm test
```

Cobertura:

```bash
npm run test:coverage
```

A configuração em `vite.config.js` exige cobertura mínima de **80%** em statements, branches, functions e lines.

---

## Estrutura de pastas

```
.
├── README.md                                  # Este arquivo
├── RELATORIO_PROJETO_QUALIDADE.md             # Relatório técnico completo (entrega)
├── backend/
│   ├── RaizesDoNordeste.sln
│   ├── src/
│   │   ├── RaizesDoNordeste.Dominio/          # Entidades, enums, exceções, interfaces
│   │   ├── RaizesDoNordeste.Aplicacao/        # Casos de uso, DTOs
│   │   ├── RaizesDoNordeste.Infraestrutura/   # EF Core, repositórios, serviços externos
│   │   └── RaizesDoNordeste.API/              # Controllers, Program.cs, Swagger
│   └── testes/
│       ├── RaizesDoNordeste.Testes.Unitarios/
│       └── RaizesDoNordeste.Testes.Integracao/
├── frontend/
│   ├── src/
│   │   ├── armazenamento/                     # Stores Pinia
│   │   ├── componentes/                       # Componentes reutilizáveis
│   │   ├── paginas/                           # Telas (Login, Cadastro, Cardápio, ...)
│   │   ├── roteador/                          # Vue Router + guards
│   │   ├── servicos/                          # Clientes HTTP
│   │   └── utilitarios/
│   ├── public/
│   │   └── politica-privacidade.html          # Política de Privacidade (LGPD)
│   └── testes/
├── docs/
│   └── diagramas/                             # PlantUML: casos de uso, arquitetura, fluxo
├── testes-performance/
│   └── README-jmeter.md                       # Plano de teste de carga
├── testes-seguranca/
│   └── relatorio-zap.md                       # Relatório OWASP ZAP simulado
└── .github/
    └── workflows/
        └── ci.yml                             # Pipeline GitHub Actions
```

---

## Documentação principal

- **[RELATORIO_PROJETO_QUALIDADE.md](RELATORIO_PROJETO_QUALIDADE.md)** — Relatório técnico completo, com 14 seções cobrindo todos os requisitos do PDF da disciplina (introdução, requisitos funcionais e não funcionais, escopo, plano de QA, plano de testes, métricas, LGPD, arquitetura, conclusão e referências ABNT).
- **[docs/diagramas/](docs/diagramas/)** — Diagramas em PlantUML (casos de uso, arquitetura em camadas, fluxo de pedido).
- **[testes-performance/README-jmeter.md](testes-performance/README-jmeter.md)** — Plano e cenários do teste de carga com JMeter.
- **[testes-seguranca/relatorio-zap.md](testes-seguranca/relatorio-zap.md)** — Relatório de teste de segurança baseado em OWASP Top 10.

---

## LGPD e Privacidade

A conformidade com a Lei 13.709/2018 (LGPD) é estrutural neste projeto e não decorativa:

- **Consentimento explícito:** checkbox obrigatório no `Cadastro.vue`, validado também na entidade de domínio `Cliente.cs`
- **Minimização de dados:** apenas nome, e-mail, telefone e endereço de entrega são coletados
- **Transparência:** [`politica-privacidade.html`](frontend/public/politica-privacidade.html) acessível em todas as telas que coletam dados
- **Direitos do titular:** tela `MinhaConta.vue` com opções de exportar e excluir dados
- **Segurança das credenciais:** senhas armazenadas com hash bcrypt; comunicação via HTTPS/TLS
- **Banner de cookies/dados:** componente `AvisoLgpd.vue` exibido na primeira visita

---

## Declaração de uso de IA

Conforme orientação da disciplina, declaro que utilizei ferramentas de IA generativa (Claude, Anthropic) como assistente durante o planejamento, organização da arquitetura, escrita de documentação e geração de trechos de código.

A revisão crítica, validação técnica, decisões de arquitetura e adaptações ao escopo da disciplina foram realizadas pelo aluno.
Todo o código foi revisado, testado e adaptado ao contexto do projeto antes da entrega.
