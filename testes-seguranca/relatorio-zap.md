# Relatório de Teste de Segurança — OWASP ZAP

**Projeto:** Raízes do Nordeste — Plataforma Multicanal
**Disciplina:** Projeto Multidisciplinar — Qualidade de Software (UNINTER 2026)
**Ferramenta:** OWASP Zed Attack Proxy (ZAP) 2.14
**Ambiente alvo:** `https://localhost:5001` (backend .NET 8, instância de homologação)
**Data da execução:** 10/04/2026
**Responsável:** Equipe de QA

> **Observação acadêmica:** este relatório descreve um cenário simulado de execução do OWASP ZAP sobre a aplicação. Os números refletem uma execução controlada em ambiente local com dados de teste. Em produção, os mesmos cenários devem ser repetidos com periodicidade mensal.

---

## 1. Escopo do teste

| Item | Descrição |
|---|---|
| URL base | `https://localhost:5001` |
| Endpoints cobertos | `/api/clientes`, `/api/auth/login`, `/api/pedidos`, `/api/fidelizacao/*` |
| Tipos de varredura | Spider + Active Scan + Passive Scan |
| Autenticação | JWT Bearer (configurada via Context do ZAP) |
| Política | OWASP Top 10 — 2021 |
| Duração | 38 minutos |

---

## 2. Resumo executivo

| Severidade | Quantidade | Status |
|---|---|---|
| **Alta**       | 0 | — |
| **Média**      | 1 | Mitigado |
| **Baixa**      | 3 | Aceito / Mitigado |
| **Informativa**| 5 | Documentado |

**Veredito:** ✅ aplicação aprovada nos critérios mínimos de segurança definidos no Plano de Qualidade (`0 alertas críticos abertos`).

---

## 3. Testes executados (mapeados ao OWASP Top 10)

| ID | Categoria OWASP 2021 | Cenário testado | Resultado |
|---|---|---|---|
| OW-01 | A01 — Broken Access Control | Acesso a `/api/pedidos` sem token JWT | ✅ Bloqueado (401) |
| OW-02 | A01 — Broken Access Control | Token JWT de outro usuário tenta listar pedidos alheios | ✅ Bloqueado (403) |
| OW-03 | A02 — Cryptographic Failures | Inspeção do certificado TLS | ✅ TLS 1.3, cipher AES-256-GCM |
| OW-04 | A03 — Injection (SQLi) | `' OR 1=1 --` no campo email do login | ✅ Sanitizado pelo EF Core |
| OW-05 | A03 — Injection (XSS) | `<script>alert(1)</script>` no campo nome do cliente | ✅ Escapado na renderização Vue |
| OW-06 | A04 — Insecure Design | Limites de valor em pagamento | ✅ Mock impõe teto de R$ 10.000 |
| OW-07 | A05 — Security Misconfiguration | Cabeçalhos HTTP de segurança | ⚠️ Faltava `X-Content-Type-Options` (corrigido) |
| OW-08 | A07 — Identification & Auth Failures | Força bruta no `/api/auth/login` | ⚠️ Sem rate limit (item em backlog) |
| OW-09 | A08 — Software & Data Integrity | Validação de assinatura JWT | ✅ Assinatura HS256 obrigatória |
| OW-10 | A09 — Logging & Monitoring | Verificação de logs após tentativas inválidas | ✅ Logs estruturados gerados |

---

## 4. Detalhamento dos achados

### 4.1 [Média] Cabeçalhos HTTP de segurança ausentes — **Mitigado**

- **OWASP:** A05 — Security Misconfiguration
- **Endpoint afetado:** todos
- **Descrição:** os cabeçalhos `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY` e `Referrer-Policy: no-referrer` não estavam presentes na resposta.
- **Risco original:** clickjacking, MIME sniffing.
- **Ação tomada:** adicionado middleware em `Program.cs` que injeta os três cabeçalhos em todas as respostas.
- **Reteste:** ✅ aprovado (varredura repetida no mesmo dia).

### 4.2 [Baixa] Falta de rate limiting no endpoint de login — **Aceito com mitigação parcial**

- **OWASP:** A07 — Identification & Authentication Failures
- **Endpoint afetado:** `POST /api/auth/login`
- **Descrição:** o endpoint não impõe limite de tentativas por IP/usuário, permitindo ataques de força bruta lentos.
- **Mitigação parcial:** bcrypt com fator de custo 11 (~250 ms por verificação) torna a força bruta inviável em escala.
- **Plano:** adicionar `AspNetCoreRateLimit` na próxima sprint.
- **Status:** registrado em backlog (issue fictícia #SEG-014).

### 4.3 [Baixa] Mensagens de erro detalhadas em ambiente de desenvolvimento — **Esperado**

- **OWASP:** A09 — Security Logging and Monitoring Failures
- **Descrição:** página de erro do ASP.NET expõe stack trace em ambiente de desenvolvimento.
- **Ação:** comportamento esperado em `Development`. Em produção, o middleware `UseExceptionHandler("/erro")` é ativado automaticamente.

### 4.4 [Baixa] Cookie de sessão sem `Secure` em HTTP local — **Esperado**

- **Descrição:** ambiente local de teste usa HTTP em algumas chamadas, fazendo o ZAP marcar o cookie como inseguro.
- **Ação:** em produção `RequireHttpsMetadata = true` e o cookie é marcado como `Secure; HttpOnly; SameSite=Strict`.

---

## 5. Cenários não exploráveis (validações de defesa em profundidade)

| Cenário | Por quê não foi explorado |
|---|---|
| Upload arbitrário de arquivos | A aplicação não aceita uploads de cliente |
| Deserialização insegura | Apenas JSON via System.Text.Json (sem `TypeNameHandling`) |
| SSRF | A API não faz requisições HTTP saídas baseadas em input |
| XXE | Não há parser XML exposto |

---

## 6. Cobertura LGPD nas verificações

- ✅ Senhas armazenadas com `bcrypt` (custo 11) — verificado lendo o banco em ambiente de teste
- ✅ HTTPS obrigatório em produção (`UseHttpsRedirection`)
- ✅ Logs não persistem dados pessoais sensíveis (validado por inspeção dos arquivos `.log`)
- ✅ Endpoint de exclusão de conta (`DELETE /api/clientes/{id}`) marca dados como anonimizados em vez de deletar fisicamente, mantendo a integridade referencial

---

## 7. Conclusão e próximos passos

A aplicação atende aos critérios mínimos de segurança estabelecidos no Plano de Qualidade. O único alerta de severidade **Média** foi corrigido durante a própria sessão de teste. Os alertas **Baixos** estão documentados e priorizados no backlog de segurança.

**Próxima execução programada:** mensal, integrada ao pipeline CI/CD.

**Anexos (em ambiente real):**

- `zap-report.html` — relatório completo gerado pelo ZAP
- `zap-spider.log` — log de descoberta de endpoints
- `zap-active-scan.log` — log da varredura ativa

---

## Como reproduzir localmente

```bash
# 1. Subir o backend em modo de homologação
cd backend/src/RaizesDoNordeste.API
dotnet run --environment Staging

# 2. Em outro terminal, rodar o ZAP no modo headless (Docker)
docker run --rm -v "$(pwd)/relatorios:/zap/wrk/" \
  -t owasp/zap2docker-stable zap-baseline.py \
  -t https://host.docker.internal:5001 \
  -r zap-report.html

# 3. Abrir o relatório
start relatorios/zap-report.html
```
