<template>
  <div class="border rounded-lg p-4 hover:shadow-lg transition-shadow bg-white">
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <div class="flex items-center gap-3">
          <span 
            class="text-2xl font-bold"
            :class="{
              'text-green-600': entry.hours === 8,
              'text-yellow-600': entry.hours < 8,
              'text-red-600': entry.hours > 8,
            }"
          >
            {{ entry.hours.toFixed(1) }}ч
          </span>
          <span class="text-sm text-gray-500">
            {{ formatDate(entry.date) }}
          </span>
        </div>
        <h4 class="font-semibold mt-1">{{ entry.taskName }}</h4>
        <p class="text-sm text-gray-500">
          Проект: {{ entry.projectName }}
        </p>
        <p v-if="entry.description" class="text-sm text-gray-600 mt-1">
          {{ entry.description }}
        </p>
        <p class="text-xs text-gray-400 mt-1">
          Обновлено: {{ formatDateTime(entry.updatedAt) }}
        </p>
      </div>
      <div class="flex gap-2">
        <button 
          @click="$emit('edit')" 
          class="text-blue-600 hover:text-blue-800"
          title="Редактировать"
        >
          ✏️
        </button>
        <button 
          @click="$emit('delete')" 
          class="text-red-600 hover:text-red-800"
          title="Удалить"
        >
          🗑️
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { dateUtils } from '@/utils/dateUtils';

export default {
  name: 'TimeEntryCard',
  props: {
    entry: {
      type: Object,
      required: true,
    },
  },
  emits: ['edit', 'delete'],
  methods: {
    formatDate(date) {
      return dateUtils.formatDisplay(date);
    },
    formatDateTime(date) {
      return dateUtils.formatDateTime(date);
    },
  },
};
</script>