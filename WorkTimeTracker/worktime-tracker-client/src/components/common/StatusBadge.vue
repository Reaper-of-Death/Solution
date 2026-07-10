<template>
  <span 
    class="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold uppercase"
    :class="active ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-500'"
  >
    <span class="w-2 h-2 rounded-full mr-2" :class="active ? 'bg-green-500' : 'bg-gray-400'"></span>
    {{ active ? 'Активен' : 'Неактивен' }}
  </span>
</template>

<script>
export default {
  name: 'StatusBadge',
  props: {
    active: {
      type: Boolean,
      required: true,
    },
  },
};
</script>