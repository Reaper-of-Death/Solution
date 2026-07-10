<template>
  <form @submit.prevent="submit" class="space-y-4">
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Название проекта *
      </label>
      <input
        type="text"
        v-model="form.name"
        required
        maxlength="200"
        class="input-field"
        placeholder="Введите название проекта"
      />
      <p class="text-xs text-gray-500 mt-1">{{ form.name?.length || 0 }}/200</p>
    </div>
    
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Код проекта *
      </label>
      <input
        type="text"
        v-model="form.code"
        required
        maxlength="50"
        class="input-field"
        placeholder="Введите уникальный код"
      />
      <p class="text-xs text-gray-500 mt-1">{{ form.code?.length || 0 }}/50</p>
    </div>
    
    <div class="flex items-center gap-2">
      <input
        type="checkbox"
        v-model="form.isActive"
        id="isActive"
        class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
      />
      <label for="isActive" class="text-sm text-gray-700">
        Активен
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
  name: 'ProjectForm',
  props: {
    initialData: {
      type: Object,
      default: null,
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
      code: '',
      isActive: true,
    });
    
    const editMode = computed(() => !!props.initialData?.id);
    
    watch(
      () => props.initialData,
      (data) => {
        if (data) {
          form.value = {
            name: data.name || '',
            code: data.code || '',
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