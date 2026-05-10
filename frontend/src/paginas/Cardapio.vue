<template>
  <div class="pagina-cardapio">
    <div class="cardapio-hero">
      <h1>Nosso Cardápio</h1>
      <p>Sabores autênticos do Nordeste brasileiro</p>
    </div>

    <div class="cardapio-conteudo">
      <section class="lista-produtos">
        <article
          v-for="produto in produtos"
          :key="produto.id"
          class="produto-cartao"
          :class="{ 'produto-cartao--indisponivel': !produto.disponivel }"
        >
          <div class="produto-imagem">
            <span class="produto-emoji">{{ getEmojiProduto(produto.nome) }}</span>
          </div>
          <div class="produto-info">
            <h3>{{ produto.nome }}</h3>
            <p>{{ produto.descricao }}</p>
            <div class="produto-footer">
              <strong class="produto-preco">{{ formatarMoeda(produto.preco) }}</strong>
              <button
                class="botao-adicionar"
                :disabled="!produto.disponivel"
                @click="adicionarAoCarrinho(produto)"
              >
                {{ produto.disponivel ? '+' : '✕' }}
              </button>
            </div>
          </div>
        </article>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useArmazenamentoPedido } from '../armazenamento/pedido'
import { formatarMoeda } from '../utilitarios/formatar-moeda'

const armazenamentoPedido = useArmazenamentoPedido()

const produtos = ref([
  {
    id: 'c1000000-0000-0000-0000-000000000001',
    nome: 'X-Nordeste',
    descricao: 'Hambúrguer artesanal com queijo coalho e baião de dois',
    preco: 32.90,
    disponivel: true
  },
  {
    id: 'c2000000-0000-0000-0000-000000000002',
    nome: 'Tapioca Recheada',
    descricao: 'Tapioca crocante com carne de sol e catupiry',
    preco: 18.50,
    disponivel: true
  },
  {
    id: 'c3000000-0000-0000-0000-000000000003',
    nome: 'Suco de Cajá',
    descricao: 'Suco natural de cajá',
    preco: 8.00,
    disponivel: true
  },
  {
    id: 'c4000000-0000-0000-0000-000000000004',
    nome: 'Cuscuz Nordestino',
    descricao: 'Cuscuz com manteiga e queijo',
    preco: 12.00,
    disponivel: false
  },
  {
    id: 'c5000000-0000-0000-0000-000000000005',
    nome: 'Acarajé',
    descricao: 'Bolinho de feijão fradinho com vatapá e camarão',
    preco: 15.00,
    disponivel: true
  },
  {
    id: 'c6000000-0000-0000-0000-000000000006',
    nome: 'Baião de Dois',
    descricao: 'Arroz com feijão verde, queijo coalho e nata',
    preco: 22.00,
    disponivel: true
  }
])

function adicionarAoCarrinho(produto) {
  armazenamentoPedido.adicionarItem(produto)
}

function getEmojiProduto(nome) {
  const emojis = {
    'X-Nordeste': '🍔',
    'Tapioca Recheada': '🫓',
    'Suco de Cajá': '🧃',
    'Cuscuz Nordestino': '🌽',
    'Acarajé': '🥙',
    'Baião de Dois': '🍚'
  }
  return emojis[nome] || '🍽️'
}
</script>

<style scoped>
.pagina-cardapio {
  min-height: calc(100vh - 60px);
  background: linear-gradient(180deg, #FFF8DC 0%, #FAEBD7 100%);
}

.cardapio-hero {
  background: linear-gradient(140deg, #8B4513 0%, #A0522D 100%);
  color: #fff;
  padding: 2rem 1.5rem;
  text-align: center;
}

.cardapio-hero h1 {
  font-size: 1.8rem;
  margin: 0 0 0.5rem;
}

.cardapio-hero p {
  opacity: 0.9;
  margin: 0;
}

.cardapio-conteudo {
  padding: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
}

.lista-produtos {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.25rem;
}

.produto-cartao {
  background: #fff;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  transition: transform 0.2s, box-shadow 0.2s;
}

.produto-cartao:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.12);
}

.produto-cartao--indisponivel {
  opacity: 0.5;
}

.produto-imagem {
  background: linear-gradient(130deg, #e67e22 0%, #d35400 100%);
  height: 100px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.produto-emoji {
  font-size: 3rem;
}

.produto-info {
  padding: 1rem 1.25rem 1.25rem;
}

.produto-info h3 {
  margin: 0 0 0.5rem;
  color: #8B4513;
  font-size: 1.15rem;
}

.produto-info p {
  color: #666;
  font-size: 0.9rem;
  margin: 0 0 1rem;
  line-height: 1.4;
}

.produto-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.produto-preco {
  color: #e67e22;
  font-size: 1.25rem;
}

.botao-adicionar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #e67e22 0%, #d35400 100%);
  color: #fff;
  border: none;
  font-size: 1.5rem;
  font-weight: 700;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.2s, box-shadow 0.2s;
}

.botao-adicionar:hover:not(:disabled) {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(230, 126, 34, 0.4);
}

.botao-adicionar:disabled {
  background: #ccc;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .cardapio-conteudo {
    padding: 1rem;
  }

  .lista-produtos {
    grid-template-columns: 1fr;
  }

  .cardapio-hero h1 {
    font-size: 1.5rem;
  }
}
</style>
