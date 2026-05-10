<template>
  <div class="pagina-acompanhamento">
    <div class="acompanhamento-hero">
      <h1>Acompanhe seu Pedido</h1>
      <p>Preparando seu pedido com carinho</p>
    </div>

    <div class="acompanhamento-conteudo">
      <div v-if="carregando" class="estado-carregando" role="status">
        Carregando status do pedido...
      </div>

      <div v-else-if="erro" class="erro-mensagem" role="alert">
        {{ erro }}
      </div>

      <div v-else-if="statusPedido" class="pedido-card">
        <div class="pedido-numero">
          <span class="rotulo">Número do pedido</span>
          <strong>{{ statusPedido.numeroPedido }}</strong>
        </div>

        <div class="pedido-status">
          <span class="status-badge" :class="`status-badge--${classeStatus}`">
            {{ statusPedido.status }}
          </span>
        </div>

        <div class="pedido-etapas">
          <div
            v-for="(etapa, indice) in etapas"
            :key="indice"
            class="etapa"
            :class="{
              'etapa--concluida': etapa.concluida,
              'etapa--atual': etapa.atual
            }"
          >
            <div class="etapa-indicador"></div>
            <span>{{ etapa.rotulo }}</span>
          </div>
        </div>

        <div class="pedido-valor">
          <span>Total pago:</span>
          <strong>{{ formatarMoeda(statusPedido.valorTotal) }}</strong>
        </div>

        <div v-if="statusPedido.pontosAdicionados" class="pedido-pontos">
          +{{ statusPedido.pontosAdicionados }} pontos adicionados ao seu saldo de fidelidade
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import ServicoPedido from '../servicos/servico-pedido'
import { formatarMoeda } from '../utilitarios/formatar-moeda'

const rota = useRoute()
const numeroPedido = rota.params.numeroPedido

const carregando = ref(true)
const erro = ref(null)
const statusPedido = ref(null)

// Mapeamento de status para classe CSS
const classeStatus = computed(() => {
  const mapa = {
    EmPreparacao: 'preparando',
    ProntoParaEntrega: 'pronto',
    EmEntrega: 'entrega',
    Entregue: 'entregue',
    Cancelado: 'cancelado'
  }
  return mapa[statusPedido.value?.status] || 'preparando'
})

const etapas = computed(() => {
  const statusAtual = statusPedido.value?.status
  const ordem = ['EmPreparacao', 'ProntoParaEntrega', 'EmEntrega', 'Entregue']
  const indiceAtual = ordem.indexOf(statusAtual)

  return [
    { rotulo: 'Pedido recebido', concluida: indiceAtual >= 0, atual: indiceAtual === 0 },
    { rotulo: 'Em preparação', concluida: indiceAtual >= 1, atual: indiceAtual === 1 },
    { rotulo: 'Pronto para entrega', concluida: indiceAtual >= 2, atual: indiceAtual === 2 },
    { rotulo: 'Entregue', concluida: indiceAtual >= 3, atual: indiceAtual === 3 }
  ]
})

onMounted(async () => {
  try {
    statusPedido.value = await ServicoPedido.consultarStatus(numeroPedido)
  } catch {
    erro.value = 'Não foi possível carregar o status do pedido. Tente novamente em instantes.'
  } finally {
    carregando.value = false
  }
})
</script>

<style scoped>
.pagina-acompanhamento {
  min-height: calc(100vh - 60px);
  background: linear-gradient(180deg, #FFF8DC 0%, #FAEBD7 100%);
}

.acompanhamento-hero {
  background: linear-gradient(135deg, #27ae60 0%, #2ecc71 100%);
  color: #fff;
  padding: 1.5rem;
  text-align: center;
}

.acompanhamento-hero h1 {
  font-size: 1.5rem;
  margin: 0 0 0.25rem;
}

.acompanhamento-hero p {
  opacity: 0.9;
  margin: 0;
  font-size: 0.9rem;
}

.acompanhamento-conteudo {
  max-width: 600px;
  margin: 0 auto;
  padding: 1.5rem;
}

.pedido-card {
  background: #fff;
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.pedido-numero {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  text-align: center;
  padding-bottom: 1rem;
  border-bottom: 2px dashed #eee;
}

.rotulo {
  font-size: 0.85rem;
  color: #888;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.pedido-numero strong {
  font-size: 1.8rem;
  color: #8B4513;
}

.pedido-status {
  text-align: center;
}

.status-badge {
  display: inline-block;
  padding: 0.5rem 1.5rem;
  border-radius: 25px;
  font-weight: 600;
  font-size: 1rem;
}

.status-badge--preparando { background: #fef9e7; color: #d4ac0d; }
.status-badge--pronto { background: #eafaf1; color: #27ae60; }
.status-badge--entrega { background: #ebf5fb; color: #2980b9; }
.status-badge--entregue { background: #eafaf1; color: #1e8449; }
.status-badge--cancelado { background: #fdecea; color: #c0392b; }

.pedido-etapas {
  display: flex;
  flex-direction: column;
  gap: 0;
  padding: 0 1rem;
}

.etapa {
  display: flex;
  align-items: center;
  gap: 1rem;
  color: #bbb;
  padding: 0.75rem 0;
  position: relative;
}

.etapa:not(:last-child)::after {
  content: '';
  position: absolute;
  left: 7px;
  top: 40px;
  width: 2px;
  height: calc(100% - 20px);
  background: #ddd;
}

.etapa--concluida:not(:last-child)::after {
  background: #27ae60;
}

.etapa-indicador {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 2px solid #ddd;
  flex-shrink: 0;
  z-index: 1;
  background: #fff;
}

.etapa--concluida .etapa-indicador {
  background: #27ae60;
  border-color: #27ae60;
}

.etapa--atual {
  color: #2c3e50;
  font-weight: 600;
}

.etapa--atual .etapa-indicador {
  background: #e67e22;
  border-color: #e67e22;
  box-shadow: 0 0 0 4px rgba(230, 126, 34, 0.2);
}

.pedido-valor {
  display: flex;
  justify-content: space-between;
  font-size: 1.1rem;
  border-top: 2px solid #eee;
  padding-top: 1rem;
}

.pedido-valor strong {
  color: #e67e22;
  font-size: 1.25rem;
}

.pedido-pontos {
  background: linear-gradient(135deg, #fff8f2 0%, #ffeaa7 100%);
  color: #8B4513;
  padding: 0.75rem 1rem;
  border-radius: 10px;
  font-size: 0.95rem;
  font-weight: 600;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.pedido-pontos::before {
  content: '⭐';
}

.estado-carregando {
  text-align: center;
  padding: 3rem;
  color: #888;
}

.erro-mensagem {
  background: #fdecea;
  color: #c0392b;
  padding: 1rem;
  border-radius: 10px;
  border-left: 4px solid #c0392b;
}
</style>
