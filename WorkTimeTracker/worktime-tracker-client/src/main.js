import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import './style.css'

// Определяем маршруты
const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: () => import('./views/Dashboard.vue'),
  },
  {
    path: '/projects',
    name: 'Projects',
    component: () => import('./views/ProjectsView.vue'),
  },
  {
    path: '/tasks',
    name: 'Tasks',
    component: () => import('./views/TasksView.vue'),
  },
  {
    path: '/time-entries',
    name: 'TimeEntries',
    component: () => import('./views/TimeEntriesView.vue'),
  },
]

// Создаем роутер
const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Создаем приложение
const app = createApp(App)
app.use(router)
app.mount('#app')