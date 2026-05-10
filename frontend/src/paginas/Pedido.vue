<template>
  <div class="pagina-pedido">
    <div class="pedido-hero">
      <h1>Finalizar Pedido</h1>
      <p>Escolha a unidade e forma de pagamento</p>
    </div>

    <div class="pedido-conteudo">
      <!-- Seleção de unidade franqueada -->
      <section class="pedido-secao">
        <h2>Selecione a unidade</h2>
        <div class="franquias-lista">
          <label
            v-for="franquia in franquias"
            :key="franquia.id"
            class="franquia-opcao"
            :class="{ 'franquia-opcao--selecionada': armazenamentoPedido.franquiaSelecionada?.id === franquia.id }"
          >
            <input
              type="radio"
              name="franquia"
              :value="franquia"
              v-model="armazenamentoPedido.franquiaSelecionada"
            />
            <span>{{ franquia.nome }}</span>
          </label>
        </div>
      </section>

      <!-- Método de pagamento -->
      <section class="pedido-secao">
        <h2>Pagamento</h2>
        <div class="pagamento-opcoes">
          <label
            v-for="metodo in metodosPagamento"
            :key="metodo.valor"
            class="pagamento-opcao"
            :class="{ 'pagamento-opcao--selecionada': metodoPagamento === metodo.valor }"
          >
            <input type="radio" name="pagamento" :value="metodo.valor" v-model="metodoPagamento" />
            <span>{{ metodo.rotulo }}</span>
          </label>
        </div>
      </section>

      <!-- Resumo do pedido -->
      <section class="pedido-secao">
        <h2>Resumo</h2>
        <CarrinhoResumo @finalizar="confirmarPedido" />
      </section>

      <div v-if="armazenamentoPedido.erro" class="erro-mensagem" role="alert">
        {{ armazenamentoPedido.erro }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useArmazenamentoPedido } from '../armazenamento/pedido'
import { useArmazenamentoUsuario } from '../armazenamento/usuario'
import CarrinhoResumo from '../componentes/CarrinhoResumo.vue'

const roteador = useRouter()
const armazenamentoPedido = useArmazenamentoPedido()
const armazenamentoUsuario = useArmazenamentoUsuario()

const metodoPagamento = ref('cartao_credito')

const franquias = ref([
  { id: 'a1000000-0000-0000-0000-000000000001', nome: 'Fortaleza Centro' },
  { id: 'a2000000-0000-0000-0000-000000000002', nome: 'Recife Boa Viagem' },
  { id: 'a3000000-0000-0000-0000-000000000003', nome: 'Salvador Pituba' },
  { id: 'a4000000-0000-0000-0000-000000000004', nome: 'Natal Ponta Negra' }
])

const metodosPagamento = ref([
  { valor: 'cartao_credito', rotulo: 'Cartão de Crédito' },
  { valor: 'cartao_debito', rotulo: 'Cartão de Débito' },
  { valor: 'pix', rotulo: 'Pix' },
  { valor: 'dinheiro', rotulo: 'Dinheiro' }
])

async function confirmarPedido() {
  try {
    const resultado = await armazenamentoPedido.finalizarPedido(
      armazenamentoUsuario.dadosUsuario?.clienteId,
      metodoPagamento.value
    )
    roteador.push(`/acompanhamento/${resultado.numeroPedido}`)
  } catch {
    // Erro já gerenciado no armazenamento
  }
}
</script>

<style scoped>
.pagina-pedido {
  min-height: calc(100vh - 60px);
  background: linear-gradient(180deg, #FFF8DC 0%, #FAEBD7 100%);
}

.pedido-hero {
  background: linear-gradient(135deg, #8B4513 0%, #A0522D 100%);
  color: #fff;
  padding: 1.5rem;
  text-align: center;
}

.pedido-hero h1 {
  font-size: 1.5rem;
  margin: 0 0 0.25rem;
}

.pedido-hero p {
  opacity: 0.9;
  margin: 0;
  font-size: 0.9rem;
}

.pedido-conteudo {
  max-width: 700px;
  margin: 0 auto;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.pedido-secao {
  background: #fff;
  border-radius: 12px;
  padding: 1.25rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
}

.pedido-secao h2 {
  margin: 0 0 1rem;
  color: #8B4513;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.pedido-secao h2::before {
  content: '';
  width: 4px;
  height: 20px;
  background: #e67e22;
  border-radius: 2px;
}

.franquias-lista,
.pagamento-opcoes {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.franquia-opcao,
.pagamento-opcao {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.9rem 1rem;
  border: 2px solid #eee;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s;
}

.franquia-opcao:hover,
.pagamento-opcao:hover {
  border-color: #e67e22;
}

.franquia-opcao--selecionada,
.pagamento-opcao--selecionada {
  border-color: #e67e22;
  background: linear-gradient(135deg, #fff8f2 0%, #fff 100%);
  box-shadow: 0 2px 8px rgba(230, 126, 34, 0.15);
}

.franquia-opcao input,
.pagamento-opcao input {
  accent-color: #e67e22;
}

.erro-mensagem {
  background: #fdecea;
  color: #c0392b;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  border-left: 4px solid #c0392b;
}
</style>
