<script setup lang="ts">
import api from '@/api'
import { computed, ref } from 'vue'
import { setAccessToken } from "../auth";
import calenderImg from '@/assets/calender.webp';
import { useAlert } from '@/composables/alert'

const { displayAlert } = useAlert()

interface LoginDto {
  email: string
  password: string
}

const email = ref('')
const password = ref('')

const fieldsPopulated = computed(() => {
  return (
    !!email.value &&
    !!password.value
  )
})

async function Submit(){
  const loginDto: LoginDto = {
    email: email.value,
    password: password.value
  }
  
  try {
    const response = await api.post('/login', loginDto)
    setAccessToken(response.data)
    displayAlert('Login successful!', 'success')
  } catch (error: any) {
    let errorMessage = 'Login failed. Please try again.'
    if (error.response?.status === 401) {
      errorMessage = 'Invalid email or password.'
    } else if (error.response?.status === 400 && error.response?.data) {
      errorMessage = error.response.data
    }
    displayAlert(errorMessage, 'failure')
  }
}
</script>

<template>
    <img :src="calenderImg" alt="calender" class="icon" />
    <div class="container">
      <div class="input-container">
        <div>
          <label for="email">Email</label>
          <input id="email" v-model="email" type="email" />
        </div>
        <div>
          <label for="password">Password</label>
          <input id="password" v-model="password" type="password" />
        </div>
      </div>
      <button :disabled="!fieldsPopulated" @click="Submit">Login</button>
    </div>
</template>

<style scoped>
.container {
  display: flex;
  flex-direction: column;
  max-width: 400px;
  width: 100%;
  padding: 2rem;
  background: #ffffff;
  border-radius: 12px;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333333;
  font-size: 0.95rem;
}

.input-container {
  display: flex;
  flex-direction: column;
  margin-bottom: 2rem;
  gap: 1rem;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-family: inherit;
  font-size: 1rem;
  transition: border-color 0.2s ease;
}

input:focus {
  outline: none;
  border-color: #eb820b;
}

button {
  width: 100%;
  padding: 1rem;
  background: #eb820b;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: 0.2s ease;
}

button:disabled {
  background: #cccccc;
  cursor: not-allowed;
  transform: none;
  opacity: 0.7;
}

button:hover {
  background: #ffa136;
}

button:active {
  transform: translateY(1px);
}
</style>
