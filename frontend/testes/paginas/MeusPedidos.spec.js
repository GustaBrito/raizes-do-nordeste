import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import MeusPedidos from '../../src/paginas/MeusPedidos.vue'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/cliente-http', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn()
  }
}))

import clienteHttp from '../../src/servicos/cliente-http'

function criarRoteador() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/meus-pedidos', component: { template: '<div/>' } },
      { path: '/cardapio', component: { template: '<div/>' } },
      { path: '/acompanhamento/:numeroPedido', component: { template: '<div/>' } }
    ]
  })
}

describe('MeusPedidos', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
    roteador = criarRoteador()
  })

  it('deve renderizar o titulo "Meus Pedidos"', async () => {
    clienteHttp.get.mockResolvedValue({ data: [] })

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('h1.titulo').text()).toContain('Meus Pedidos')
  })

  it('deve exibir estado vazio quando nao ha pedidos', async () => {
    clienteHttp.get.mockResolvedValue({ data: [] })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.sem-pedidos').exists()).toBe(true)
    expect(wrapper.text()).toContain('Nenhum pedido encontrado')
  })

  it('deve exibir link para o cardapio no estado vazio', async () => {
    clienteHttp.get.mockResolvedValue({ data: [] })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('a.botao-cardapio').exists()).toBe(true)
  })

  it('deve exibir spinner de carregamento antes de receber dados', async () => {
    clienteHttp.get.mockImplementation(() => new Promise(() => {}))

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    expect(wrapper.find('.carregando').exists()).toBe(true)
  })

  it('deve exibir lista de pedidos quando ha pedidos', async () => {
    const pedidosMock = [
      {
        numeroPedido: 'RDN-001',
        status: 'Entregue',
        dataPedido: '2026-04-01T10:00:00Z',
        quantidadeItens: 2,
        valorTotal: 65.80
      },
      {
        numeroPedido: 'RDN-002',
        status: 'Pendente',
        dataPedido: '2026-04-02T15:00:00Z',
        quantidadeItens: 1,
        valorTotal: 32.90
      }
    ]
    clienteHttp.get.mockResolvedValue({ data: pedidosMock })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.lista-pedidos').exists()).toBe(true)
    const cards = wrapper.findAll('.card-pedido')
    expect(cards).toHaveLength(2)
  })

  it('deve exibir o numero do pedido no card', async () => {
    const pedidosMock = [
      {
        numeroPedido: 'RDN-001',
        status: 'Entregue',
        dataPedido: '2026-04-01T10:00:00Z',
        quantidadeItens: 2,
        valorTotal: 65.80
      }
    ]
    clienteHttp.get.mockResolvedValue({ data: pedidosMock })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.numero-pedido').text()).toContain('RDN-001')
  })

  it('deve exibir botao de acompanhar apenas para pedidos nao entregues e nao cancelados', async () => {
    const pedidosMock = [
      {
        numeroPedido: 'RDN-001',
        status: 'Entregue',
        dataPedido: '2026-04-01T10:00:00Z',
        quantidadeItens: 1,
        valorTotal: 32.90
      },
      {
        numeroPedido: 'RDN-002',
        status: 'Pendente',
        dataPedido: '2026-04-02T15:00:00Z',
        quantidadeItens: 1,
        valorTotal: 32.90
      }
    ]
    clienteHttp.get.mockResolvedValue({ data: pedidosMock })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    const botoes = wrapper.findAll('.botao-acompanhar')
    // Apenas o pedido Pendente deve ter botao
    expect(botoes).toHaveLength(1)
  })

  it('nao deve fazer chamada HTTP quando clienteId nao esta disponivel', async () => {
    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = null

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(clienteHttp.get).not.toHaveBeenCalled()
  })

  it('deve lidar com erro na chamada HTTP sem quebrar', async () => {
    clienteHttp.get.mockRejectedValue(new Error('Erro de rede'))

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MeusPedidos, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    // Deve renderizar sem erros, mostrando lista vazia
    expect(wrapper.find('.sem-pedidos').exists()).toBe(true)
  })
})
