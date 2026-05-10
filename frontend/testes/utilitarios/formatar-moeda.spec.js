import { describe, it, expect } from 'vitest'
import { formatarMoeda, calcularSubtotal } from '../../src/utilitarios/formatar-moeda'

describe('formatarMoeda', () => {
  it('deve formatar valor em reais corretamente', () => {
    expect(formatarMoeda(32.90)).toBe('R$\u00a032,90')
  })

  it('deve formatar valor zero como R$ 0,00', () => {
    expect(formatarMoeda(0)).toBe('R$\u00a00,00')
  })

  it('deve formatar valores grandes corretamente', () => {
    const resultado = formatarMoeda(1000)
    expect(resultado).toContain('1.000')
  })
})

describe('calcularSubtotal', () => {
  it('deve multiplicar preco por quantidade corretamente', () => {
    expect(calcularSubtotal(25.90, 2)).toBe(51.80)
  })

  it('deve retornar zero quando quantidade e zero', () => {
    expect(calcularSubtotal(25.90, 0)).toBe(0)
  })
})
