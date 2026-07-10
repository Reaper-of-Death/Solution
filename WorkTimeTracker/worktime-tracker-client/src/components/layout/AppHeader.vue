<template>
  <header class="bg-white shadow-md">
    <nav class="container mx-auto px-4 py-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-8">
          <h1 class="text-2xl font-bold text-blue-600">⏱️ WorkTimeTracker</h1>
          
          <div class="flex gap-1">
            <router-link 
              to="/" 
              class="px-4 py-2 rounded-lg text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
              active-class="bg-blue-50 text-blue-600 font-semibold"
            >
              📊 Дашборд
            </router-link>
            <router-link 
              to="/projects" 
              class="px-4 py-2 rounded-lg text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
              active-class="bg-blue-50 text-blue-600 font-semibold"
            >
              📁 Проекты
            </router-link>
            <router-link 
              to="/tasks" 
              class="px-4 py-2 rounded-lg text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
              active-class="bg-blue-50 text-blue-600 font-semibold"
            >
              ✅ Задачи
            </router-link>
            <router-link 
              to="/time-entries" 
              class="px-4 py-2 rounded-lg text-gray-700 hover:bg-gray-100 hover:text-blue-600 transition-colors"
              active-class="bg-blue-50 text-blue-600 font-semibold"
            >
              📝 Проводки
            </router-link>
          </div>
        </div>
        
        <div class="flex items-center gap-4">
          <span class="text-sm text-gray-500">📅 {{ today }}</span>
        </div>
      </div>
    </nav>
  </header>
</template>

<script>
export default {
  name: 'AppHeader',
  data() {
    return {
      today: new Date().toLocaleDateString('ru-RU', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
      })
    }
  }
}
</script>