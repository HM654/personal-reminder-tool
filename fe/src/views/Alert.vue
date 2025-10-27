<script setup lang="ts">
import { computed } from 'vue'

interface Alert {
  message: string
  type: 'success' | 'failure'
  show: boolean
}

const props = defineProps<Alert>()

const bgColor = computed(() => {
  return props.type === 'success' ? '#4CAF50' : '#f44336'
})
</script>

<template>
  <Transition name="fade">
    <div v-if="show" class="alert-container" :style="{ backgroundColor: bgColor }">
      <p>{{ message }}</p>
    </div>
  </Transition>
</template>

<style scoped>
.alert-container {
  position: fixed;
  top: 5vh;
  color: white;
  padding: 1rem;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.4s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
