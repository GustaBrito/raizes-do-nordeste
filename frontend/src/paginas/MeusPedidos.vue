<template>
  <div class="meus-pedidos">
    <div class="container">
      <h1 class="titulo">Meus Pedidos</h1>

      <div v-if="carregando" class="carregando">
        <div class="spinner"></div>
        <p>Carregando pedidos...</p>
      </div>

      <div v-else-if="pedidos.length === 0" class="sem-pedidos">
        <span class="icone-vazio">📋</span>
        <h2>Nenhum pedido encontrado</h2>
        <p>Você ainda não fez nenhum pedido.</p>
        <router-link to="/cardapio" class="botao-cardapio">
          Ver Cardápio
        </router-link>
      </div>

      <div v-else class="lista-pedidos">
        <div v-for="pedido in pedidos" :key="pedido.numeroPedido" class="card-pedido">
          <div class="pedido-header">
            <span class="numero-pedido">#{{ pedido.numeroPedido }}</span>
            <span class="status-badge" :class="'status-' + pedido.status.toLowerCase()">
              {{ formatarStatus(pedido.status) }}
            </span>
          </div>

          <div class="pedido-info">
            <div class="info-item">
              <span class="label">Data:</span>
              <span class="valor">{{ formatarData(pedido.dataPedido) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Itens:</span>
              <span class="valor">{{ pedido.quantidadeItens }} item(s)</span>
            </div>
            <div class="info-item destaque">
              <span class="label">Total:</span>
              <span class="valor">R$ {{ pedido.valorTotal.toFixed(2) }}</span>
            </div>
          </div>

          <router-link
            v-if="pedido.status !== 'Entregue' && pedido.status !== 'Cancelado'"
            :to="`/acompanhamento/${pedido.numeroPedido}`"
            class="botao-acompanhar"
          >
            Acompanhar
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useArmazenamentoUsuario } from '../armazenamento/usuario'
import clienteHttp from '../servicos/cliente-http'

const armazenamentoUsuario = useArmazenamentoUsuario()
const pedidos = ref([])
const carregando = ref(true)

onMounted(async () => {
  try {
    const clienteId = armazenamentoUsuario.dadosUsuario?.clienteId
    if (clienteId) {
      const resposta = await clienteHttp.get(`/clientes/${clienteId}/pedidos`)
      pedidos.value = resposta.data || []
    }
  } catch (erro) {
    console.error('Erro ao carregar pedidos:', erro)
    pedidos.value = []
  } finally {
    carregando.value = false
  }
})

function formatarData(dataString) {
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatarStatus(status) {
  const mapa = {
    'Pendente': 'Pendente',
    'EmPreparo': 'Em Preparo',
    'Pronto': 'Pronto',
    'EmEntrega': 'Em Entrega',
    'Entregue': 'Entregue',
    'Cancelado': 'Cancelado'
  }
  return mapa[status] || status
}
</script>

<style scoped>
.meus-pedidos {
  min-height: calc(100vh - 60px);
  background: linear-gradient(180deg, #FFF8DC 0%, #FAEBD7 100%);
  padding: 2rem 1rem;
}

.container {
  max-width: 800px;
  margin: 0 auto;
}

.titulo {
  color: #8B4513;
  font-size: 1.8rem;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.titulo::before {
  content: '📋';
}

.carregando {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 3rem;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e67e22;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.sem-pedidos {
  text-align: center;
  padding: 3rem;
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
}

.icone-vazio {
  font-size: 4rem;
  display: block;
  margin-bottom: 1rem;
}

.sem-pedidos h2 {
  color: #8B4513;
  margin-bottom: 0.5rem;
}

.sem-pedidos p {
  color: #666;
  margin-bottom: 1.5rem;
}

.botao-cardapio {
  display: inline-block;
  padding: 0.8rem 2rem;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  text-decoration: none;
  border-radius: 8px;
  font-weight: 600;
  transition: transform 0.2s, box-shadow 0.2s;
}

.botao-cardapio:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(230, 126, 34, 0.4);
}

.lista-pedidos {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.card-pedido {
  background: #fff;
  border-radius: 12px;
  padding: 1.25rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
  transition: transform 0.2s, box-shadow 0.2s;
}

.card-pedido:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.12);
}

.pedido-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid #eee;
}

.numero-pedido {
  font-size: 1.1rem;
  font-weight: 700;
  color: #8B4513;
}

.status-badge {
  padding: 0.3rem 0.8rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
}

.status-pendente {
  background: #ffeaa7;
  color: #856404;
}

.status-empreparo {
  background: #81ecec;
  color: #00695c;
}

.status-pronto {
  background: #a29bfe;
  color: #4a148c;
}

.status-ementrega {
  background: #74b9ff;
  color: #01579b;
}

.status-entregue {
  background: #55efc4;
  color: #1b5e20;
}

.status-cancelado {
  background: #fab1a0;
  color: #c62828;
}

.pedido-info {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.75rem;
  margin-bottom: 1rem;
}

@media (max-width: 500px) {
  .pedido-info {
    grid-template-columns: 1fr 1fr;
  }
}

.info-item {
  display: flex;
  flex-direction: column;
}

.info-item .label {
  font-size: 0.75rem;
  color: #888;
  text-transform: uppercase;
}

.info-item .valor {
  font-weight: 500;
  color: #333;
}

.info-item.destaque .valor {
  color: #e67e22;
  font-weight: 700;
  font-size: 1.1rem;
}

.botao-acompanhar {
  display: block;
  text-align: center;
  padding: 0.7rem;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  text-decoration: none;
  border-radius: 8px;
  font-weight: 600;
  transition: transform 0.2s, box-shadow 0.2s;
}

.botao-acompanhar:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(230, 126, 34, 0.4);
}
</style>
