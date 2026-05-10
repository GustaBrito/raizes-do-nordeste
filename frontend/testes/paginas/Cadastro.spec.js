import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import Cadastro from '../../src/paginas/Cadastro.vue'

vi.mock('../../src/servicos/servico-autenticacao', () => ({
  default: { cadastrar: vi.fn() }
}))

import ServicoAutenticacao from '../../src/servicos/servico-autenticacao'

function criarRoteador() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', component: { template: '<div/>' } },
      { path: '/login', component: { template: '<div/>' } },
      { path: '/cadastro', component: { template: '<div/>' } }
    ]
  })
}

describe('Cadastro', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
    roteador = criarRoteador()
  })

  it('deve renderizar o formulario de cadastro', async () => {
    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('form.cadastro-formulario').exists()).toBe(true)
    expect(wrapper.find('#nome').exists()).toBe(true)
    expect(wrapper.find('#email').exists()).toBe(true)
    expect(wrapper.find('#senha').exists()).toBe(true)
    expect(wrapper.find('#telefone').exists()).toBe(true)
  })

  it('deve renderizar checkbox de consentimento LGPD', async () => {
    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('#consentimento-lgpd').exists()).toBe(true)
  })

  it('deve renderizar o botao de criar conta', async () => {
    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('button[type="submit"]').text()).toContain('Criar conta')
  })

  it('deve exibir mensagem de erro quando consentimento LGPD nao foi marcado', async () => {
    ServicoAutenticacao.cadastrar.mockResolvedValue({})

    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#nome').setValue('Joao Silva')
    await wrapper.find('#email').setValue('joao@email.com')
    await wrapper.find('#senha').setValue('senha123')
    // nao marca o checkbox
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(wrapper.find('.erro-mensagem').exists()).toBe(true)
    expect(wrapper.find('.erro-mensagem').text()).toContain('consentimento')
    expect(ServicoAutenticacao.cadastrar).not.toHaveBeenCalled()
  })

  it('deve chamar ServicoAutenticacao.cadastrar ao submeter formulario valido', async () => {
    ServicoAutenticacao.cadastrar.mockResolvedValue({})

    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#nome').setValue('Joao Silva')
    await wrapper.find('#email').setValue('joao@email.com')
    await wrapper.find('#telefone').setValue('(11) 99999-9999')
    await wrapper.find('#senha').setValue('senha123')
    await wrapper.find('#consentimento-lgpd').setValue(true)
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(ServicoAutenticacao.cadastrar).toHaveBeenCalledWith({
      nome: 'Joao Silva',
      email: 'joao@email.com',
      telefone: '(11) 99999-9999',
      senha: 'senha123',
      consentimentoLgpd: true
    })
  })

  it('deve exibir mensagem de sucesso apos cadastro bem-sucedido', async () => {
    ServicoAutenticacao.cadastrar.mockResolvedValue({})
    vi.useFakeTimers()

    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#nome').setValue('Joao Silva')
    await wrapper.find('#email').setValue('joao@email.com')
    await wrapper.find('#senha').setValue('senha123')
    await wrapper.find('#consentimento-lgpd').setValue(true)
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(wrapper.find('.sucesso-mensagem').exists()).toBe(true)
    expect(wrapper.find('.sucesso-mensagem').text()).toContain('sucesso')

    vi.useRealTimers()
  })

  it('deve exibir mensagem de erro quando o cadastro falha', async () => {
    ServicoAutenticacao.cadastrar.mockRejectedValue(new Error('E-mail ja cadastrado.'))

    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#nome').setValue('Joao Silva')
    await wrapper.find('#email').setValue('joao@email.com')
    await wrapper.find('#senha').setValue('senha123')
    await wrapper.find('#consentimento-lgpd').setValue(true)
    await wrapper.find('form').trigger('submit.prevent')
    await flushPromises()

    expect(wrapper.find('.erro-mensagem').exists()).toBe(true)
    expect(wrapper.find('.erro-mensagem').text()).toContain('E-mail ja cadastrado.')
  })

  it('deve desabilitar botao enquanto esta carregando', async () => {
    ServicoAutenticacao.cadastrar.mockImplementation(() => new Promise(() => {}))

    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()

    await wrapper.find('#nome').setValue('Joao Silva')
    await wrapper.find('#email').setValue('joao@email.com')
    await wrapper.find('#senha').setValue('senha123')
    await wrapper.find('#consentimento-lgpd').setValue(true)
    wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()

    const botao = wrapper.find('button[type="submit"]')
    expect(botao.attributes('disabled')).toBeDefined()
  })

  it('deve exibir link para pagina de login', async () => {
    const wrapper = mount(Cadastro, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    expect(wrapper.find('a[href="/login"]').exists()).toBe(true)
  })
})
