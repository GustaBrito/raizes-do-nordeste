import axios from 'axios'

const clienteHttp = axios.create({
  baseURL: import.meta.env.VITE_URL_API || 'http://localhost:5000/api',
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' }
})

// Interceptor de requisição: injeta o token JWT no cabeçalho Authorization
clienteHttp.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Interceptor de resposta: extrai dados, trata 401 e normaliza erros
clienteHttp.interceptors.response.use(
  (resposta) => resposta.data,
  (erro) => {
    if (erro.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
      return Promise.reject(new Error('Sessão expirada. Faça login novamente.'))
    }
    const mensagem = erro.response?.data?.mensagem || 'Erro de comunicação com o servidor.'
    return Promise.reject(new Error(mensagem))
  }
)

export default clienteHttp
