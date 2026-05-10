import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import MinhaConta from '../../src/paginas/MinhaConta.vue'
import { useArmazenamentoUsuario } from '../../src/armazenamento/usuario'

vi.mock('../../src/servicos/servico-fidelizacao', () => ({
  default: {
    consultarPontos: vi.fn(),
    resgatar: vi.fn()
  }
}))

import ServicoFidelizacao from '../../src/servicos/servico-fidelizacao'

function criarRoteador() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/minha-conta', component: { template: '<div/>' } },
      { path: '/login', component: { template: '<div/>' } }
    ]
  })
}

describe('MinhaConta', () => {
  let roteador

  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
    roteador = criarRoteador()
    // window.alert pode ser chamado nesta pagina
    vi.stubGlobal('alert', vi.fn())
  })

  it('deve renderizar a pagina sem erros quando usuario esta logado', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 150 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1', email: 'joao@email.com' }
    localStorage.setItem('nomeUsuario', 'Joao Silva')

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.pagina-minha-conta').exists()).toBe(true)
  })

  it('deve exibir pontos de fidelidade carregados do servico', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 250 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1', email: 'joao@email.com' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.pontos-valor').text()).toBe('250')
  })

  it('deve exibir 0 pontos quando servico falha', async () => {
    ServicoFidelizacao.consultarPontos.mockRejectedValue(new Error('Erro'))

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.pontos-valor').text()).toBe('0')
  })

  it('deve desabilitar botao de resgatar quando pontos sao menores que 100', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 50 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    const botaoResgatar = wrapper.find('.botao-resgatar')
    expect(botaoResgatar.attributes('disabled')).toBeDefined()
  })

  it('deve habilitar botao de resgatar quando pontos sao 100 ou mais', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 100 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    const botaoResgatar = wrapper.find('.botao-resgatar')
    expect(botaoResgatar.attributes('disabled')).toBeUndefined()
  })

  it('deve exibir modal de resgate ao clicar em Resgatar', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 200 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    await wrapper.find('.botao-resgatar').trigger('click')
    expect(wrapper.find('.modal-resgate').exists()).toBe(true)
  })

  it('deve fechar modal de resgate ao clicar em Cancelar', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 200 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    await wrapper.find('.botao-resgatar').trigger('click')
    expect(wrapper.find('.modal-resgate').exists()).toBe(true)

    await wrapper.find('.botao-cancelar').trigger('click')
    expect(wrapper.find('.modal-resgate').exists()).toBe(false)
  })

  it('deve exibir dialogo de confirmacao de exclusao ao clicar no botao', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 0 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    await wrapper.find('.botao--perigo').trigger('click')
    expect(wrapper.find('.confirmacao-exclusao').exists()).toBe(true)
  })

  it('deve fechar dialogo de confirmacao ao clicar em Cancelar', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 0 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    await wrapper.find('.botao--perigo').trigger('click')
    expect(wrapper.find('.confirmacao-exclusao').exists()).toBe(true)

    await wrapper.find('.confirmacao-exclusao .botao--secundario').trigger('click')
    expect(wrapper.find('.confirmacao-exclusao').exists()).toBe(false)
  })

  it('deve exibir secao de direitos LGPD', async () => {
    ServicoFidelizacao.consultarPontos.mockResolvedValue({ saldoPontos: 0 })

    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = { clienteId: 'cli-1' }

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(wrapper.find('.lgpd-secao').exists()).toBe(true)
    expect(wrapper.find('.lgpd-lista').exists()).toBe(true)
  })

  it('nao deve consultar pontos quando clienteId nao esta disponivel', async () => {
    const armazenamento = useArmazenamentoUsuario()
    armazenamento.dadosUsuario = null

    const wrapper = mount(MinhaConta, {
      global: { plugins: [roteador] }
    })
    await roteador.isReady()
    await flushPromises()

    expect(ServicoFidelizacao.consultarPontos).not.toHaveBeenCalled()
  })
})
