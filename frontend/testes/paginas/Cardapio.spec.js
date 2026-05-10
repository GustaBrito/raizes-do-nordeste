import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import Cardapio from '../../src/paginas/Cardapio.vue'
import { useArmazenamentoPedido } from '../../src/armazenamento/pedido'

vi.mock('../../src/servicos/servico-pedido', () => ({
  default: { realizarPedido: vi.fn() }
}))

describe('Cardapio', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('deve renderizar o hero do cardapio', () => {
    const wrapper = mount(Cardapio)
    expect(wrapper.find('.cardapio-hero').exists()).toBe(true)
    expect(wrapper.find('.cardapio-hero h1').text()).toBe('Nosso Cardápio')
  })

  it('deve renderizar a lista de produtos', () => {
    const wrapper = mount(Cardapio)
    const cartoes = wrapper.findAll('.produto-cartao')
    expect(cartoes.length).toBeGreaterThan(0)
  })

  it('deve renderizar 6 produtos no cardapio', () => {
    const wrapper = mount(Cardapio)
    const cartoes = wrapper.findAll('.produto-cartao')
    expect(cartoes).toHaveLength(6)
  })

  it('deve aplicar classe indisponivel em produto sem disponibilidade', () => {
    const wrapper = mount(Cardapio)
    const indisponiveis = wrapper.findAll('.produto-cartao--indisponivel')
    expect(indisponiveis.length).toBeGreaterThan(0)
  })

  it('deve desabilitar botao de adicionar para produto indisponivel', () => {
    const wrapper = mount(Cardapio)
    const botoesAdicionarDesabilitados = wrapper.findAll('.botao-adicionar[disabled]')
    expect(botoesAdicionarDesabilitados.length).toBeGreaterThan(0)
  })

  it('deve chamar adicionarItem do armazenamento ao clicar em adicionar', async () => {
    const armazenamento = useArmazenamentoPedido()
    const spyAdicionar = vi.spyOn(armazenamento, 'adicionarItem')

    const wrapper = mount(Cardapio)
    // Clica no primeiro botao habilitado
    const botoesHabilitados = wrapper.findAll('.botao-adicionar:not([disabled])')
    await botoesHabilitados[0].trigger('click')

    expect(spyAdicionar).toHaveBeenCalledOnce()
  })

  it('deve adicionar item ao carrinho ao clicar em adicionar', async () => {
    const armazenamento = useArmazenamentoPedido()
    const wrapper = mount(Cardapio)

    const botoesHabilitados = wrapper.findAll('.botao-adicionar:not([disabled])')
    await botoesHabilitados[0].trigger('click')

    expect(armazenamento.itensCarrinho).toHaveLength(1)
    expect(armazenamento.quantidadeItens).toBe(1)
  })

  it('deve incrementar quantidade ao adicionar o mesmo produto novamente', async () => {
    const armazenamento = useArmazenamentoPedido()
    const wrapper = mount(Cardapio)

    const botoesHabilitados = wrapper.findAll('.botao-adicionar:not([disabled])')
    await botoesHabilitados[0].trigger('click')
    await botoesHabilitados[0].trigger('click')

    expect(armazenamento.itensCarrinho).toHaveLength(1)
    expect(armazenamento.itensCarrinho[0].quantidade).toBe(2)
  })

  it('deve exibir emoji correspondente ao produto X-Nordeste', () => {
    const wrapper = mount(Cardapio)
    const emojis = wrapper.findAll('.produto-emoji')
    const textos = emojis.map(e => e.text())
    expect(textos).toContain('🍔')
  })

  it('deve formatar preco do produto em reais', () => {
    const wrapper = mount(Cardapio)
    const precos = wrapper.findAll('.produto-preco')
    expect(precos[0].text()).toMatch(/R\$/)
  })

  it('deve mostrar "+" no botao de produto disponivel', () => {
    const wrapper = mount(Cardapio)
    const botoesHabilitados = wrapper.findAll('.botao-adicionar:not([disabled])')
    expect(botoesHabilitados[0].text()).toBe('+')
  })

  it('deve mostrar "x" no botao de produto indisponivel', () => {
    const wrapper = mount(Cardapio)
    const botoesDesabilitados = wrapper.findAll('.botao-adicionar[disabled]')
    expect(botoesDesabilitados[0].text()).toBe('✕')
  })
})
