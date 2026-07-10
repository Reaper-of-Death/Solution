<template>
  <div>
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-xl font-semibold">✅ Задачи</h2>
      <button @click="$emit('add')" class="btn-primary">
        ➕ Создать задачу
      </button>
    </div>
    
    <LoadingSpinner v-if="loading" />
    
    <div v-else-if="tasks.length === 0" class="text-center py-12 text-gray-500">
      <div class="text-6xl mb-4">📭</div>
      <p>Нет задач</p>
      <p class="text-sm mt-1">Создайте первую задачу</p>
    </div>
    
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <TaskCard
        v-for="task in tasks"
        :key="task.id"
        :task="task"
        @edit="$emit('edit', task.id)"
        @delete="$emit('delete', task.id)"
      />
    </div>
  </div>
</template>

<script>
import TaskCard from './TaskCard.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

export default {
  name: 'TaskList',
  components: { TaskCard, LoadingSpinner },
  props: {
    tasks: {
      type: Array,
      required: true,
    },
    loading: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['add', 'edit', 'delete'],
};
</script>