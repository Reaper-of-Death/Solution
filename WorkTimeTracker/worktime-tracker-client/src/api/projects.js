import apiClient from './axios';

export const projectsApi = {
  // Получить все проекты
  getAll: () => apiClient.get('/api/projects'),
  
  // Получить проект по ID
  getById: (id) => apiClient.get(`/api/projects/${id}`),
  
  // Создать проект
  create: (data) => apiClient.post('/api/projects', data),
  
  // Обновить проект
  update: (id, data) => apiClient.put(`/api/projects/${id}`, data),
  
  // Удалить проект
  delete: (id) => apiClient.delete(`/api/projects/${id}`),
};