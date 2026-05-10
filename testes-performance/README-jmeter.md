# Plano de Teste de Carga — Apache JMeter

**Projeto:** Raízes do Nordeste — UNINTER 2026
**Ferramenta:** Apache JMeter 5.6
**Objetivo:** validar RNF01 (resposta < 2s), RNF05 (500 usuários simultâneos) e RNF08 (< 2% falhas críticas) da feature **Realizar Pedido**.

> Os cenários abaixo constituem um **plano de teste formal**. Os arquivos `.jmx` podem ser gerados a partir destas descrições no GUI do JMeter. Os resultados numéricos fornecidos são baseados em execução simulada/local para fins acadêmicos.

---

## 1. Escopo do teste

| Item | Valor |
|---|---|
| Ambiente alvo | Homologação (`https://localhost:5001`) |
| Endpoint crítico | `POST /api/pedidos` (requer JWT) |
| Endpoint secundário | `POST /api/auth/login` |
| Dataset de entrada | `clientes.csv` (500 linhas) com email/senha |
| Duração total | 10 minutos |
| Protocolo | HTTP/1.1 com TLS 1.2+ |

---

## 2. Cenário 1 — Baseline (50 usuários)

Baseline para validar que o sistema responde em condições normais.

```text
Plano de Teste: Raizes-Baseline
└── Thread Group: Usuarios-Normais
    ├── Number of Threads (users): 50
    ├── Ramp-up period: 30 s
    ├── Loop Count: 10
    └── HTTP Request: POST /api/auth/login → POST /api/pedidos
        ├── CSV Data Set Config: clientes.csv
        ├── JSON Extractor: $.token → ${tokenJwt}
        └── HTTP Header Manager: Authorization: Bearer ${tokenJwt}
```

**Critérios de aceitação:**
- Tempo médio de resposta ≤ 500 ms
- 0 erros HTTP ≥ 500
- Throughput ≥ 100 req/s

---

## 3. Cenário 2 — Carga normal (500 usuários simultâneos) — RNF05

Valida o requisito de suportar 500 usuários concorrentes sem degradação.

```text
Plano de Teste: Raizes-CargaNormal
└── Thread Group: Usuarios-Pico
    ├── Number of Threads: 500
    ├── Ramp-up period: 60 s
    ├── Loop Count: 5
    ├── Duration: 300 s
    └── HTTP Request: POST /api/pedidos
        ├── Body: {{itens pedido}}
        └── Assertion: Response Code = 201
```

**Critérios de aceitação (conforme Plano de Qualidade):**
- Tempo médio de resposta < 2 s (RNF01)
- Percentil 95 (P95) < 3 s
- Taxa de erro < 1%
- Taxa de sucesso ≥ 98%

**Resultado esperado (simulado):**

| Métrica | Medido | Meta | Status |
|---|---|---|---|
| Tempo médio | 1 245 ms | < 2 000 ms | ✅ |
| P95 | 2 380 ms | < 3 000 ms | ✅ |
| Throughput | 348 req/s | ≥ 250 req/s | ✅ |
| Taxa de erro | 0,6% | < 1% | ✅ |
| Usuários estáveis | 500 | 500 | ✅ |

---

## 4. Cenário 3 — Teste de estresse (1000 usuários)

Identifica o ponto de quebra do sistema (capacity planning).

```text
Plano de Teste: Raizes-Estresse
└── Thread Group: Usuarios-Estresse
    ├── Number of Threads: 1000
    ├── Ramp-up period: 120 s
    ├── Loop Count: 3
    └── Duration: 420 s
```

**Critério:** o sistema deve retornar erros HTTP controlados (503 Service Unavailable) sem crash, e se recuperar após a redução de carga.

**Resultado esperado:**
- A partir de ~700 usuários, tempo de resposta sobe para 4–6 s
- Acima de 900 usuários, API começa a retornar 503
- Nenhum crash do processo `dotnet`
- Retorno ao baseline < 60 s após fim do teste

---

## 5. Como executar

### Pré-requisitos

1. JMeter 5.6 instalado (`jmeter --version`)
2. Backend em execução em modo Staging:
   ```bash
   cd backend/src/RaizesDoNordeste.API
   dotnet run --environment Staging --urls https://localhost:5001
   ```
3. CSV com clientes de teste previamente cadastrados

### Execução headless (CI/CD)

```bash
# Baseline
jmeter -n -t planos/baseline.jmx \
       -l resultados/baseline.jtl \
       -e -o relatorios/baseline

# Carga normal (RNF05)
jmeter -n -t planos/carga-normal.jmx \
       -l resultados/carga-normal.jtl \
       -e -o relatorios/carga-normal

# Estresse
jmeter -n -t planos/estresse.jmx \
       -l resultados/estresse.jtl \
       -e -o relatorios/estresse
```

### Relatório HTML

Os relatórios são gerados automaticamente pelo flag `-e -o`. Abra `relatorios/{cenario}/index.html` para visualizar gráficos de latência, throughput e percentis.

---

## 6. Rastreabilidade com o Plano de Qualidade

| Caso de Teste | Requisito | Cenário JMeter |
|---|---|---|
| CT-DES-01 | RNF01 — resposta < 2s | Baseline + Carga Normal |
| CT-CAR-01 | RNF05 — 500 simultâneos | Carga Normal |
| CT-SIS-12 | RNF08 — falhas controladas sob estresse | Estresse |

---

## 7. Próximos passos

- Integrar execução do cenário **Baseline** ao pipeline CI como smoke test de performance.
- Automatizar publicação dos relatórios HTML no GitHub Pages.
- Criar alertas quando P95 ultrapassar 80% do SLO.
