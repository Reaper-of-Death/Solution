<template>
  <div>
    <h1 class="text-2xl font-bold mb-6">📝 Проводки времени</h1>
    
    <ErrorMessage 
      v-if="error" 
      :error="error" 
      @close="error = null"
    />
    
    <!-- Фильтры -->
    <div class="card mb-4">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            По дате
          </label>
          <input
            type="date"
            v-model="filters.date"
            class="input-field"
            @change="applyFilters"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            По месяцу
          </label>
          <input
            type="month"
            v-model="filters.month"
            class="input-field"
            @change="applyFilters"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            По неделе
          </label>
          <input
            type="week"
            v-model="filters.week"
            class="input-field"
            @change="applyFilters"
          />
        </div>
      </div>
      <div class="mt-4 flex gap-2">
        <button @click="clearFilters" class="btn-secondary text-sm">
          ✖️ Очистить фильтры
        </button>
        <button @click="loadEntries" class="btn-primary text-sm">
          🔄 Применить
        </button>
      </div>
    </div>
    
    <TimeEntryList
      :entries="entries"
      :loading="loading"
      @add="openCreateModal"
      @edit="openEditModal"
      @delete="confirmDelete"
    />
    
    <!-- Модальное окно для создания/редактирования -->
    <Modal 
      :is-open="modalOpen" 
      :title="modalTitle"
      @close="closeModal"
    >
      <TimeEntryForm
        :initial-data="editingEntry"
        :active-tasks="activeTasks"
        :loading="submitting"
        :error="formError"
        @submit="handleSubmit"
        @close="closeModal"
      />
    </Modal>
    
    <!-- Модальное окно подтверждения удаления -->
    <Modal
      :is-open="deleteModalOpen"
      title="Подтверждение удаления"
      @close="deleteModalOpen = false"
    >
      <div class="space-y-4">
        <p class="text-gray-700">
          Вы уверены, что хотите удалить эту проводку?
        </p>
        <div class="flex justify-end gap-3 pt-4 border-t">
          <button 
            @click="deleteModalOpen = false" 
            class="btn-secondary"
          >
            Отмена
          </button>
          <button 
            @click="handleDelete" 
            class="btn-danger"
            :disabled="deleting"
          >
            {{ deleting ? 'Удаление...' : 'Удалить' }}
          </button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import TimeEntryList from '@/components/timeentries/TimeEntryList.vue';
import TimeEntryForm from '@/components/timeentries/TimeEntryForm.vue';
import Modal from '@/components/common/Modal.vue';
import ErrorMessage from '@/components/common/ErrorMessage.vue';
import { timeEntriesApi } from '@/api/timeentries';
import { tasksApi } from '@/api/tasks';

export default {
  name: 'TimeEntriesView',
  components: { TimeEntryList, TimeEntryForm, Modal, ErrorMessage },
  setup() {
    const entries = ref([]);
    const activeTasks = ref([]);
    const loading = ref(false);
    const error = ref(null);
    const filters = ref({
      date: '',
      month: '',
      week: '',
    });
    const modalOpen = ref(false);
    const editingEntry = ref(null);
    const submitting = ref(false);
    const formError = ref(null);
    const deleteModalOpen = ref(false);
    const deletingEntry = ref(null);
    const deleting = ref(false);

    const modalTitle = computed(() => 
      editingEntry.value ? '✏️ Редактировать проводку' : '➕ Создать проводку'
    );

    const loadEntries = async () => {
      loading.value = true;
      error.value = null;
      try {
        const params = {};
        if (filters.value.date) params.date = filters.value.date;
        if (filters.value.month) params.month = filters.value.month;
        if (filters.value.week) params.week = filters.value.week;
        
        const response = await timeEntriesApi.getAll(params);
        entries.value = response.data;
      } catch (err) {
        error.value = err;
        console.error('Error loading entries:', err);
      } finally {
        loading.value = false;
      }
    };

    const loadActiveTasks = async () => {
      try {
        const response = await tasksApi.getActive();
        activeTasks.value = response.data;
      } catch (err) {
        console.error('Error loading active tasks:', err);
      }
    };

    const applyFilters = () => {
      loadEntries();
    };

    const clearFilters = () => {
      filters.value = { date: '', month: '', week: '' };
      loadEntries();
    };

    const openCreateModal = () => {
      editingEntry.value = null;
      formError.value = null;
      modalOpen.value = true;
    };

    const openEditModal = (id) => {
      const entry = entries.value.find(e => e.id === id);
      if (entry) {
        editingEntry.value = { ...entry };
        formError.value = null;
        modalOpen.value = true;
      }
    };

    const closeModal = () => {
      modalOpen.value = false;
      editingEntry.value = null;
      formError.value = null;
    };

    const handleSubmit = async (data) => {
      submitting.value = true;
      formError.value = null;
      
      try {
        if (editingEntry.value) {
          await timeEntriesApi.update(editingEntry.value.id, data);
        } else {
          await timeEntriesApi.create(data);
        }
        await loadEntries();
        closeModal();
      } catch (err) {
        formError.value = err.detail || 'Ошибка сохранения проводки';
        console.error('Error saving entry:', err);
      } finally {
        submitting.value = false;
      }
    };

    const confirmDelete = (id) => {
      const entry = entries.value.find(e => e.id === id);
      if (entry) {
        deletingEntry.value = entry;
        deleteModalOpen.value = true;
      }
    };

    const handleDelete = async () => {
      if (!deletingEntry.value) return;
      
      deleting.value = true;
      try {
        await timeEntriesApi.delete(deletingEntry.value.id);
        await loadEntries();
        deleteModalOpen.value = false;
      } catch (err) {
        error.value = err;
        console.error('Error deleting entry:', err);
        deleteModalOpen.value = false;
      } finally {
        deleting.value = false;
        deletingEntry.value = null;
      }
    };

    onMounted(() => {
      loadEntries();
      loadActiveTasks();
    });

    return {
      entries,
      activeTasks,
      loading,
      error,
      filters,
      modalOpen,
      editingEntry,
      modalTitle,
      submitting,
      formError,
      deleteModalOpen,
      deleting,
      openCreateModal,
      openEditModal,
      closeModal,
      handleSubmit,
      confirmDelete,
      handleDelete,
      applyFilters,
      clearFilters,
      loadEntries,
    };
  },
};
</script>