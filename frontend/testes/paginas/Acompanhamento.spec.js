import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import Acompanhamento from '../../src/paginas/Acompanhamento.vue'

vi.mock('../../src/servicos/servico-pedido', () => ({
  default: {
    consultarStatus: vi.fn(),
    realizarPedido: vi.fn(),
    listarPorCliente: vi.fn()
  }
}))

import ServicoPedido from '../../src/servicos/servico-pedido'

function criarRoteador(numeroPedido = 'RDN-001') {
  return createRouter({
    history: createWebHistory(),
    routes: [
      {
        path: '/acompanhamento/:numeroPedido',
        component: { template: '<div/>' }
      }
    ]
  })
}

describe('Acompanhamento', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
  })

  async function montarComRota(numeroPedido = 'RDN-001') {
    roteador = createRouter({
      history: createWebHistory(),
      routes: [
        {
          path: '/acompanhamento/:numeroPedido',
          component: Acompanhamento
        }
      ]
    })
    roteador.push(`/acompanhamento/${numeroPedido}`)
    await roteador.isReady()

    const wrapper = mount(Acompanhamento, {
      global: {
        plugins: [roteador],
        mocks: {
          $route: { params: { numeroPedido } }
        }
      }
    })
    return wrapper
  }

  it('deve renderizar o titulo "Acompanhe seu Pedido"', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 32.90
    })

    const wrapper = await montarComRota('RDN-001')
    expect(wrapper.find('.acompanhamento-hero h1').text()).toBe('Acompanhe seu Pedido')
  })

  it('deve exibir estado de carregamento antes de receber dados', async () => {
    ServicoPedido.consultarStatus.mockImplementation(() => new Promise(() => {}))

    const wrapper = await montarComRota('RDN-001')
    expect(wrapper.find('[role="status"]').exists()).toBe(true)
    expect(wrapper.text()).toContain('Carregando status do pedido')
  })

  it('deve exibir mensagem de erro quando a consulta falha', async () => {
    ServicoPedido.consultarStatus.mockRejectedValue(new Error('Erro de rede'))

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    expect(wrapper.find('[role="alert"]').exists()).toBe(true)
    expect(wrapper.find('.erro-mensagem').text()).toContain('possível')
  })

  it('deve exibir numero do pedido apos carregamento', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 32.90
    })

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    expect(wrapper.find('.pedido-numero strong').text()).toContain('RDN-001')
  })

  it('deve exibir status do pedido', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 32.90
    })

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    expect(wrapper.find('.status-badge').exists()).toBe(true)
    expect(wrapper.find('.status-badge').text()).toContain('EmPreparacao')
  })

  it('deve exibir etapas do pedido', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 32.90
    })

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    const etapas = wrapper.findAll('.etapa')
    expect(etapas).toHaveLength(4)
  })

  it('deve exibir valor total do pedido', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 65.80
    })

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    expect(wrapper.find('.pedido-valor').text()).toContain('65')
  })

  it('deve exibir pontos adicionados quando houver pontosAdicionados', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'Entregue',
      valorTotal: 32.90,
      pontosAdicionados: 30
    })

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    expect(wrapper.find('.pedido-pontos').exists()).toBe(true)
    expect(wrapper.find('.pedido-pontos').text()).toContain('30')
  })

  it('nao deve exibir secao de pontos quando pontosAdicionados e zero/nulo', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 32.90,
      pontosAdicionados: 0
    })

    const wrapper = await montarComRota('RDN-001')
    await flushPromises()

    expect(wrapper.find('.pedido-pontos').exists()).toBe(false)
  })

  it('deve chamar consultarStatus com o numeroPedido da rota', async () => {
    ServicoPedido.consultarStatus.mockResolvedValue({
      numeroPedido: 'RDN-999',
      status: 'Entregue',
      valorTotal: 45.00
    })

    const wrapper = await montarComRota('RDN-999')
    await flushPromises()

    expect(ServicoPedido.consultarStatus).toHaveBeenCalledWith('RDN-999')
  })
})
