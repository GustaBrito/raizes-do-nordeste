<template>
  <div class="pagina-cadastro">
    <div class="cadastro-caixa">
      <div class="cadastro-logo">🌵</div>
      <h2 class="cadastro-marca">Raízes do Nordeste</h2>
      <h1>Criar sua conta</h1>

      <form class="cadastro-formulario" @submit.prevent="realizarCadastro">
        <div class="campo">
          <label for="nome">Nome completo</label>
          <input id="nome" name="nome" v-model="formulario.nome" type="text" required placeholder="João Silva" />
        </div>

        <div class="campo">
          <label for="email">E-mail</label>
          <input id="email" name="email" v-model="formulario.email" type="email" required placeholder="joao@email.com" />
        </div>

        <div class="campo">
          <label for="telefone">Telefone</label>
          <input id="telefone" name="telefone" :value="formulario.telefone" @input="formatarTelefone" type="tel" placeholder="(11) 99999-9999" maxlength="15" />
          <span v-if="erroTelefone" class="erro-telefone" role="alert">{{ erroTelefone }}</span>
        </div>

        <div class="campo">
          <label for="senha">Senha</label>
          <input id="senha" name="senha" v-model="formulario.senha" type="password" required minlength="8" />
        </div>

        <!-- LGPD: Consentimento obrigatório para coleta de dados pessoais -->
        <div class="campo campo--checkbox">
          <input
            id="consentimento-lgpd"
            v-model="formulario.consentimentoLgpd"
            type="checkbox"
            required
          />
          <label for="consentimento-lgpd">
            Li e aceito a
            <a href="/politica-privacidade" target="_blank">Política de Privacidade</a>
            e autorizo o uso dos meus dados para fins operacionais e de fidelização, conforme a
            <strong>Lei Geral de Proteção de Dados (LGPD – Lei 13.709/2018)</strong>.
          </label>
        </div>

        <div v-if="mensagemErro" class="erro-mensagem" role="alert" aria-live="assertive">
          {{ mensagemErro }}
        </div>

        <div v-if="mensagemSucesso" class="sucesso-mensagem" role="status" aria-live="polite">
          {{ mensagemSucesso }}
        </div>

        <button type="submit" class="botao botao--primario botao--bloco" :disabled="carregando">
          {{ carregando ? 'Cadastrando...' : 'Criar conta' }}
        </button>
      </form>

      <p class="cadastro-rodape">
        Já tem conta?
        <router-link to="/login">Entrar</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import ServicoAutenticacao from '../servicos/servico-autenticacao'

const roteador = useRouter()
const carregando = ref(false)
const mensagemErro = ref('')
const mensagemSucesso = ref('')
const erroTelefone = ref('')

const formulario = reactive({
  nome: '',
  email: '',
  telefone: '',
  senha: '',
  consentimentoLgpd: false
})

function formatarTelefone(event) {
  const digitos = event.target.value.replace(/\D/g, '').slice(0, 11)
  let formatted = ''
  if (digitos.length === 0) {
    formatted = ''
  } else if (digitos.length <= 2) {
    formatted = `(${digitos}`
  } else if (digitos.length <= 6) {
    formatted = `(${digitos.slice(0, 2)}) ${digitos.slice(2)}`
  } else if (digitos.length <= 10) {
    formatted = `(${digitos.slice(0, 2)}) ${digitos.slice(2, 6)}-${digitos.slice(6)}`
  } else {
    formatted = `(${digitos.slice(0, 2)}) ${digitos.slice(2, 7)}-${digitos.slice(7)}`
  }
  formulario.telefone = formatted
  event.target.value = formatted
}

async function realizarCadastro() {
  erroTelefone.value = ''

  if (!formulario.consentimentoLgpd) {
    mensagemErro.value = 'O consentimento com a política de privacidade é obrigatório.'
    return
  }

  if (formulario.telefone && formulario.telefone.replace(/\D/g, '').length < 11) {
    erroTelefone.value = 'Telefone inválido. Use o formato (XX) XXXXX-XXXX'
    return
  }

  carregando.value = true
  mensagemErro.value = ''

  try {
    await ServicoAutenticacao.cadastrar({
      nome: formulario.nome,
      email: formulario.email,
      telefone: formulario.telefone,
      senha: formulario.senha,
      consentimentoLgpd: formulario.consentimentoLgpd
    })

    mensagemSucesso.value = 'Conta criada com sucesso! Redirecionando para o login...'
    setTimeout(() => roteador.push('/login'), 2000)
  } catch (erro) {
    mensagemErro.value = erro.message
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.pagina-cadastro {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(150deg, #8B4513 0%, #A0522D 45%, #e67e22 100%);
  padding: 1rem;
}

.cadastro-caixa {
  background: #fff;
  border-radius: 20px;
  padding: 2rem;
  width: 100%;
  max-width: 440px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.cadastro-logo {
  font-size: 3rem;
  text-align: center;
  margin-bottom: 0.25rem;
}

.cadastro-marca {
  text-align: center;
  color: #8B4513;
  font-size: 1.3rem;
  margin: 0 0 0.25rem;
  font-weight: 700;
}

h1 {
  text-align: center;
  color: #555;
  margin-bottom: 1.5rem;
  font-size: 1rem;
  font-weight: 400;
}

.cadastro-formulario {
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

.campo input[type="text"],
.campo input[type="email"],
.campo input[type="tel"],
.campo input[type="password"] {
  padding: 0.75rem 1rem;
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

.campo--checkbox {
  flex-direction: row;
  align-items: flex-start;
  gap: 0.75rem;
  background: #f9f9f9;
  padding: 1rem;
  border-radius: 10px;
  border: 1px solid #eee;
}

.campo--checkbox input {
  margin-top: 3px;
  flex-shrink: 0;
  accent-color: #e67e22;
  width: 18px;
  height: 18px;
}

.campo--checkbox label {
  font-weight: 400;
  font-size: 0.8rem;
  line-height: 1.5;
  color: #555;
}

.campo--checkbox label a {
  color: #e67e22;
}

.erro-mensagem {
  background: #fdecea;
  color: #c0392b;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  border-left: 4px solid #c0392b;
}

.sucesso-mensagem {
  background: #eafaf1;
  color: #27ae60;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  border-left: 4px solid #27ae60;
}

.botao--primario {
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
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

.cadastro-rodape {
  text-align: center;
  margin-top: 1.5rem;
  font-size: 0.9rem;
  color: #666;
}

.cadastro-rodape a {
  color: #e67e22;
  font-weight: 600;
  text-decoration: none;
}

.cadastro-rodape a:hover {
  text-decoration: underline;
}

.erro-telefone {
  color: #dc3545;
  font-size: 0.85em;
  margin-top: 4px;
  display: block;
}
</style>
