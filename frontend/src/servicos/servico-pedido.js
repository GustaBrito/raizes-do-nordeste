import clienteHttp from './cliente-http'

const ServicoPedido = {
  async realizarPedido(dadosPedido) {
    return clienteHttp.post('/pedidos', dadosPedido)
  },

  async consultarStatus(numeroPedido) {
    return clienteHttp.get(`/pedidos/${numeroPedido}/status`)
  },

  async listarPorCliente(clienteId) {
    return clienteHttp.get(`/clientes/${clienteId}/pedidos`)
  }
}

export default ServicoPedido
