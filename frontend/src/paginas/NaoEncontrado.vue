<template>
  <div class="pagina-nao-encontrada">
    <div class="nao-encontrada-conteudo">
      <h1 class="codigo-erro">404</h1>
      <p class="mensagem-erro">Página não encontrada.</p>
      <p class="descricao-erro">
        O endereço que você tentou acessar não existe ou foi movido.
      </p>
      <router-link to="/" class="botao botao--primario">
        Voltar ao início
      </router-link>
    </div>
  </div>
</template>

<style scoped>
.pagina-nao-encontrada {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f0e8;
  padding: 1rem;
}

.nao-encontrada-conteudo {
  text-align: center;
}

.codigo-erro {
  font-size: 6rem;
  font-weight: 800;
  color: #e67e22;
  margin: 0;
  line-height: 1;
}

.mensagem-erro {
  font-size: 1.5rem;
  color: #2c3e50;
  margin: 0.5rem 0;
}

.descricao-erro {
  color: #888;
  margin-bottom: 2rem;
}

.botao--primario {
  display: inline-block;
  background: #e67e22;
  color: #fff;
  text-decoration: none;
  padding: 0.75rem 2rem;
  border-radius: 6px;
  font-weight: 600;
  font-size: 1rem;
}
</style>
