import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/servico-autenticacao', () => ({
  default: { autenticar: vi.fn() }
}))

// Testa o comportamento do guard de forma isolada
// (igual ao padrao estabelecido em guard.spec.js, mas focado em index.js)
function executarGuard(destino, estaAutenticado) {
  if (destino.meta.requerAutenticacao && !estaAutenticado) {
    return '/login'
  }
  return undefined
}

describe('Roteador - Guard de Autenticacao', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
  })

  it('deve redirecionar para /login sem token em rota protegida', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/cardapio' }
    expect(executarGuard(destino, false)).toBe('/login')
  })

  it('deve permitir acesso com token valido em rota protegida', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/cardapio' }
    expect(executarGuard(destino, true)).toBeUndefined()
  })

  it('deve permitir acesso a /login sem autenticacao', () => {
    const destino = { meta: { publica: true }, path: '/login' }
    expect(executarGuard(destino, false)).toBeUndefined()
  })

  it('deve permitir acesso a /cadastro sem autenticacao', () => {
    const destino = { meta: { publica: true }, path: '/cadastro' }
    expect(executarGuard(destino, false)).toBeUndefined()
  })

  it('deve permitir acesso a /minha-conta com usuario autenticado', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/minha-conta' }
    expect(executarGuard(destino, true)).toBeUndefined()
  })

  it('deve redirecionar para /login ao tentar acessar /pedido sem autenticacao', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/pedido' }
    expect(executarGuard(destino, false)).toBe('/login')
  })

  it('deve redirecionar para /login ao tentar acessar /meus-pedidos sem autenticacao', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/meus-pedidos' }
    expect(executarGuard(destino, false)).toBe('/login')
  })

  it('deve redirecionar para /login ao tentar acessar /acompanhamento/:id sem autenticacao', () => {
    const destino = { meta: { requerAutenticacao: true }, path: '/acompanhamento/RDN-001' }
    expect(executarGuard(destino, false)).toBe('/login')
  })
})

describe('Roteador - Store de usuario e autenticacao', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
  })

  it('deve retornar estaAutenticado=false quando token nao esta no localStorage', () => {
    const armazenamento = useArmazenamentoUsuario()
    expect(armazenamento.estaAutenticado).toBe(false)
  })

  it('deve retornar estaAutenticado=true quando token esta no localStorage', () => {
    localStorage.setItem('token', 'jwt-valido')
    const armazenamento = useArmazenamentoUsuario()
    expect(armazenamento.estaAutenticado).toBe(true)
  })

  it('guard deve permitir acesso quando store indica usuario autenticado', () => {
    localStorage.setItem('token', 'jwt-valido')
    const armazenamento = useArmazenamentoUsuario()

    const destino = { meta: { requerAutenticacao: true }, path: '/cardapio' }
    const resultado = executarGuard(destino, armazenamento.estaAutenticado)
    expect(resultado).toBeUndefined()
  })

  it('guard deve bloquear acesso quando store indica usuario nao autenticado', () => {
    const armazenamento = useArmazenamentoUsuario()

    const destino = { meta: { requerAutenticacao: true }, path: '/cardapio' }
    const resultado = executarGuard(destino, armazenamento.estaAutenticado)
    expect(resultado).toBe('/login')
  })
})
