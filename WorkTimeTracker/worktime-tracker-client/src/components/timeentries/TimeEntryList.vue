<template>
  <div>
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-xl font-semibold">📝 Проводки времени</h2>
      <button @click="$emit('add')" class="btn-primary">
        ➕ Добавить проводку
      </button>
    </div>
    
    <LoadingSpinner v-if="loading" />
    
    <div v-else-if="entries.length === 0" class="text-center py-12 text-gray-500">
      <div class="text-6xl mb-4">📭</div>
      <p>Нет проводок</p>
      <p class="text-sm mt-1">Добавьте первую проводку времени</p>
    </div>
    
    <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <TimeEntryCard
        v-for="entry in entries"
        :key="entry.id"
        :entry="entry"
        @edit="$emit('edit', entry.id)"
        @delete="$emit('delete', entry.id)"
      />
    </div>
  </div>
</template>

<script>
import TimeEntryCard from './TimeEntryCard.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

export default {
  name: 'TimeEntryList',
  components: { TimeEntryCard, LoadingSpinner },
  props: {
    entries: {
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