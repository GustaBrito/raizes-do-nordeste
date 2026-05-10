import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/servico-autenticacao', () => ({
  default: { autenticar: vi.fn() }
}))

// Simula o guard do roteador de forma isolada (sem montar o router completo)
function executarGuard(destino, estaAutenticado) {
  const usuarioLogado = estaAutenticado

  if (destino.meta.requerAutenticacao && !usuarioLogado) {
    return '/login'
  }
  return undefined
}

describe('Guard de Autenticacao do Roteador', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
  })

  it('deve redirecionar para /login quando rota requer autenticacao e usuario nao esta logado', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/cardapio' }
    const resultado = executarGuard(destino, false)
    expect(resultado).toBe('/login')
  })

  it('deve permitir acesso quando usuario esta autenticado em rota protegida', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/cardapio' }
    const resultado = executarGuard(destino, true)
    expect(resultado).toBeUndefined()
  })

  it('deve permitir acesso a rotas publicas sem autenticacao', () => {
    const destino = { meta: { publica: true }, path: '/login' }
    const resultado = executarGuard(destino, false)
    expect(resultado).toBeUndefined()
  })

  it('deve permitir acesso a rota de cadastro sem autenticacao', () => {
    const destino = { meta: { publica: true }, path: '/cadastro' }
    const resultado = executarGuard(destino, false)
    expect(resultado).toBeUndefined()
  })

  it('deve refletir autenticacao via store quando token esta no localStorage', () => {
    localStorage.setItem('token', 'jwt-valido')
    const armazenamento = useArmazenamentoUsuario()
    expect(armazenamento.estaAutenticado).toBe(true)

    const destino = { meta: { requerAutenticacao: true }, path: '/minha-conta' }
    const resultado = executarGuard(destino, armazenamento.estaAutenticado)
    expect(resultado).toBeUndefined()
  })
})
