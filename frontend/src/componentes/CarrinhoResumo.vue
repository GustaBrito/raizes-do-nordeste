<template>
  <aside class="carrinho-resumo">
    <h3>Seu Pedido</h3>

    <div v-if="armazenamentoPedido.itensCarrinho.length === 0" class="carrinho-resumo__vazio">
      <p>Nenhum item adicionado.</p>
    </div>

    <ul v-else class="carrinho-resumo__lista">
      <li
        v-for="item in armazenamentoPedido.itensCarrinho"
        :key="item.produtoId"
        class="carrinho-resumo__item"
      >
        <div class="carrinho-resumo__info">
          <span class="carrinho-resumo__nome">{{ item.nomeProduto }}</span>
          <span class="carrinho-resumo__preco">{{ formatarMoeda(item.precoUnitario) }}</span>
        </div>
        <div class="carrinho-resumo__controles">
          <button
            aria-label="Diminuir quantidade"
            @click="armazenamentoPedido.alterarQuantidade(item.produtoId, item.quantidade - 1)"
          >-</button>
          <span>{{ item.quantidade }}</span>
          <button
            aria-label="Aumentar quantidade"
            @click="armazenamentoPedido.alterarQuantidade(item.produtoId, item.quantidade + 1)"
          >+</button>
        </div>
      </li>
    </ul>

    <div class="carrinho-resumo__total">
      <strong>Total:</strong>
      <strong>{{ formatarMoeda(armazenamentoPedido.valorTotal) }}</strong>
    </div>

    <button
      class="botao botao--finalizar"
      :disabled="armazenamentoPedido.itensCarrinho.length === 0 || armazenamentoPedido.carregando"
      @click="emit('finalizar')"
    >
      {{ armazenamentoPedido.carregando ? 'Processando...' : 'Finalizar Pedido' }}
    </button>
  </aside>
</template>

<script setup>
import { useArmazenamentoPedido } from '../armazenamento/pedido'
import { formatarMoeda } from '../utilitarios/formatar-moeda'

const armazenamentoPedido = useArmazenamentoPedido()
const emit = defineEmits(['finalizar'])
</script>

<style scoped>
.carrinho-resumo {
  background: #fff;
  border-radius: 12px;
  padding: 1.25rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
}

.carrinho-resumo h3 {
  margin: 0 0 1rem;
  color: #8B4513;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.carrinho-resumo h3::before {
  content: '🛒';
}

.carrinho-resumo__lista {
  list-style: none;
  padding: 0;
  margin: 0 0 1rem;
}

.carrinho-resumo__item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.carrinho-resumo__info {
  display: flex;
  flex-direction: column;
}

.carrinho-resumo__nome {
  font-weight: 500;
  color: #333;
}

.carrinho-resumo__preco {
  font-size: 0.85rem;
  color: #888;
}

.carrinho-resumo__controles {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.carrinho-resumo__controles button {
  width: 32px;
  height: 32px;
  border: none;
  background: #f0f0f0;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1.1rem;
  font-weight: 600;
  color: #555;
  transition: all 0.2s;
}

.carrinho-resumo__controles button:hover {
  background: #e67e22;
  color: #fff;
}

.carrinho-resumo__controles span {
  min-width: 24px;
  text-align: center;
  font-weight: 600;
}

.carrinho-resumo__total {
  display: flex;
  justify-content: space-between;
  margin: 1rem 0;
  font-size: 1.15rem;
  padding-top: 0.75rem;
  border-top: 2px solid #e67e22;
}

.carrinho-resumo__total strong:last-child {
  color: #e67e22;
}

.carrinho-resumo__vazio {
  color: #999;
  text-align: center;
  padding: 1.5rem 0;
}

.carrinho-resumo__vazio::before {
  content: '🛒';
  display: block;
  font-size: 2rem;
  margin-bottom: 0.5rem;
  opacity: 0.5;
}

.botao--finalizar {
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.botao--finalizar:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(230, 126, 34, 0.4);
}

.botao--finalizar:disabled {
  background: #ccc;
  cursor: not-allowed;
}
</style>
