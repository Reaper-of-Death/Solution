<template>
  <form @submit.prevent="submit" class="space-y-4">
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Дата *
      </label>
      <input
        type="date"
        v-model="form.date"
        required
        class="input-field"
      />
    </div>
    
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Часы (0.01 - 24) *
      </label>
      <input
        type="number"
        v-model.number="form.hours"
        min="0.01"
        max="24"
        step="0.5"
        required
        class="input-field"
      />
      <p class="text-xs text-gray-500 mt-1">
        Введите количество часов (с шагом 0.5)
      </p>
    </div>
    
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Задача *
      </label>
      <select
        v-model="form.taskId"
        required
        class="input-field"
      >
        <option value="">Выберите задачу</option>
        <option 
          v-for="task in activeTasks" 
          :key="task.id" 
          :value="task.id"
        >
          {{ task.name }} ({{ task.projectCode }})
        </option>
      </select>
      <p v-if="activeTasks.length === 0" class="text-xs text-yellow-600 mt-1">
        ⚠️ Нет активных задач. Создайте задачу в разделе "Задачи".
      </p>
    </div>
    
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Описание
      </label>
      <textarea
        v-model="form.description"
        rows="3"
        maxlength="500"
        class="input-field"
        placeholder="Опишите выполненную работу"
      />
      <p class="text-xs text-gray-500 mt-1">{{ form.description?.length || 0 }}/500</p>
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
import { dateUtils } from '@/utils/dateUtils';

export default {
  name: 'TimeEntryForm',
  props: {
    initialData: {
      type: Object,
      default: null,
    },
    activeTasks: {
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
      date: dateUtils.today(),
      hours: 1,
      description: '',
      taskId: '',
    });
    
    const editMode = computed(() => !!props.initialData?.id);
    
    watch(
      () => props.initialData,
      (data) => {
        if (data) {
          form.value = {
            date: dateUtils.formatDate(data.date) || dateUtils.today(),
            hours: data.hours || 1,
            description: data.description || '',
            taskId: data.taskId || '',
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