<template>
  <div class="pagina-login">
    <div class="login-caixa">
      <div class="login-logo">🌵</div>
      <h2 class="login-marca">Raízes do Nordeste</h2>
      <h1>Bem-vindo de volta!</h1>

      <form class="login-formulario" @submit.prevent="realizarLogin">
        <div class="campo">
          <label for="email">E-mail</label>
          <input
            id="email"
            v-model="email"
            type="email"
            placeholder="seu@email.com"
            required
            autocomplete="email"
          />
        </div>

        <div class="campo">
          <label for="senha">Senha</label>
          <input
            id="senha"
            v-model="senha"
            type="password"
            placeholder="••••••••"
            required
            autocomplete="current-password"
          />
        </div>

        <div v-if="armazenamentoUsuario.erro" class="erro-mensagem" role="alert">
          {{ armazenamentoUsuario.erro }}
        </div>

        <button
          type="submit"
          class="botao botao--primario botao--bloco"
          :disabled="armazenamentoUsuario.carregando"
        >
          {{ armazenamentoUsuario.carregando ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>

      <p class="login-rodape">
        Ainda não tem conta?
        <router-link to="/cadastro">Cadastre-se</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useArmazenamentoUsuario } from '../armazenamento/usuario'

const email = ref('')
const senha = ref('')
const roteador = useRouter()
const armazenamentoUsuario = useArmazenamentoUsuario()

async function realizarLogin() {
  try {
    await armazenamentoUsuario.entrar(email.value, senha.value)
    roteador.push('/cardapio')
  } catch {
    // Erro já gerenciado no armazenamento
  }
}
</script>

<style scoped>
.pagina-login {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(145deg, #8B4513 0%, #A0522D 50%, #e67e22 100%);
  padding: 1rem;
}

.login-caixa {
  background: #fff;
  border-radius: 20px;
  padding: 2.5rem 2rem;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.login-logo {
  font-size: 4rem;
  text-align: center;
  margin-bottom: 0.5rem;
}

.login-marca {
  text-align: center;
  color: #8B4513;
  font-size: 1.4rem;
  margin: 0 0 0.5rem;
  font-weight: 700;
}

h1 {
  text-align: center;
  color: #555;
  margin-bottom: 1.5rem;
  font-size: 1.1rem;
  font-weight: 400;
}

.login-formulario {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.campo {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.campo label {
  font-weight: 600;
  color: #8B4513;
  font-size: 0.85rem;
}

.campo input {
  padding: 0.8rem 1rem;
  border: 2px solid #eee;
  border-radius: 10px;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.campo input:focus {
  outline: none;
  border-color: #e67e22;
  box-shadow: 0 0 0 3px rgba(230, 126, 34, 0.1);
}

.erro-mensagem {
  background: #fdecea;
  color: #c0392b;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  border-left: 4px solid #c0392b;
}

.botao--primario {
  background: linear-gradient(120deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  border: none;
  border-radius: 10px;
  padding: 0.9rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.botao--primario:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(230, 126, 34, 0.4);
}

.botao--primario:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.botao--bloco {
  width: 100%;
}

.login-rodape {
  text-align: center;
  margin-top: 1.5rem;
  font-size: 0.9rem;
  color: #666;
}

.login-rodape a {
  color: #e67e22;
  font-weight: 600;
  text-decoration: none;
}

.login-rodape a:hover {
  text-decoration: underline;
}
</style>
