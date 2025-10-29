import { ref } from 'vue'

const showAlert = ref(false)
const alertMessage = ref('')
const alertType = ref<'success' | 'failure'>('success')

export function useAlert() {
  function displayAlert(message: string, type: 'success' | 'failure') {
    alertMessage.value = message
    alertType.value = type
    showAlert.value = true
    setTimeout(() => (showAlert.value = false), 3000)
  }

  return {
    showAlert,
    alertMessage,
    alertType,
    displayAlert
  }
}