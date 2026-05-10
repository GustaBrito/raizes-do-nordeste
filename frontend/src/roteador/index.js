import { createRouter, createWebHistory } from 'vue-router'
import { useArmazenamentoUsuario } from '../armazenamento/usuario'

const rotas = [
  { path: '/', redirect: '/cardapio' },
  { path: '/login', component: () => import('../paginas/Login.vue'), meta: { publica: true } },
  { path: '/cadastro', component: () => import('../paginas/Cadastro.vue'), meta: { publica: true } },
  { path: '/cardapio', component: () => import('../paginas/Cardapio.vue'), meta: { requerAutenticacao: true } },
  { path: '/pedido', component: () => import('../paginas/Pedido.vue'), meta: { requerAutenticacao: true } },
  { path: '/acompanhamento/:numeroPedido', component: () => import('../paginas/Acompanhamento.vue'), meta: { requerAutenticacao: true } },
  { path: '/minha-conta', component: () => import('../paginas/MinhaConta.vue'), meta: { requerAutenticacao: true } },
  { path: '/meus-pedidos', component: () => import('../paginas/MeusPedidos.vue'), meta: { requerAutenticacao: true } },
  { path: '/:pathMatch(.*)*', component: () => import('../paginas/NaoEncontrado.vue'), meta: { publica: true } }
]

const roteador = createRouter({
  history: createWebHistory(),
  routes: rotas
})

roteador.beforeEach((destino) => {
  const armazenamentoUsuario = useArmazenamentoUsuario()
  const usuarioLogado = armazenamentoUsuario.estaAutenticado

  if (destino.meta.requerAutenticacao && !usuarioLogado) {
    return '/login'
  }
})

export default roteador
