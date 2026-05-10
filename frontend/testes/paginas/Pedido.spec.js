import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import Pedido from '../../src/paginas/Pedido.vue'
import { useArmazenamentoPedido } from '../../src/armazenamento/pedido'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/servico-pedido', () => ({
  default: {
    realizarPedido: vi.fn(),
    consultarStatus: vi.fn(),
    listarPorCliente: vi.fn()
  }
}))

function criarRoteador() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/pedido', component: { template: '<div/>' } },
      { path: '/acompanhamento/:numeroPedido', component: { template: '<div/>' } }
    ]
  })
}

describe('Pedido', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
    roteador = criarRoteador()
  })

  it('deve renderizar o titulo "Finalizar Pedido"', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('.pedido-hero h1').text()).toBe('Finalizar Pedido')
  })

  it('deve renderizar lista de franquias', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    const opcoes = wrapper.findAll('.franquia-opcao')
    expect(opcoes.length).toBeGreaterThan(0)
  })

  it('deve exibir as 4 franquias disponíveis', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    const opcoes = wrapper.findAll('.franquia-opcao')
    expect(opcoes).toHaveLength(4)
  })

  it('deve renderizar opcoes de pagamento', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    const opcoes = wrapper.findAll('.pagamento-opcao')
    expect(opcoes.length).toBeGreaterThan(0)
  })

  it('deve exibir os 4 metodos de pagamento', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    const opcoes = wrapper.findAll('.pagamento-opcao')
    expect(opcoes).toHaveLength(4)
  })

  it('deve ter cartao_credito selecionado por padrao', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    const inputCartaoCredito = wrapper.find('input[value="cartao_credito"]')
    expect(inputCartaoCredito.element.checked).toBe(true)
  })

  it('deve exibir mensagem de erro do store quando existir', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.erro = 'Selecione uma unidade antes de finalizar o pedido.'

    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    expect(wrapper.find('.erro-mensagem').exists()).toBe(true)
    expect(wrapper.find('.erro-mensagem').text()).toContain('Selecione uma unidade')
  })

  it('deve renderizar o componente CarrinhoResumo', async () => {
    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    // CarrinhoResumo renderiza dentro de .pedido-secao
    const secoes = wrapper.findAll('.pedido-secao')
    expect(secoes.length).toBeGreaterThanOrEqual(3)
  })

  it('deve chamar finalizarPedido ao receber evento finalizar do CarrinhoResumo', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]
    armazenamentoPedido.franquiaSelecionada = {
      id: 'a1000000-0000-0000-0000-000000000001',
      nome: 'Fortaleza Centro'
    }

    const armazenamentoUsuario = useArmazenamentoUsuario()
    armazenamentoUsuario.dadosUsuario = { clienteId: 'cli-1' }

    const finalizarPedidoSpy = vi.spyOn(armazenamentoPedido, 'finalizarPedido')
    finalizarPedidoSpy.mockResolvedValue({ numeroPedido: 'RDN-001' })

    const wrapper = mount(Pedido, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    // Simula o evento emitido pelo CarrinhoResumo
    await wrapper.findComponent({ name: 'CarrinhoResumo' }).vm.$emit('finalizar')
    await flushPromises()

    expect(finalizarPedidoSpy).toHaveBeenCalled()
  })
})
