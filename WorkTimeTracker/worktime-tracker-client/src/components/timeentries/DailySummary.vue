<template>
  <div class="card">
    <h3 class="text-lg font-semibold mb-4">📊 Сводка за день</h3>
    
    <div class="flex items-center gap-4 mb-4">
      <input
        type="date"
        v-model="selectedDate"
        @change="fetchSummary"
        class="input-field max-w-xs"
      />
      <button @click="fetchSummary" class="btn-primary">
        🔄 Обновить
      </button>
    </div>
    
    <div v-if="loading" class="text-center py-8">
      <LoadingSpinner />
    </div>
    
    <div v-else-if="summary" class="text-center py-4">
      <div 
        class="inline-block p-8 rounded-2xl border-4 min-w-[200px]"
        :class="{
          'bg-yellow-50 border-yellow-400': summary.color === 'yellow',
          'bg-green-50 border-green-400': summary.color === 'green',
          'bg-red-50 border-red-400': summary.color === 'red',
        }"
      >
        <div class="text-5xl font-bold mb-2">
          {{ summary.totalHours.toFixed(1) }}ч
        </div>
        <div class="text-sm text-gray-600">
          {{ summary.statusMessage }}
        </div>
      </div>
      
      <div class="mt-4 flex justify-center">
        <div 
          class="px-6 py-2 rounded-full text-white font-semibold text-lg shadow-lg"
          :class="{
            'bg-yellow-500': summary.color === 'yellow',
            'bg-green-500': summary.color === 'green',
            'bg-red-500': summary.color === 'red',
          }"
        >
          {{ 
            summary.color === 'yellow' ? '🟡 Недостаточно' : 
            summary.color === 'green' ? '🟢 Достаточно' : 
            '🔴 Избыточно' 
          }}
        </div>
      </div>
    </div>
    
    <div v-else class="text-center text-gray-500 py-8">
      Нет данных для выбранной даты
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { timeEntriesApi } from '@/api/timeentries';
import { dateUtils } from '@/utils/dateUtils';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

export default {
  name: 'DailySummary',
  components: { LoadingSpinner },
  setup() {
    const selectedDate = ref(dateUtils.today());
    const summary = ref(null);
    const loading = ref(false);
    const error = ref(null);

    const fetchSummary = async () => {
      if (!selectedDate.value) return;
      
      loading.value = true;
      error.value = null;
      
      try {
        const response = await timeEntriesApi.getDailySummary(selectedDate.value);
        summary.value = response.data;
      } catch (err) {
        error.value = err.detail || 'Не удалось загрузить сводку';
        console.error('Error fetching summary:', err);
      } finally {
        loading.value = false;
      }
    };

    onMounted(fetchSummary);

    return {
      selectedDate,
      summary,
      loading,
      error,
      fetchSummary,
    };
  },
};
</script>