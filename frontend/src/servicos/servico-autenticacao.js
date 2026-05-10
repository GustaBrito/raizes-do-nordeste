import clienteHttp from './cliente-http'

const ServicoAutenticacao = {
  async autenticar(email, senha) {
    return clienteHttp.post('/autenticacao/login', { email, senha })
  },

  async cadastrar(dadosCliente) {
    return clienteHttp.post('/clientes', dadosCliente)
  }
}

export default ServicoAutenticacao
