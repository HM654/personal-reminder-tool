<script setup lang="ts">
import { ref, watch } from 'vue';

enum Platforms {
  None = 0,
  SMS = 1 << 0,
  Email = 1 << 1
}

const platforms = [
  { value: Platforms.SMS, label: 'SMS' },
  { value: Platforms.Email, label: 'Email' }
]

const emit = defineEmits<{
  (e: 'update:platforms', value: number): void;
}>();

const selected = ref<Platforms[]>([]);

watch(selected, () => {
  const bitmask = selected.value.reduce((acc, val) => acc | val, Platforms.None);
  emit('update:platforms', bitmask);
});
</script>

<template>
  <div class="section">
    <h2>Platforms</h2>
    <div class="platforms-container">
      <label 
        v-for="platform in platforms" 
        :key="platform.value" 
        class="platform-checkbox"
      >
        <input
          type="checkbox"
          :value="platform.value"
          v-model="selected"
          class="checkbox-input"
        />
        <span class="checkbox-custom"></span>
        <span class="checkbox-label">{{ platform.label }}</span>
      </label>
    </div>
  </div>
</template>

<style scoped>

.platforms-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.platform-checkbox {
  display: flex;
  align-items: center;
  padding: 0.75rem;
  background: #f8f8f8;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
}

.platform-checkbox:hover {
  border-color: #eb820b;
}

.checkbox-input {
  position: absolute;
  opacity: 0;
}

.checkbox-custom {
  width: 20px;
  height: 20px;
  border: 2px solid #e0e0e0;
  border-radius: 4px;
  margin-right: 0.75rem;
  position: relative;
  transition: 0.2s ease;
  background: white;
  flex-shrink: 0;
}

.checkbox-input:checked ~ .checkbox-custom {
  background: #eb820b;
  border-color: #eb820b;
}

.checkbox-input:checked ~ .checkbox-custom::after {
  content: '';
  position: absolute;
  left: 6px;
  top: 2px;
  width: 5px;
  height: 10px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.checkbox-label {
  color: #333333;
  font-size: 1rem;
  font-weight: 500;
  user-select: none;
}

.platform-checkbox:has(.checkbox-input:checked) {
  background: #fff5eb;
  border-color: #eb820b;
}
</style>
