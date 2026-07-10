<template>
  <div id="app" class="min-h-screen bg-gray-100">
    <AppHeader />
    
    <main class="container mx-auto px-4 py-8">
      <router-view />
    </main>
    
    <AppFooter />
  </div>
</template>

<script>
import AppHeader from './components/layout/AppHeader.vue';
import AppFooter from './components/layout/AppFooter.vue';

export default {
  name: 'App',
  components: {
    AppHeader,
    AppFooter,
  },
};
</script>