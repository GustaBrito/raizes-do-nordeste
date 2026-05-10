import { describe, it, expect, beforeEach, vi } from 'vitest'

vi.mock('../../src/servicos/cliente-http', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn()
  }
}))

import clienteHttp from '../../src/servicos/cliente-http'
import ServicoAutenticacao from '../../src/servicos/servico-autenticacao'
import ServicoPedido from '../../src/servicos/servico-pedido'
import ServicoFidelizacao from '../../src/servicos/servico-fidelizacao'

describe('ServicoAutenticacao', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('deve chamar POST /autenticacao/login com email e senha', async () => {
    clienteHttp.post.mockResolvedValue({ token: 'tok', nome: 'Gustavo' })

    const resultado = await ServicoAutenticacao.autenticar('g@g.com', 'senha123')

    expect(clienteHttp.post).toHaveBeenCalledWith('/autenticacao/login', {
      email: 'g@g.com',
      senha: 'senha123'
    })
    expect(resultado.token).toBe('tok')
  })

  it('deve rejeitar quando o POST de login falha', async () => {
    clienteHttp.post.mockRejectedValue(new Error('Credenciais inválidas.'))

    await expect(ServicoAutenticacao.autenticar('errado@g.com', 'senhaerrada'))
      .rejects.toThrow('Credenciais inválidas.')
  })

  it('deve chamar POST /clientes com dados do cliente ao cadastrar', async () => {
    clienteHttp.post.mockResolvedValue({ clienteId: 'uuid-123' })

    const dados = { nome: 'Gustavo', email: 'g@g.com', senha: 'senha123' }
    const resultado = await ServicoAutenticacao.cadastrar(dados)

    expect(clienteHttp.post).toHaveBeenCalledWith('/clientes', dados)
    expect(resultado.clienteId).toBe('uuid-123')
  })

  it('deve rejeitar quando o POST de cadastro falha', async () => {
    clienteHttp.post.mockRejectedValue(new Error('E-mail já cadastrado.'))

    await expect(ServicoAutenticacao.cadastrar({ email: 'existente@g.com' }))
      .rejects.toThrow('E-mail já cadastrado.')
  })
})

describe('ServicoPedido', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('deve chamar POST /pedidos com dados do pedido ao realizar pedido', async () => {
    const pedidoMock = { pedidoId: 'uuid-p1', numeroPedido: 'RDN-001', status: 'EmPreparacao' }
    clienteHttp.post.mockResolvedValue(pedidoMock)

    const dados = {
      clienteId: 'cli-1',
      franquiaId: 'fran-1',
      canal: 'Aplicativo',
      metodoPagamento: 'cartao_credito',
      itens: [{ produtoId: 'p1', quantidade: 1, precoUnitario: 32.90 }]
    }

    const resultado = await ServicoPedido.realizarPedido(dados)

    expect(clienteHttp.post).toHaveBeenCalledWith('/pedidos', dados)
    expect(resultado.numeroPedido).toBe('RDN-001')
  })

  it('deve chamar GET /pedidos/:id/status para consultar status', async () => {
    clienteHttp.get.mockResolvedValue({ status: 'EmPreparacao' })

    const resultado = await ServicoPedido.consultarStatus('RDN-001')

    expect(clienteHttp.get).toHaveBeenCalledWith('/pedidos/RDN-001/status')
    expect(resultado.status).toBe('EmPreparacao')
  })

  it('deve chamar GET /clientes/:id/pedidos para listar pedidos do cliente', async () => {
    clienteHttp.get.mockResolvedValue([{ pedidoId: 'p1' }])

    const resultado = await ServicoPedido.listarPorCliente('cli-1')

    expect(clienteHttp.get).toHaveBeenCalledWith('/clientes/cli-1/pedidos')
    expect(resultado).toHaveLength(1)
  })

  it('deve rejeitar quando realizarPedido falha', async () => {
    clienteHttp.post.mockRejectedValue(new Error('Erro ao realizar pedido.'))

    await expect(ServicoPedido.realizarPedido({}))
      .rejects.toThrow('Erro ao realizar pedido.')
  })
})

describe('ServicoFidelizacao', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('deve chamar GET /fidelizacao/:clienteId para consultar pontos', async () => {
    clienteHttp.get.mockResolvedValue({ pontos: 150 })

    const resultado = await ServicoFidelizacao.consultarPontos('cli-1')

    expect(clienteHttp.get).toHaveBeenCalledWith('/fidelizacao/cli-1')
    expect(resultado.pontos).toBe(150)
  })

  it('deve chamar POST /fidelizacao/resgatar com clienteId e quantidade', async () => {
    clienteHttp.post.mockResolvedValue({ sucesso: true, pontosRestantes: 50 })

    const resultado = await ServicoFidelizacao.resgatar('cli-1', 100)

    expect(clienteHttp.post).toHaveBeenCalledWith('/fidelizacao/resgatar', {
      clienteId: 'cli-1',
      quantidadePontos: 100
    })
    expect(resultado.sucesso).toBe(true)
  })

  it('deve rejeitar quando consultarPontos falha', async () => {
    clienteHttp.get.mockRejectedValue(new Error('Cliente nao encontrado.'))

    await expect(ServicoFidelizacao.consultarPontos('cli-inexistente'))
      .rejects.toThrow('Cliente nao encontrado.')
  })

  it('deve rejeitar quando resgatar falha', async () => {
    clienteHttp.post.mockRejectedValue(new Error('Pontos insuficientes.'))

    await expect(ServicoFidelizacao.resgatar('cli-1', 9999))
      .rejects.toThrow('Pontos insuficientes.')
  })
})
