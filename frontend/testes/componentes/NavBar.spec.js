import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import NavBar from '../../src/componentes/NavBar.vue'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'
import { useArmazenamentoPedido } from '../../src/armazenamento/pedido'

vi.mock('../../src/servicos/servico-autenticacao', () => ({
  default: { autenticar: vi.fn() }
}))

vi.mock('../../src/servicos/servico-pedido', () => ({
  default: { realizarPedido: vi.fn() }
}))

function criarRoteador() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', component: { template: '<div/>' } },
      { path: '/login', component: { template: '<div/>' } },
      { path: '/cardapio', component: { template: '<div/>' } },
      { path: '/meus-pedidos', component: { template: '<div/>' } },
      { path: '/minha-conta', component: { template: '<div/>' } }
    ]
  })
}

describe('NavBar', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
    roteador = criarRoteador()
  })

  it('deve renderizar o componente navbar', async () => {
    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('nav.navbar').exists()).toBe(true)
  })

  it('deve exibir nome do usuario do armazenamento', async () => {
    const armazenamentoUsuario = useArmazenamentoUsuario()
    armazenamentoUsuario.dadosUsuario = { nome: 'Gustavo', email: 'g@g.com', token: 'tok', clienteId: '1' }
    armazenamentoUsuario.tokenAcesso = 'tok'

    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.text()).toContain('Gustavo')
  })

  it('deve exibir "Usuario" quando nao ha nome no armazenamento', async () => {
    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('.usuario-nome').text()).toBe('Usuário')
  })

  it('deve alternar menuAberto ao clicar no botao toggle', async () => {
    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    const toggle = wrapper.find('.navbar-toggle')
    expect(wrapper.find('.navbar-menu--aberto').exists()).toBe(false)
    await toggle.trigger('click')
    expect(wrapper.find('.navbar-menu--aberto').exists()).toBe(true)
    await toggle.trigger('click')
    expect(wrapper.find('.navbar-menu--aberto').exists()).toBe(false)
  })

  it('ao clicar em Sair deve chamar sair() do armazenamento e redirecionar para /login', async () => {
    const armazenamentoUsuario = useArmazenamentoUsuario()
    const spySair = vi.spyOn(armazenamentoUsuario, 'sair')

    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('.botao-sair').trigger('click')
    await flushPromises()
    expect(spySair).toHaveBeenCalledOnce()
    expect(roteador.currentRoute.value.path).toBe('/login')
  })

  it('nao deve exibir botao do carrinho quando nao ha itens', async () => {
    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('.navbar-carrinho').exists()).toBe(false)
  })

  it('deve exibir botao do carrinho com badge quando ha itens', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })

    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('.navbar-carrinho').exists()).toBe(true)
    expect(wrapper.find('.carrinho-badge').text()).toBe('1')
  })

  it('deve emitir abrirCarrinho ao clicar no carrinho', async () => {
    const armazenamentoPedido = useArmazenamentoPedido()
    armazenamentoPedido.adicionarItem({ id: '1', nome: 'X-Nordeste', preco: 32.90 })

    const wrapper = mount(NavBar, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await wrapper.find('.navbar-carrinho').trigger('click')
    expect(wrapper.emitted('abrirCarrinho')).toBeTruthy()
  })
})
