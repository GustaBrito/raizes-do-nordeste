import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import CarrinhoResumo from '../../src/componentes/CarrinhoResumo.vue'
import { useArmazenamentoPedido } from '../../src/armazenamento/pedido'

vi.mock('../../src/servicos/servico-pedido', () => ({
  default: {
    realizarPedido: vi.fn(),
    consultarStatus: vi.fn(),
    listarPorCliente: vi.fn()
  }
}))

describe('CarrinhoResumo', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('deve renderizar o titulo "Seu Pedido"', () => {
    const wrapper = mount(CarrinhoResumo)
    expect(wrapper.find('h3').text()).toBe('Seu Pedido')
  })

  it('deve exibir mensagem de carrinho vazio quando nao ha itens', () => {
    const wrapper = mount(CarrinhoResumo)
    expect(wrapper.find('.carrinho-resumo__vazio').exists()).toBe(true)
    expect(wrapper.text()).toContain('Nenhum item adicionado')
  })

  it('deve exibir lista de itens quando o carrinho tem produtos', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 2, precoUnitario: 32.90 },
      { produtoId: 'p2', nomeProduto: 'Cuscuz', quantidade: 1, precoUnitario: 15.00 }
    ]

    const wrapper = mount(CarrinhoResumo)
    const itens = wrapper.findAll('.carrinho-resumo__item')
    expect(itens).toHaveLength(2)
  })

  it('deve exibir o nome do produto no carrinho', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]

    const wrapper = mount(CarrinhoResumo)
    expect(wrapper.find('.carrinho-resumo__nome').text()).toBe('Baiao de dois')
  })

  it('deve exibir a quantidade do item', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 3, precoUnitario: 32.90 }
    ]

    const wrapper = mount(CarrinhoResumo)
    expect(wrapper.find('.carrinho-resumo__controles span').text()).toBe('3')
  })

  it('deve exibir o total calculado corretamente', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 2, precoUnitario: 32.90 },
      { produtoId: 'p2', nomeProduto: 'Cuscuz', quantidade: 1, precoUnitario: 15.00 }
    ]

    const wrapper = mount(CarrinhoResumo)
    // Total esperado: 2*32.90 + 1*15.00 = 80.80
    expect(wrapper.find('.carrinho-resumo__total').text()).toContain('80')
  })

  it('deve desabilitar o botao de finalizar quando o carrinho esta vazio', () => {
    const wrapper = mount(CarrinhoResumo)
    const botao = wrapper.find('.botao--finalizar')
    expect(botao.attributes('disabled')).toBeDefined()
  })

  it('deve habilitar o botao de finalizar quando ha itens no carrinho', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]

    const wrapper = mount(CarrinhoResumo)
    const botao = wrapper.find('.botao--finalizar')
    expect(botao.attributes('disabled')).toBeUndefined()
  })

  it('deve emitir evento "finalizar" ao clicar no botao', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]

    const wrapper = mount(CarrinhoResumo)
    await wrapper.find('.botao--finalizar').trigger('click')

    expect(wrapper.emitted('finalizar')).toBeTruthy()
    expect(wrapper.emitted('finalizar')).toHaveLength(1)
  })

  it('deve exibir botoes de aumentar e diminuir quantidade para cada item', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]

    const wrapper = mount(CarrinhoResumo)
    const botoes = wrapper.find('.carrinho-resumo__controles').findAll('button')
    expect(botoes).toHaveLength(2)
    expect(botoes[0].attributes('aria-label')).toBe('Diminuir quantidade')
    expect(botoes[1].attributes('aria-label')).toBe('Aumentar quantidade')
  })

  it('deve chamar alterarQuantidade ao clicar em aumentar', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]

    const alterarQuantidadeSpy = vi.spyOn(armazenamentoPedido, 'alterarQuantidade')

    const wrapper = mount(CarrinhoResumo)
    const botaoAumentar = wrapper.find('button[aria-label="Aumentar quantidade"]')
    await botaoAumentar.trigger('click')

    expect(alterarQuantidadeSpy).toHaveBeenCalledWith('p1', 2)
  })

  it('deve chamar alterarQuantidade ao clicar em diminuir', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 2, precoUnitario: 32.90 }
    ]

    const alterarQuantidadeSpy = vi.spyOn(armazenamentoPedido, 'alterarQuantidade')

    const wrapper = mount(CarrinhoResumo)
    const botaoDiminuir = wrapper.find('button[aria-label="Diminuir quantidade"]')
    await botaoDiminuir.trigger('click')

    expect(alterarQuantidadeSpy).toHaveBeenCalledWith('p1', 1)
  })

  it('deve desabilitar botao de finalizar quando o store esta carregando', () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.itensCarrinho = [
      { produtoId: 'p1', nomeProduto: 'Baiao de dois', quantidade: 1, precoUnitario: 32.90 }
    ]
    armazenamentoPedido.carregando = true

    const wrapper = mount(CarrinhoResumo)
    const botao = wrapper.find('.botao--finalizar')
    expect(botao.attributes('disabled')).toBeDefined()
    expect(botao.text()).toContain('Processando')
  })
})
