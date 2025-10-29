<script setup lang="ts">
import { computed, ref } from 'vue';
import Platforms from '../components/Platforms.vue';
import api from '@/api'
import calenderImg from '@/assets/calender.webp';
import { useAlert } from '@/composables/alert'

const { displayAlert } = useAlert()

interface ReminderDto {
  platforms: number
  reminderMessage: string
  scheduledDateTime: string
  timeZone: string
}

const platformsSelected = ref(0)
const reminderMessage = ref('')
const scheduledDateTime  = ref<string>('')
const userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

const minDateTime = computed(() => {
  const now = new Date(); 
  return new Date(now.getTime() - now.getTimezoneOffset() * 60000)
  .toISOString()
  .slice(0, 16);
});

const fieldsPopulated = computed(() => {
  return (
    platformsSelected.value > 0 &&
    !!reminderMessage.value &&
    !!scheduledDateTime .value
  )
})

function resetForm() {
  reminderMessage.value = '';
  scheduledDateTime.value = '';
}

async function Submit(){
  const reminderDto: ReminderDto = {
    platforms: platformsSelected.value,
    reminderMessage: reminderMessage.value,
    scheduledDateTime: scheduledDateTime .value,
    timeZone: userTimeZone
  }
  
  try {
    await api.post('/set-reminder', reminderDto)
    displayAlert('Reminder set successfully!', 'success');
    resetForm()
  } catch (error: any) { 
    let errorMessage = 'Failed to set reminder. Please try again.'
    if (error.response?.status === 400 && error.response?.data) {
      errorMessage = error.response?.data
      console.log(errorMessage)
    }
    displayAlert(errorMessage, 'failure');
  }
}
</script>

<template>
    <img :src="calenderImg" alt="calender" class="icon" />
    <div class="container">
      <Platforms @update:platforms="platformsSelected = $event" />
      <h2>Reminder</h2>
      <div class="reminder-container">
        <textarea v-model="reminderMessage" maxlength="160" rows="3"></textarea>
        <input type=datetime-local v-model="scheduledDateTime" :min="minDateTime"/>
      </div>
      <button :disabled="!fieldsPopulated" @click="Submit">Remind Me!</button>
    </div>
</template>

<style scoped>
.container {
  max-width: 500px;
  width: 100%;
  margin: 0 auto;
  padding: 2rem;
  background: #ffffff;
  border-radius: 12px;
}

.reminder-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 2rem;
}

textarea {
  width: 100%;
  min-height: 115px;
  padding: 0.75rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-family: inherit;
  font-size: 1rem;
  resize: none;
  transition: 0.2s ease;
}

textarea:focus {
  outline: none;
  border-color: #eb820b;
}

input[type="datetime-local"] {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-family: inherit;
  font-size: 1rem;
  transition: border-color 0.2s ease;
}

input[type="datetime-local"]:focus {
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

button:hover {
  background: #ffa136;
}

button:active {
  transform: translateY(1px);
}

button:disabled {
  background: #cccccc;
  cursor: not-allowed;
  transform: none;
  opacity: 0.7;
}
</style>
