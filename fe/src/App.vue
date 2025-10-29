<script setup lang="ts">
import { tryLogin, isLoggedIn, isCheckingAuth } from './auth';
import Home from './views/Home.vue';
import Login from './views/Login.vue';
import Alert from '@/components/Alert.vue'
import { useAlert } from './composables/alert';

tryLogin()

const { showAlert, alertMessage, alertType } = useAlert()
</script>

<template>
  <span v-if="isCheckingAuth" class="loader"></span>
  <Login v-else-if="!isLoggedIn" />
  <Home v-else />
    <Alert :show="showAlert" :message="alertMessage" :type="alertType" />
</template>

<style scoped>
.loader {
  position: absolute;
  width: 48px;
  height: 48px;
  border: 5px solid #FFF;
  border-bottom-color: transparent;
  border-radius: 50%;
  display: inline-block;
  box-sizing: border-box;
  animation: rotation 1s linear infinite;
}

@keyframes rotation {
  100% {transform: rotate(360deg);}
} 
</style>
