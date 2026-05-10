<template>
  <div class="pagina-minha-conta">
    <div class="conta-hero">
      <div class="avatar">{{ iniciais }}</div>
      <h1>{{ armazenamentoUsuario.nomeUsuario }}</h1>
      <p>{{ armazenamentoUsuario.dadosUsuario?.email }}</p>
    </div>

    <div class="conta-conteudo">
      <section class="pontos-card">
        <div class="pontos-icone">⭐</div>
        <div class="pontos-info">
          <span class="pontos-label">Seus pontos de fidelidade</span>
          <span class="pontos-valor">{{ pontos }}</span>
          <span class="pontos-texto">pontos disponíveis</span>
        </div>
        <button class="botao-resgatar" @click="mostrarResgate = true" :disabled="pontos < 100">
          Resgatar
        </button>
      </section>

      <!-- LGPD: Direitos do titular de dados pessoais (Art. 18 da Lei 13.709/2018) -->
      <section class="conta-secao lgpd-secao">
        <h2>Seus Direitos (LGPD)</h2>
        <p>
          Em conformidade com a Lei Geral de Proteção de Dados (Lei 13.709/2018), você tem direito a:
        </p>
        <ul class="lgpd-lista">
          <li>Acesso aos seus dados pessoais</li>
          <li>Correção de dados incompletos ou desatualizados</li>
          <li>Portabilidade dos dados</li>
          <li>Eliminação dos dados tratados com consentimento</li>
          <li>Revogação do consentimento a qualquer momento</li>
        </ul>

        <div class="lgpd-acoes">
          <button class="botao botao--secundario" @click="exportarDados">
            Exportar meus dados
          </button>
          <button class="botao botao--perigo" @click="confirmarExclusao = true">
            Solicitar exclusão de dados
          </button>
        </div>

        <div
          v-if="confirmarExclusao"
          class="confirmacao-exclusao"
          role="dialog"
          aria-modal="true"
          aria-label="Confirmar exclusão de dados"
        >
          <p>
            <strong>Atenção:</strong> Ao solicitar a exclusão, todos os seus dados pessoais serão
            removidos permanentemente. Esta ação não pode ser desfeita.
          </p>
          <div class="confirmacao-exclusao__acoes">
            <button class="botao botao--perigo" @click="solicitarExclusao">
              Confirmar exclusão
            </button>
            <button class="botao botao--secundario" @click="confirmarExclusao = false">
              Cancelar
            </button>
          </div>
        </div>
      </section>

      <div v-if="mostrarResgate" class="modal-resgate">
        <div class="modal-conteudo">
          <h3>Resgatar Pontos</h3>
          <p>Você tem <strong>{{ pontos }}</strong> pontos disponíveis.</p>
          <p>Cada 100 pontos = R$ 5,00 de desconto</p>
          <div class="resgate-acoes">
            <button class="botao-cancelar" @click="mostrarResgate = false">Cancelar</button>
            <button class="botao-confirmar" @click="resgatarPontos">Confirmar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useArmazenamentoUsuario } from '../armazenamento/usuario'
import ServicoFidelizacao from '../servicos/servico-fidelizacao'

const roteador = useRouter()
const armazenamentoUsuario = useArmazenamentoUsuario()
const pontos = ref(0)
const confirmarExclusao = ref(false)
const mostrarResgate = ref(false)

const iniciais = computed(() => {
  const nome = armazenamentoUsuario.nomeUsuario || ''
  const partes = nome.split(' ')
  if (partes.length >= 2) {
    return (partes[0][0] + partes[partes.length - 1][0]).toUpperCase()
  }
  return nome.substring(0, 2).toUpperCase()
})

onMounted(async () => {
  const clienteId = armazenamentoUsuario.dadosUsuario?.clienteId
  if (clienteId) {
    try {
      const saldo = await ServicoFidelizacao.consultarPontos(clienteId)
      pontos.value = saldo.saldoPontos
    } catch {
      pontos.value = 0
    }
  }
})

