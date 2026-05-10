import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import ServicoPedido from '../servicos/servico-pedido'

export const useArmazenamentoPedido = defineStore('pedido', () => {
  const itensCarrinho = ref([])
  const franquiaSelecionada = ref(null)
  const canalAtendimento = ref('Aplicativo')
  const carregando = ref(false)
  const erro = ref(null)
  const pedidoConfirmado = ref(null)

  const quantidadeItens = computed(() =>
    itensCarrinho.value.reduce((total, item) => total + item.quantidade, 0)
  )

  const valorTotal = computed(() =>
    itensCarrinho.value.reduce((total, item) => total + item.precoUnitario * item.quantidade, 0)
  )

  function adicionarItem(produto) {
    const itemExistente = itensCarrinho.value.find(i => i.produtoId === produto.id)

    if (itemExistente) {
      itemExistente.quantidade++
    } else {
      itensCarrinho.value.push({
        produtoId: produto.id,
        nomeProduto: produto.nome,
        quantidade: 1,
        precoUnitario: produto.preco
      })
    }
  }

  function removerItem(produtoId) {
    const indice = itensCarrinho.value.findIndex(i => i.produtoId === produtoId)
    if (indice !== -1) {
      itensCarrinho.value.splice(indice, 1)
    }
  }

  function alterarQuantidade(produtoId, novaQuantidade) {
    if (novaQuantidade <= 0) {
      removerItem(produtoId)
      return
    }

    const item = itensCarrinho.value.find(i => i.produtoId === produtoId)
    if (item) {
      item.quantidade = novaQuantidade
    }
  }

  function limparCarrinho() {
    itensCarrinho.value = []
    pedidoConfirmado.value = null
  }

  async function finalizarPedido(clienteId, metodoPagamento) {
    if (!franquiaSelecionada.value) {
      throw new Error('Selecione uma unidade antes de finalizar o pedido.')
    }
    if (itensCarrinho.value.length === 0) {
      throw new Error('Adicione ao menos um item ao carrinho.')
    }

    carregando.value = true
    erro.value = null

    try {
      const entrada = {
        clienteId,
        franquiaId: franquiaSelecionada.value.id,
        canal: canalAtendimento.value,
        metodoPagamento,
        itens: itensCarrinho.value
      }

      const resultado = await ServicoPedido.realizarPedido(entrada)
      pedidoConfirmado.value = resultado
      limparCarrinho()
      return resultado
    } catch (erroApi) {
      erro.value = erroApi.message || 'Erro ao finalizar pedido. Tente novamente.'
      throw erroApi
    } finally {
      carregando.value = false
    }
  }

  return {
    itensCarrinho,
    franquiaSelecionada,
    canalAtendimento,
    carregando,
    erro,
    pedidoConfirmado,
    quantidadeItens,
    valorTotal,
    adicionarItem,
    removerItem,
    alterarQuantidade,
    limparCarrinho,
    finalizarPedido
  }
})
