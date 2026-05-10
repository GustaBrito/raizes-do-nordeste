<template>
  <div id="app">
    <NavBar v-if="mostrarNavBar" @abrirCarrinho="armazenamentoPedido.carrinhoAberto = true" />
    <AvisoLgpd @consentimento-alterado="tratarConsentimento" />
    <router-view />

    <!-- Modal do Carrinho -->
    <div v-if="armazenamentoPedido.carrinhoAberto" class="modal-overlay" @click.self="armazenamentoPedido.carrinhoAberto = false">
      <div class="modal-carrinho">
        <div class="modal-header">
          <h3>Seu Carrinho</h3>
          <button class="fechar-modal" @click="armazenamentoPedido.carrinhoAberto = false">&times;</button>
        </div>
        <div class="modal-body">
          <div v-if="armazenamentoPedido.itensCarrinho.length === 0" class="carrinho-vazio">
            Seu carrinho está vazio
          </div>
          <div v-else>
            <div v-for="item in armazenamentoPedido.itensCarrinho" :key="item.produtoId" class="item-carrinho">
              <span class="item-nome">{{ item.nomeProduto }}</span>
              <span class="item-qtd">x{{ item.quantidade }}</span>
              <span class="item-preco">R$ {{ (item.precoUnitario * item.quantidade).toFixed(2) }}</span>
              <button class="btn-remover" @click="armazenamentoPedido.removerItem(item.produtoId)" title="Remover item">×</button>
            </div>
            <div class="carrinho-total">
              <strong>Total:</strong> R$ {{ armazenamentoPedido.valorTotal.toFixed(2) }}
            </div>
            <button class="botao-finalizar" @click="irParaPedido">Finalizar Pedido</button>
          </div>
        </div>
      </div>
    </div>
    <transition name="toast">
      <div v-if="armazenamentoPedido.toastMensagem" class="toast-carrinho">
        {{ armazenamentoPedido.toastMensagem }}
      </div>
    </transition>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import NavBar from './componentes/NavBar.vue'
import AvisoLgpd from './componentes/AvisoLgpd.vue'
import { useArmazenamentoUsuario } from './armazenamento/usuario'
import { useArmazenamentoPedido } from './armazenamento/pedido'

const route = useRoute()
const router = useRouter()
const armazenamentoUsuario = useArmazenamentoUsuario()
const armazenamentoPedido = useArmazenamentoPedido()

const mostrarNavBar = computed(() => {
  const rotasPublicas = ['/login', '/cadastro']
  return armazenamentoUsuario.estaAutenticado && !rotasPublicas.includes(route.path)
})

function tratarConsentimento(aceito) {
  if (!aceito) {
    console.warn('Usuário recusou os termos de privacidade.')
  }
}

function irParaPedido() {
  armazenamentoPedido.carrinhoAberto = false
  router.push('/pedido')
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: #FFF8DC;
  min-height: 100vh;
}

#app {
  min-height: 100vh;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  justify-content: flex-end;
  align-items: stretch;
  z-index: 2000;
}

.modal-carrinho {
  background: #fff;
  border-radius: 12px 0 0 12px;
  width: 400px;
  max-width: 100%;
  height: 100vh;
  overflow-y: auto;
  box-shadow: -4px 0 24px rgba(0, 0, 0, 0.2);
  display: flex;
  flex-direction: column;
  animation: slideIn 0.25s ease;
}

@keyframes slideIn {
  from { transform: translateX(100%); }
  to   { transform: translateX(0); }
}

@media (max-width: 768px) {
  .modal-carrinho {
    width: 100%;
    border-radius: 0;
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #eee;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  border-radius: 12px 0 0 0;
  flex-shrink: 0;
}

.modal-header h3 {
  font-size: 1.1rem;
}

.fechar-modal {
  background: none;
  border: none;
  color: #fff;
  font-size: 1.5rem;
  cursor: pointer;
}

.modal-body {
  padding: 1.5rem;
  flex: 1;
  overflow-y: auto;
}

.carrinho-vazio {
  text-align: center;
  color: #888;
  padding: 2rem;
}

.item-carrinho {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #eee;
}

.item-nome {
  flex: 1;
  font-weight: 500;
}

.item-qtd {
  color: #666;
  margin: 0 1rem;
}

.item-preco {
  font-weight: 600;
  color: #e67e22;
}

.btn-remover {
  background: none;
  border: none;
  color: #c0392b;
  font-size: 1.25rem;
  line-height: 1;
  cursor: pointer;
  padding: 0 0 0 0.5rem;
  opacity: 0.7;
  transition: opacity 0.15s;
}

.btn-remover:hover {
  opacity: 1;
}

.carrinho-total {
  display: flex;
  justify-content: space-between;
  padding: 1rem 0;
  font-size: 1.1rem;
  border-top: 2px solid #e67e22;
  margin-top: 0.5rem;
}

.botao-finalizar {
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  margin-top: 1rem;
  transition: transform 0.2s, box-shadow 0.2s;
}

.botao-finalizar:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(230, 126, 34, 0.4);
}

.toast-carrinho {
  position: fixed;
  bottom: 1.5rem;
  right: 1.5rem;
  background: #2ecc71;
  color: #fff;
  padding: 0.75rem 1.25rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.9rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 3000;
  pointer-events: none;
}

.toast-enter-active,
.toast-leave-active {
  transition: opacity 0.3s, transform 0.3s;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateY(8px);
}
</style>
