<template>
  <div v-if="exibindo" class="aviso-lgpd" role="banner" aria-label="Aviso de privacidade">
    <div class="aviso-lgpd__conteudo">
      <p>
        Utilizamos seus dados pessoais para processar pedidos e melhorar sua experiência,
        conforme nossa
        <a href="/politica-privacidade" target="_blank">Política de Privacidade</a>
        (Lei 13.709/2018 – LGPD).
      </p>
      <div class="aviso-lgpd__acoes">
        <button class="botao botao--primario" @click="aceitar">Aceitar</button>
        <button class="botao botao--secundario" @click="recusar">Recusar</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const exibindo = ref(false)
const emit = defineEmits(['consentimento-alterado'])

onMounted(() => {
  const consentimentoSalvo = localStorage.getItem('consentimento_lgpd')
  exibindo.value = !consentimentoSalvo
})

function aceitar() {
  localStorage.setItem('consentimento_lgpd', 'aceito')
  exibindo.value = false
  emit('consentimento-alterado', true)
}

function recusar() {
  localStorage.setItem('consentimento_lgpd', 'recusado')
  exibindo.value = false
  emit('consentimento-alterado', false)
}
</script>

<style scoped>
.aviso-lgpd {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: #1a1a2e;
  color: #fff;
  padding: 1rem;
  z-index: 1000;
}

.aviso-lgpd__conteudo {
  max-width: 800px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.aviso-lgpd__acoes {
  display: flex;
  gap: 0.5rem;
}

.botao {
  padding: 0.5rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
}

.botao--primario {
  background: #e67e22;
  color: #fff;
}

.botao--secundario {
  background: transparent;
  color: #fff;
  border: 1px solid #fff;
}
</style>
