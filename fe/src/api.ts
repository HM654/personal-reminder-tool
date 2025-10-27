import axios from 'axios'
import { accessToken, setAccessToken, clearAccessToken } from "./auth";

const baseConfig = {
  baseURL: `${import.meta.env.VITE_API_URL}/api`,
  withCredentials: true
};

const api = axios.create(baseConfig);
const refreshApi = axios.create(baseConfig);

api.interceptors.request.use((config) => {
  if (accessToken.value) {
    config.headers.Authorization = `Bearer ${accessToken.value}`;
  }
  return config;
});

api.interceptors.response.use(undefined, async (error) => {
    const originalRequest = error.config;
    const isLoginEndpoint = originalRequest.url?.includes('/login');
    const isRefreshEndpoint = originalRequest.url?.includes('/refresh');

    if (error.response.status === 401 && !originalRequest._retry && !isLoginEndpoint && !isRefreshEndpoint){
      originalRequest._retry = true

      try {
        const newToken = await refreshApi.post('/refresh');
        setAccessToken(newToken.data)
        originalRequest.headers.Authorization = `Bearer ${newToken.data}`;

        return api(originalRequest);
      } catch (refreshError) {
        clearAccessToken()
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api
