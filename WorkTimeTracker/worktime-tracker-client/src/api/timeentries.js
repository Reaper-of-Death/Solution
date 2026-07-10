import apiClient from './axios';

export const timeEntriesApi = {
  // Получить все проводки с фильтрацией
  getAll: (params = {}) => apiClient.get('/api/timeentries', { params }),
  
  // Получить проводку по ID
  getById: (id) => apiClient.get(`/api/timeentries/${id}`),
  
  // Создать проводку
  create: (data) => apiClient.post('/api/timeentries', data),
  
  // Обновить проводку
  update: (id, data) => apiClient.put(`/api/timeentries/${id}`, data),
  
  // Сменить задачу
  changeTask: (id, taskId) => apiClient.patch(`/api/timeentries/${id}/task`, { taskId }),
  
  // Удалить проводку
  delete: (id) => apiClient.delete(`/api/timeentries/${id}`),
  
  // Дневная сводка
  getDailySummary: (date) => apiClient.get('/api/timeentries/summary/daily', { 
    params: { date } 
  }),
  
  // Месячная сводка
  getMonthlySummary: (year, month) => apiClient.get('/api/timeentries/summary/monthly', { 
    params: { year, month } 
  }),
};