<template>
  <div id="app">
    <NavBar v-if="mostrarNavBar" @abrirCarrinho="carrinhoAberto = true" />
    <AvisoLgpd @consentimento-alterado="tratarConsentimento" />
    <router-view />

    <!-- Modal do Carrinho -->
    <div v-if="carrinhoAberto" class="modal-overlay" @click.self="carrinhoAberto = false">
      <div class="modal-carrinho">
        <div class="modal-header">
          <h3>Seu Carrinho</h3>
          <button class="fechar-modal" @click="carrinhoAberto = false">&times;</button>
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
            </div>
            <div class="carrinho-total">
              <strong>Total:</strong> R$ {{ armazenamentoPedido.valorTotal.toFixed(2) }}
            </div>
            <button class="botao-finalizar" @click="irParaPedido">Finalizar Pedido</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import NavBar from './componentes/NavBar.vue'
import AvisoLgpd from './componentes/AvisoLgpd.vue'
import { useArmazenamentoUsuario } from './armazenamento/usuario'
import { useArmazenamentoPedido } from './armazenamento/pedido'

const route = useRoute()
const router = useRouter()
const armazenamentoUsuario = useArmazenamentoUsuario()
const armazenamentoPedido = useArmazenamentoPedido()

const carrinhoAberto = ref(false)

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
  carrinhoAberto.value = false
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
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding-top: 80px;
  z-index: 2000;
}

.modal-carrinho {
  background: #fff;
  border-radius: 12px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #eee;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  border-radius: 12px 12px 0 0;
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
</style>
