import axios from 'axios';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7174',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

apiClient.interceptors.request.use(
  (config) => {
    console.log(`[API] ${config.method?.toUpperCase()} ${config.url}`);
    return config;
  },
  (error) => {
    console.error('[API] Request error:', error);
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response) {
      const message = error.response.data?.detail || error.message;
      console.error(`[API] Error ${error.response.status}:`, message);
      
      // Показываем пользователю понятное сообщение
      const errorMessage = {
        400: 'Ошибка валидации данных',
        401: 'Требуется авторизация',
        403: 'Доступ запрещен',
        404: 'Ресурс не найден',
        500: 'Внутренняя ошибка сервера',
      }[error.response.status] || 'Произошла ошибка';
      
      error.userMessage = errorMessage;
      error.detail = message;
    } else if (error.request) {
      error.userMessage = 'Нет связи с сервером';
      error.detail = 'Проверьте подключение к интернету';
    } else {
      error.userMessage = 'Неизвестная ошибка';
      error.detail = error.message;
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;