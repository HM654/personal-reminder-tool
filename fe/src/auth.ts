import { ref } from "vue";

export const accessToken = ref<string | null>(null)
export const isLoggedIn = ref<boolean>(false)
export const isCheckingAuth = ref<boolean>(true)

export async function tryLogin(){
  try {
    const { default: api } = await import('./api')
    const result = await api.post('/refresh')
    isLoggedIn.value = result.status === 200
  } catch (error) {
    isLoggedIn.value = false
  } finally {
    isCheckingAuth.value = false
  }
}

export function setAccessToken(token: string) {
  accessToken.value = token
  isLoggedIn.value = true
}

export function clearAccessToken() {
  accessToken.value = null
  isLoggedIn.value = false
}