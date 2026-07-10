<template>
  <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg relative">
    <div class="flex items-start gap-3">
      <span class="text-xl">❌</span>
      <div class="flex-1">
        <p class="font-semibold">{{ title || 'Ошибка' }}</p>
        <p class="text-sm">{{ message }}</p>
      </div>
      <button @click="$emit('close')" class="text-red-500 hover:text-red-700">
        ✕
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ErrorMessage',
  props: {
    error: {
      type: Object,
      required: true,
    },
    title: {
      type: String,
      default: '',
    },
  },
  computed: {
    message() {
      return this.error?.detail || this.error?.userMessage || 'Произошла неизвестная ошибка';
    },
  },
  emits: ['close'],
};
</script>