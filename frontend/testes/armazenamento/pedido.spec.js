import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useArmazenamentoPedido } from '../../src/armazenamento/pedido'

vi.mock('../../src/servicos/servico-pedido', () => ({
  default: {
    realizarPedido: vi.fn().mockResolvedValue({
      pedidoId: 'uuid-teste',
      numeroPedido: 'RDN-001',
      status: 'EmPreparacao',
      valorTotal: 51.80,
      pontosAdicionados: 51
    })
  }
}))

describe('ArmazenamentoPedido', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('deve iniciar com carrinho vazio', () => {
    const armazenamento = useArmazenamentoPedido()
    expect(armazenamento.itensCarrinho).toHaveLength(0)
    expect(armazenamento.valorTotal).toBe(0)
  })

  it('deve adicionar item ao carrinho corretamente', () => {
    const armazenamento = useArmazenamentoPedido()
    const produto = { id: '1', nome: 'X-Nordeste', preco: 32.90 }

    armazenamento.adicionarItem(produto)

    expect(armazenamento.itensCarrinho).toHaveLength(1)
    expect(armazenamento.itensCarrinho[0].quantidade).toBe(1)
  })

  it('deve incrementar quantidade ao adicionar item ja existente', () => {
    const armazenamento = useArmazenamentoPedido()
    const produto = { id: '1', nome: 'X-Nordeste', preco: 32.90 }

    armazenamento.adicionarItem(produto)
    armazenamento.adicionarItem(produto)

    expect(armazenamento.itensCarrinho).toHaveLength(1)
    expect(armazenamento.itensCarrinho[0].quantidade).toBe(2)
  })

  it('deve calcular valor total corretamente', () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })
    armazenamento.adicionarItem({ id: '2', nome: 'Suco', preco: 8.00 })

    expect(armazenamento.valorTotal).toBe(40.90)
  })

  it('deve remover item do carrinho', () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })

    armazenamento.removerItem('1')

    expect(armazenamento.itensCarrinho).toHaveLength(0)
  })

  it('deve limpar carrinho apos finalizar pedido com sucesso', async () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })
    armazenamento.franquiaSelecionada = { id: 'franquia-1', nome: 'Fortaleza Centro' }

    await armazenamento.finalizarPedido('cliente-1', 'cartao_credito')

    expect(armazenamento.itensCarrinho).toHaveLength(0)
  })

  it('deve lancar erro ao finalizar pedido sem franquia selecionada', async () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })

    await expect(armazenamento.finalizarPedido('cliente-1', 'cartao_credito'))
      .rejects.toThrow('Selecione uma unidade')
  })

  it('deve remover item quando alterarQuantidade recebe valor <= 0', () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })
    expect(armazenamento.itensCarrinho).toHaveLength(1)

    armazenamento.alterarQuantidade('1', 0)

    expect(armazenamento.itensCarrinho).toHaveLength(0)
  })

  it('deve remover item quando alterarQuantidade recebe valor negativo', () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })

    armazenamento.alterarQuantidade('1', -5)

    expect(armazenamento.itensCarrinho).toHaveLength(0)
  })

  it('deve atualizar quantidade quando alterarQuantidade recebe valor positivo', () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })

    armazenamento.alterarQuantidade('1', 3)

    expect(armazenamento.itensCarrinho[0].quantidade).toBe(3)
  })

  it('deve lancar erro ao finalizar pedido com carrinho vazio', async () => {
    const armazenamento = useArmazenamentoPedido()
    armazenamento.franquiaSelecionada = { id: 'franquia-1', nome: 'Fortaleza Centro' }

    await expect(armazenamento.finalizarPedido('cliente-1', 'cartao_credito'))
      .rejects.toThrow('Adicione ao menos um item')
  })

  it('deve registrar erro e relancar excecao quando a API de pedido falha', async () => {
    const { default: ServicoPedido } = await import('../../src/servicos/servico-pedido')
    ServicoPedido.realizarPedido.mockRejectedValueOnce(new Error('Erro interno do servidor.'))

    const armazenamento = useArmazenamentoPedido()
    armazenamento.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })
    armazenamento.franquiaSelecionada = { id: 'franquia-1', nome: 'Fortaleza Centro' }

    await expect(armazenamento.finalizarPedido('cliente-1', 'cartao_credito'))
      .rejects.toThrow()

    expect(armazenamento.erro).toBeTruthy()
    expect(armazenamento.carregando).toBe(false)
  })
})
