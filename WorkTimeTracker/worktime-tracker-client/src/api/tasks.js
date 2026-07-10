import apiClient from './axios';

export const tasksApi = {
  // Получить все задачи
  getAll: () => apiClient.get('/api/tasks'),
  
  // Получить активные задачи
  getActive: () => apiClient.get('/api/tasks/active'),
  
  // Получить задачи по проекту
  getByProject: (projectId) => apiClient.get(`/api/tasks/by-project/${projectId}`),
  
  // Получить задачу по ID
  getById: (id) => apiClient.get(`/api/tasks/${id}`),
  
  // Создать задачу
  create: (data) => apiClient.post('/api/tasks', data),
  
  // Обновить задачу
  update: (id, data) => apiClient.put(`/api/tasks/${id}`, data),
  
  // Удалить задачу
  delete: (id) => apiClient.delete(`/api/tasks/${id}`),
};