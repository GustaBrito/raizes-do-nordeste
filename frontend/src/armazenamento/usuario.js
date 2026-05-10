import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import ServicoAutenticacao from '../servicos/servico-autenticacao'

export const useArmazenamentoUsuario = defineStore('usuario', () => {
  const dadosUsuarioSalvo = localStorage.getItem('dadosUsuario')
  const dadosUsuario = ref(dadosUsuarioSalvo ? JSON.parse(dadosUsuarioSalvo) : null)
  const tokenAcesso = ref(localStorage.getItem('token') || null)
  const carregando = ref(false)
  const erro = ref(null)

  const estaAutenticado = computed(() => !!tokenAcesso.value)
  const nomeUsuario = computed(() => dadosUsuario.value?.nome || '')

  async function entrar(email, senha) {
    carregando.value = true
    erro.value = null

    try {
      const resposta = await ServicoAutenticacao.autenticar(email, senha)

      // Normalizar resposta para camelCase (backend C# retorna PascalCase)
      const dadosNormalizados = {
        token: resposta.token ?? resposta.Token,
        expiraEm: resposta.expiraEm ?? resposta.ExpiraEm,
        clienteId: resposta.clienteId ?? resposta.ClienteId,
        nome: resposta.nome ?? resposta.Nome,
        email: resposta.email ?? resposta.Email
      }

      tokenAcesso.value = dadosNormalizados.token
      dadosUsuario.value = dadosNormalizados
      localStorage.setItem('token', dadosNormalizados.token)
      localStorage.setItem('dadosUsuario', JSON.stringify(dadosNormalizados))
    } catch (erroApi) {
      erro.value = erroApi.message || 'Credenciais inválidas. Tente novamente.'
      throw erroApi
    } finally {
      carregando.value = false
    }
  }

  function sair() {
    dadosUsuario.value = null
    tokenAcesso.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('dadosUsuario')
  }

  return { dadosUsuario, tokenAcesso, carregando, erro, estaAutenticado, nomeUsuario, entrar, sair }
})
