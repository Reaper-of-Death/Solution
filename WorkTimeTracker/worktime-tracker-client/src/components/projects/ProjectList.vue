<template>
  <div>
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-xl font-semibold">📁 Проекты</h2>
      <button @click="$emit('add')" class="btn-primary">
        ➕ Создать проект
      </button>
    </div>
    
    <LoadingSpinner v-if="loading" />
    
    <div v-else-if="projects.length === 0" class="text-center py-12 text-gray-500">
      <div class="text-6xl mb-4">📭</div>
      <p>Нет проектов</p>
      <p class="text-sm mt-1">Создайте первый проект</p>
    </div>
    
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <ProjectCard
        v-for="project in projects"
        :key="project.id"
        :project="project"
        @edit="$emit('edit', project.id)"
        @delete="$emit('delete', project.id)"
      />
    </div>
  </div>
</template>

<script>
import ProjectCard from './ProjectCard.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

export default {
  name: 'ProjectList',
  components: { ProjectCard, LoadingSpinner },
  props: {
    projects: {
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