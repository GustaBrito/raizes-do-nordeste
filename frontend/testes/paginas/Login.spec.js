import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import Login from '../../src/paginas/Login.vue'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/servico-autenticacao', () => ({
  default: { autenticar: vi.fn() }
}))

import ServicoAutenticacao from '../../src/servicos/servico-autenticacao'

function criarRoteador() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', component: { template: '<div/>' } },
      { path: '/login', component: { template: '<div/>' } },
      { path: '/cardapio', component: { template: '<div/>' } },
      { path: '/cadastro', component: { template: '<div/>' } }
    ]
  })
}

describe('Login', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
    roteador = criarRoteador()
  })

  it('deve renderizar o formulario de login', async () => {
    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('form.login-formulario').exists()).toBe(true)
    expect(wrapper.find('#email').exists()).toBe(true)
    expect(wrapper.find('#senha').exists()).toBe(true)
    expect(wrapper.find('button[type="submit"]').exists()).toBe(true)
  })

  it('deve renderizar texto do botao como "Entrar" quando nao esta carregando', async () => {
    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('button[type="submit"]').text()).toBe('Entrar')
  })

  it('deve chamar autenticar do servico ao submeter o formulario', async () => {
    ServicoAutenticacao.autenticar.mockResolvedValue({
      token: 'tok', nome: 'Gustavo', email: 'g@g.com', clienteId: '1'
    })

    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#email').setValue('gustavo@email.com')
    await wrapper.find('#senha').setValue('senha123')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(ServicoAutenticacao.autenticar).toHaveBeenCalledWith('gustavo@email.com', 'senha123')
  })

  it('deve redirecionar para /cardapio apos login com sucesso', async () => {
    ServicoAutenticacao.autenticar.mockResolvedValue({
      token: 'tok', nome: 'Gustavo', email: 'g@g.com', clienteId: '1'
    })

    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#email').setValue('gustavo@email.com')
    await wrapper.find('#senha').setValue('senha123')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(roteador.currentRoute.value.path).toBe('/cardapio')
  })

  it('deve exibir mensagem de erro quando login falha', async () => {
    ServicoAutenticacao.autenticar.mockRejectedValue(new Error('Credenciais inválidas.'))

    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#email').setValue('errado@email.com')
    await wrapper.find('#senha').setValue('senhaerrada')
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(wrapper.find('.erro-mensagem').exists()).toBe(true)
    expect(wrapper.find('.erro-mensagem').text()).toContain('Credenciais inválidas.')
  })

  it('deve desabilitar o botao quando esta carregando', async () => {
    // Simula estado carregando via store
    ServicoAutenticacao.autenticar.mockImplementation(
      () => new Promise(() => {}) // promise que nunca resolve
    )

    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#email').setValue('g@g.com')
    await wrapper.find('#senha').setValue('senha123')
    // Dispara submit mas não aguarda — deixa em estado carregando
    wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()

    const botao = wrapper.find('button[type="submit"]')
    expect(botao.attributes('disabled')).toBeDefined()
  })

  it('deve exibir link para pagina de cadastro', async () => {
    const wrapper = mount(Login, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('a[href="/cadastro"]').exists()).toBe(true)
  })
})
