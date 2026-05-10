import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/servico-autenticacao', () => ({
  default: {
    autenticar: vi.fn()
  }
}))

import ServicoAutenticacao from '../../src/servicos/servico-autenticacao'

describe('ArmazenamentoUsuario', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
  })

  it('deve iniciar como nao autenticado quando nao ha token', () => {
    const armazenamento = useArmazenamentoUsuario()
    expect(armazenamento.estaAutenticado).toBe(false)
    expect(armazenamento.nomeUsuario).toBe('')
  })

  it('deve autenticar e armazenar token com credenciais validas', async () => {
    ServicoAutenticacao.autenticar.mockResolvedValue({
      token: 'jwt-token-teste',
      nome: 'Gustavo',
      email: 'gustavo@email.com',
      clienteId: '12345'
    })

    const armazenamento = useArmazenamentoUsuario()
    await armazenamento.entrar('gustavo@email.com', 'senha123')

    expect(armazenamento.estaAutenticado).toBe(true)
    expect(armazenamento.nomeUsuario).toBe('Gustavo')
    expect(localStorage.getItem('token')).toBe('jwt-token-teste')
  })

  it('deve registrar erro e relancar excecao com credenciais invalidas', async () => {
    ServicoAutenticacao.autenticar.mockRejectedValue(new Error('Credenciais inválidas.'))

    const armazenamento = useArmazenamentoUsuario()

    await expect(armazenamento.entrar('errado@email.com', 'senhaerrada'))
      .rejects.toThrow()

    expect(armazenamento.estaAutenticado).toBe(false)
    expect(armazenamento.erro).toBeTruthy()
  })

  it('deve definir carregando como false apos conclusao do login', async () => {
    ServicoAutenticacao.autenticar.mockResolvedValue({
      token: 'token',
      nome: 'Gustavo',
      email: 'gustavo@email.com'
    })

    const armazenamento = useArmazenamentoUsuario()
    await armazenamento.entrar('gustavo@email.com', 'senha123')

    expect(armazenamento.carregando).toBe(false)
  })

  it('deve limpar dados e token ao sair', async () => {
    ServicoAutenticacao.autenticar.mockResolvedValue({
      token: 'jwt-token-teste',
      nome: 'Gustavo',
      email: 'gustavo@email.com'
    })

    const armazenamento = useArmazenamentoUsuario()
    await armazenamento.entrar('gustavo@email.com', 'senha123')
    armazenamento.sair()

    expect(armazenamento.estaAutenticado).toBe(false)
    expect(armazenamento.nomeUsuario).toBe('')
    expect(localStorage.getItem('token')).toBeNull()
  })

  it('deve ser autenticado quando token existe no localStorage', () => {
    localStorage.setItem('token', 'token-existente')
    const armazenamento = useArmazenamentoUsuario()
    expect(armazenamento.estaAutenticado).toBe(true)
  })

  it('deve normalizar resposta PascalCase da API C# corretamente', async () => {
    // Backend C# retorna PascalCase: Token, Nome, ClienteId, Email, ExpiraEm
    ServicoAutenticacao.autenticar.mockResolvedValue({
      Token: 'jwt-pascal-token',
      Nome: 'Gustavo PascalCase',
      Email: 'gustavo@email.com',
      ClienteId: 'cli-pascal-123',
      ExpiraEm: '2026-12-31T00:00:00Z'
    })

    const armazenamento = useArmazenamentoUsuario()
    await armazenamento.entrar('gustavo@email.com', 'senha123')

    expect(armazenamento.estaAutenticado).toBe(true)
    expect(armazenamento.nomeUsuario).toBe('Gustavo PascalCase')
    expect(localStorage.getItem('token')).toBe('jwt-pascal-token')
  })

  it('deve restaurar dados do usuario ja existentes no localStorage', () => {
    const dadosSalvos = {
      token: 'token-salvo',
      nome: 'Usuario Salvo',
      email: 'salvo@email.com',
      clienteId: 'cli-salvo-1'
    }
    localStorage.setItem('token', 'token-salvo')
    localStorage.setItem('dadosUsuario', JSON.stringify(dadosSalvos))

    const armazenamento = useArmazenamentoUsuario()

    expect(armazenamento.estaAutenticado).toBe(true)
    expect(armazenamento.nomeUsuario).toBe('Usuario Salvo')
    expect(armazenamento.dadosUsuario.clienteId).toBe('cli-salvo-1')
  })
})