function exportarDados() {
  const dadosExportacao = {
    nome: armazenamentoUsuario.nomeUsuario,
    email: armazenamentoUsuario.dadosUsuario?.email,
    dataExportacao: new Date().toISOString()
  }

  const blob = new Blob([JSON.stringify(dadosExportacao, null, 2)], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = 'meus-dados-raizes-do-nordeste.json'
  link.click()
  URL.revokeObjectURL(url)
}

function solicitarExclusao() {
  alert('Solicitação de exclusão enviada. Você receberá um e-mail de confirmação em até 15 dias.')
  confirmarExclusao.value = false
}

async function resgatarPontos() {
  try {
    const clienteId = armazenamentoUsuario.dadosUsuario?.clienteId
    await ServicoFidelizacao.resgatar(clienteId, 100)
    pontos.value = Math.max(0, pontos.value - 100)
    mostrarResgate.value = false
    alert('Resgate realizado com sucesso! R$ 5,00 de desconto no próximo pedido.')
  } catch {
    alert('Erro ao resgatar pontos. Tente novamente.')
  }
}

function sair() {
  armazenamentoUsuario.sair()
  roteador.push('/login')
}
</script>

<style scoped>
.pagina-minha-conta {
  min-height: calc(100vh - 60px);
  background: linear-gradient(180deg, #FFF8DC 0%, #FAEBD7 100%);
}

.conta-hero {
  background: linear-gradient(135deg, #8B4513 0%, #A0522D 100%);
  color: #fff;
  padding: 2rem 1.5rem;
  text-align: center;
}

.avatar {
  width: 80px;
  height: 80px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0 auto 1rem;
  border: 3px solid rgba(255, 255, 255, 0.3);
}

.conta-hero h1 {
  font-size: 1.5rem;
  margin: 0 0 0.25rem;
}

.conta-hero p {
  opacity: 0.85;
  margin: 0;
  font-size: 0.9rem;
}

.conta-conteudo {
  max-width: 700px;
  margin: 0 auto;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.pontos-card {
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  border-radius: 16px;
  padding: 1.5rem;
  color: #fff;
  display: flex;
  align-items: center;
  gap: 1rem;
  box-shadow: 0 4px 20px rgba(230, 126, 34, 0.3);
}

.pontos-icone {
  font-size: 3rem;
}

.pontos-info {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.pontos-label {
  font-size: 0.8rem;
  opacity: 0.9;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.pontos-valor {
  font-size: 2.5rem;
  font-weight: 700;
  line-height: 1;
}

.pontos-texto {
  font-size: 0.9rem;
  opacity: 0.9;
}

.botao-resgatar {
  background: rgba(255, 255, 255, 0.2);
  border: 2px solid rgba(255, 255, 255, 0.5);
  color: #fff;
  padding: 0.6rem 1.25rem;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.botao-resgatar:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
}

.botao-resgatar:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.conta-secao {
  background: #fff;
  border-radius: 12px;
  padding: 1.25rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
}

.conta-secao h2 {
  margin: 0 0 0.75rem;
  color: #8B4513;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.conta-secao h2::before {
  content: '';
  width: 4px;
  height: 20px;
  background: #e67e22;
  border-radius: 2px;
}

.conta-secao p {
  margin: 0.25rem 0;
  color: #555;
}

.lgpd-secao {
  border-left: 4px solid #e67e22;
  background: linear-gradient(90deg, #fff8f2 0%, #fff 100%);
}

.lgpd-lista {
  color: #555;
  padding-left: 1.25rem;
  margin: 0.5rem 0 1rem;
  line-height: 1.8;
}

.lgpd-acoes {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.botao {
  padding: 0.6rem 1.25rem;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.botao--secundario {
  background: #fff;
  color: #555;
  border: 1px solid #ccc;
}

.botao--secundario:hover {
  background: #f5f5f5;
}

.botao--perigo {
  background: #c0392b;
  color: #fff;
  border: none;
}

.botao--perigo:hover {
  background: #a93226;
}

.confirmacao-exclusao {
  margin-top: 1rem;
  background: #fdecea;
  border: 1px solid #e74c3c;
  border-radius: 10px;
  padding: 1rem;
}

.confirmacao-exclusao p {
  margin: 0 0 0.75rem;
  color: #c0392b;
}

.confirmacao-exclusao__acoes {
  display: flex;
  gap: 0.5rem;
}

.modal-resgate {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.modal-conteudo {
  background: #fff;
  border-radius: 16px;
  padding: 2rem;
  max-width: 400px;
  width: 90%;
  text-align: center;
}

.modal-conteudo h3 {
  color: #8B4513;
  margin: 0 0 1rem;
}

.modal-conteudo p {
  color: #555;
  margin: 0.5rem 0;
}

.resgate-acoes {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
  justify-content: center;
}

.botao-cancelar {
  padding: 0.6rem 1.5rem;
  background: #eee;
  border: none;
  border-radius: 8px;
  color: #555;
  font-weight: 600;
  cursor: pointer;
}

.botao-confirmar {
  padding: 0.6rem 1.5rem;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  border: none;
  border-radius: 8px;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
}

@media (max-width: 500px) {
  .pontos-card {
    flex-direction: column;
    text-align: center;
  }
}
</style>
