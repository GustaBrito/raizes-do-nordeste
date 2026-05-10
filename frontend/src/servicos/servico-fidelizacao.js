import clienteHttp from './cliente-http'

const ServicoFidelizacao = {
  async consultarPontos(clienteId) {
    return clienteHttp.get(`/fidelizacao/${clienteId}`)
  },

  async resgatar(clienteId, quantidade) {
    return clienteHttp.post('/fidelizacao/resgatar', { clienteId, quantidadePontos: quantidade })
  }
}

export default ServicoFidelizacao
