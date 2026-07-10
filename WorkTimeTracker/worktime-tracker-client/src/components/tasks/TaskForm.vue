<template>
  <form @submit.prevent="submit" class="space-y-4">
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Название задачи *
      </label>
      <input
        type="text"
        v-model="form.name"
        required
        maxlength="200"
        class="input-field"
        placeholder="Введите название задачи"
      />
    </div>
    
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Проект *
      </label>
      <select
        v-model="form.projectId"
        required
        class="input-field"
      >
        <option value="">Выберите проект</option>
        <option 
          v-for="project in projects" 
          :key="project.id" 
          :value="project.id"
          :disabled="!project.isActive"
        >
          {{ project.name }} ({{ project.code }})
          {{ !project.isActive ? '❌' : '' }}
        </option>
      </select>
    </div>
    
    <div class="flex items-center gap-2">
      <input
        type="checkbox"
        v-model="form.isActive"
        id="taskIsActive"
        class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
      />
      <label for="taskIsActive" class="text-sm text-gray-700">
        Активна
      </label>
    </div>
    
    <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-2 rounded-lg text-sm">
      {{ error }}
    </div>
    
    <div class="flex justify-end gap-3 pt-4 border-t">
      <button type="button" @click="$emit('close')" class="btn-secondary">
        Отмена
      </button>
      <button type="submit" class="btn-primary" :disabled="loading">
        {{ loading ? 'Сохранение...' : editMode ? 'Обновить' : 'Создать' }}
      </button>
    </div>
  </form>
</template>

<script>
import { ref, computed, watch } from 'vue';

export default {
  name: 'TaskForm',
  props: {
    initialData: {
      type: Object,
      default: null,
    },
    projects: {
      type: Array,
      required: true,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    error: {
      type: String,
      default: null,
    },
  },
  emits: ['submit', 'close'],
  setup(props, { emit }) {
    const form = ref({
      name: '',
      projectId: '',
      isActive: true,
    });
    
    const editMode = computed(() => !!props.initialData?.id);
    
    watch(
      () => props.initialData,
      (data) => {
        if (data) {
          form.value = {
            name: data.name || '',
            projectId: data.projectId || '',
            isActive: data.isActive !== undefined ? data.isActive : true,
          };
        }
      },
      { immediate: true }
    );
    
    const submit = () => {
      emit('submit', form.value);
    };
    
    return {
      form,
      editMode,
      submit,
    };
  },
};
</script>