<template>
  <div class="border rounded-lg p-4 hover:shadow-lg transition-shadow bg-white">
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <h3 class="font-semibold text-lg text-gray-900">{{ task.name }}</h3>
        <p class="text-sm text-gray-500">
          Проект: <span class="font-medium">{{ task.projectName }}</span>
          <span class="text-xs text-gray-400 ml-2">({{ task.projectCode }})</span>
        </p>
        <p class="text-xs text-gray-400 mt-1">
          Создан: {{ formatDate(task.createdAt) }}
        </p>
      </div>
      <StatusBadge :active="task.isActive" />
    </div>
    
    <div class="mt-4 flex justify-end gap-2">
      <button 
        @click="$emit('edit')" 
        class="text-sm text-blue-600 hover:text-blue-800 font-medium"
      >
        ✏️ Редактировать
      </button>
      <button 
        @click="$emit('delete')" 
        class="text-sm text-red-600 hover:text-red-800 font-medium"
      >
        🗑️ Удалить
      </button>
    </div>
  </div>
</template>

<script>
import StatusBadge from '@/components/common/StatusBadge.vue';
import { dateUtils } from '@/utils/dateUtils';

export default {
  name: 'TaskCard',
  components: { StatusBadge },
  props: {
    task: {
      type: Object,
      required: true,
    },
  },
  emits: ['edit', 'delete'],
  methods: {
    formatDate(date) {
      return dateUtils.formatDisplay(date);
    },
  },
};
</script>