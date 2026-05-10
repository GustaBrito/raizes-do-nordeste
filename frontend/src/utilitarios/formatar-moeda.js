/**
 * Formata um valor numérico para o padrão monetário brasileiro (BRL).
 * @param {number} valor - Valor a ser formatado
 * @returns {string} Valor formatado, ex: "R$ 32,90"
 */
export function formatarMoeda(valor) {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(valor)
}

/**
 * Calcula o subtotal de um item multiplicando preço pela quantidade.
 * @param {number} precoUnitario - Preço unitário do produto
 * @param {number} quantidade - Quantidade do produto
 * @returns {number} Subtotal calculado
 */
export function calcularSubtotal(precoUnitario, quantidade) {
  return precoUnitario * quantidade
}
