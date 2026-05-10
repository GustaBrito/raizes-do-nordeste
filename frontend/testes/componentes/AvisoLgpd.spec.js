import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import AvisoLgpd from '../../src/componentes/AvisoLgpd.vue'

describe('AvisoLgpd', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
  })

  it('deve exibir o aviso quando nao ha consentimento salvo', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    expect(wrapper.find('.aviso-lgpd').exists()).toBe(true)
  })

  it('nao deve exibir o aviso quando consentimento ja foi salvo', async () => {
    localStorage.setItem('consentimento_lgpd', 'aceito')
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    expect(wrapper.find('.aviso-lgpd').exists()).toBe(false)
  })

  it('deve renderizar botoes de aceitar e recusar', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    const botoes = wrapper.findAll('button')
    expect(botoes).toHaveLength(2)
    expect(botoes[0].text()).toBe('Aceitar')
    expect(botoes[1].text()).toBe('Recusar')
  })

  it('ao aceitar deve salvar "aceito" no localStorage e ocultar o aviso', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    await wrapper.find('button.botao--primario').trigger('click')
    expect(localStorage.getItem('consentimento_lgpd')).toBe('aceito')
    expect(wrapper.find('.aviso-lgpd').exists()).toBe(false)
  })

  it('ao recusar deve salvar "recusado" no localStorage e ocultar o aviso', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    await wrapper.find('button.botao--secundario').trigger('click')
    expect(localStorage.getItem('consentimento_lgpd')).toBe('recusado')
    expect(wrapper.find('.aviso-lgpd').exists()).toBe(false)
  })

  it('ao aceitar deve emitir evento consentimento-alterado com true', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    await wrapper.find('button.botao--primario').trigger('click')
    expect(wrapper.emitted('consentimento-alterado')).toBeTruthy()
    expect(wrapper.emitted('consentimento-alterado')[0]).toEqual([true])
  })

  it('ao recusar deve emitir evento consentimento-alterado com false', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    await wrapper.find('button.botao--secundario').trigger('click')
    expect(wrapper.emitted('consentimento-alterado')).toBeTruthy()
    expect(wrapper.emitted('consentimento-alterado')[0]).toEqual([false])
  })

  it('deve conter link para politica de privacidade', async () => {
    const wrapper = mount(AvisoLgpd)
    await flushPromises()
    const link = wrapper.find('a[href="/politica-privacidade"]')
    expect(link.exists()).toBe(true)
  })
})
