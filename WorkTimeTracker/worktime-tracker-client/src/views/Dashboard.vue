<template>
  <div>
    <h1 class="text-2xl font-bold mb-6">📊 Дашборд</h1>
    
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <DailySummary />
      
      <div class="card">
        <h3 class="text-lg font-semibold mb-4">🚀 Быстрые действия</h3>
        <div class="space-y-3">
          <router-link to="/time-entries" class="block btn-primary text-center">
            ➕ Добавить проводку
          </router-link>
          <router-link to="/projects" class="block btn-secondary text-center">
            📋 Управление проектами
          </router-link>
          <router-link to="/tasks" class="block btn-secondary text-center">
            ✅ Управление задачами
          </router-link>
        </div>
        
        <div class="mt-6 pt-6 border-t border-gray-200">
          <h4 class="text-sm font-medium text-gray-700 mb-2">Статистика</h4>
          <div class="grid grid-cols-2 gap-4 text-center">
            <div class="bg-blue-50 rounded-lg p-3">
              <div class="text-2xl font-bold text-blue-600">{{ projectsCount }}</div>
              <div class="text-xs text-gray-500">Проектов</div>
            </div>
            <div class="bg-green-50 rounded-lg p-3">
              <div class="text-2xl font-bold text-green-600">{{ tasksCount }}</div>
              <div class="text-xs text-gray-500">Задач</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import DailySummary from '@/components/timeentries/DailySummary.vue';
import { projectsApi } from '@/api/projects';
import { tasksApi } from '@/api/tasks';

export default {
  name: 'Dashboard',
  components: { DailySummary },
  setup() {
    const projectsCount = ref(0);
    const tasksCount = ref(0);
    
    const loadStats = async () => {
      try {
        const [projectsRes, tasksRes] = await Promise.all([
          projectsApi.getAll(),
          tasksApi.getAll(),
        ]);
        projectsCount.value = projectsRes.data.length;
        tasksCount.value = tasksRes.data.length;
      } catch (error) {
        console.error('Error loading stats:', error);
      }
    };
    
    onMounted(loadStats);
    
    return {
      projectsCount,
      tasksCount,
    };
  },
};
</script>