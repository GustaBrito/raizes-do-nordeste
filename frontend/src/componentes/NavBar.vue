<template>
  <nav class="navbar">
    <div class="navbar-container">
      <router-link to="/cardapio" class="navbar-logo">
        <span class="logo-icon">🌵</span>
        <span class="logo-texto">Raízes do Nordeste</span>
      </router-link>

      <button class="navbar-toggle" @click="menuAberto = !menuAberto" aria-label="Menu">
        <span class="hamburger"></span>
      </button>

      <div class="navbar-menu" :class="{ 'navbar-menu--aberto': menuAberto }">
        <router-link to="/cardapio" class="navbar-link" @click="menuAberto = false">
          <span class="link-icon">🍽️</span>
          Cardápio
        </router-link>

        <router-link to="/meus-pedidos" class="navbar-link" @click="menuAberto = false">
          <span class="link-icon">📋</span>
          Meus Pedidos
        </router-link>

        <router-link to="/minha-conta" class="navbar-link" @click="menuAberto = false">
          <span class="link-icon">👤</span>
          Minha Conta
        </router-link>

        <button class="navbar-carrinho" @click="$emit('abrirCarrinho')" v-if="quantidadeItens > 0">
          <span class="carrinho-icon">🛒</span>
          <span class="carrinho-badge">{{ quantidadeItens }}</span>
        </button>

        <div class="navbar-usuario">
          <span class="usuario-nome">{{ nomeUsuario }}</span>
          <button class="botao-sair" @click="sair">Sair</button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useArmazenamentoUsuario } from '../armazenamento/usuario'
import { useArmazenamentoPedido } from '../armazenamento/pedido'

defineEmits(['abrirCarrinho'])

const roteador = useRouter()
const armazenamentoUsuario = useArmazenamentoUsuario()
const armazenamentoPedido = useArmazenamentoPedido()

const menuAberto = ref(false)

const nomeUsuario = computed(() => armazenamentoUsuario.nomeUsuario || 'Usuário')
const quantidadeItens = computed(() => armazenamentoPedido.quantidadeItens)

function sair() {
  armazenamentoUsuario.sair()
  roteador.push('/login')
}
</script>

<style scoped>
.navbar {
  background: linear-gradient(90deg, #e67e22 0%, #d35400 100%);
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.15);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.navbar-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 60px;
}

.navbar-logo {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  color: #fff;
  font-weight: 700;
  font-size: 1.2rem;
}

.logo-icon {
  font-size: 1.5rem;
}

.logo-texto {
  display: none;
}

@media (min-width: 600px) {
  .logo-texto {
    display: inline;
  }
}

.navbar-toggle {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 40px;
  height: 40px;
  background: rgba(255, 255, 255, 0.1);
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.hamburger {
  width: 20px;
  height: 2px;
  background: #fff;
  position: relative;
}

.hamburger::before,
.hamburger::after {
  content: '';
  position: absolute;
  width: 20px;
  height: 2px;
  background: #fff;
  left: 0;
}

.hamburger::before {
  top: -6px;
}

.hamburger::after {
  top: 6px;
}

@media (min-width: 768px) {
  .navbar-toggle {
    display: none;
  }
}

.navbar-menu {
  display: none;
  position: absolute;
  top: 60px;
  left: 0;
  right: 0;
  background: #d35400;
  flex-direction: column;
  padding: 1rem;
  gap: 0.5rem;
}

.navbar-menu--aberto {
  display: flex;
}

@media (min-width: 768px) {
  .navbar-menu {
    display: flex;
    position: static;
    flex-direction: row;
    background: transparent;
    padding: 0;
    gap: 0.5rem;
    align-items: center;
  }
}

.navbar-link {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.6rem 1rem;
  color: #fff;
  text-decoration: none;
  border-radius: 6px;
  font-weight: 500;
  transition: background 0.2s;
}

.navbar-link:hover {
  background: rgba(255, 255, 255, 0.15);
}

.navbar-link.router-link-active {
  background: rgba(255, 255, 255, 0.2);
}

.link-icon {
  font-size: 1rem;
}

.navbar-carrinho {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.6rem 1rem;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 6px;
  color: #fff;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.2s;
}

.navbar-carrinho:hover {
  background: rgba(255, 255, 255, 0.3);
}

.carrinho-icon {
  font-size: 1.1rem;
}

.carrinho-badge {
  background: #c0392b;
  padding: 0.1rem 0.5rem;
  border-radius: 10px;
  font-size: 0.8rem;
}

.navbar-usuario {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-left: 0.5rem;
  padding-left: 0.75rem;
  border-left: 1px solid rgba(255, 255, 255, 0.3);
}

@media (max-width: 767px) {
  .navbar-usuario {
    margin-left: 0;
    padding-left: 0;
    border-left: none;
    margin-top: 0.5rem;
    padding-top: 0.5rem;
    border-top: 1px solid rgba(255, 255, 255, 0.3);
  }
}

.usuario-nome {
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.9rem;
}

.botao-sair {
  padding: 0.4rem 0.8rem;
  background: rgba(192, 57, 43, 0.8);
  border: none;
  border-radius: 4px;
  color: #fff;
  font-size: 0.85rem;
  cursor: pointer;
  transition: background 0.2s;
}

.botao-sair:hover {
  background: #c0392b;
}
</style>
