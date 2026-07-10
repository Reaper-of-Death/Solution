<template>
  <div>
    <h1 class="text-2xl font-bold mb-6">✅ Задачи</h1>
    
    <ErrorMessage 
      v-if="error" 
      :error="error" 
      @close="error = null"
    />
    
    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Фильтр по проекту
      </label>
      <select
        v-model="filterProjectId"
        class="input-field max-w-xs"
        @change="loadTasks"
      >
        <option value="">Все проекты</option>
        <option 
          v-for="project in projects" 
          :key="project.id" 
          :value="project.id"
        >
          {{ project.name }}
        </option>
      </select>
    </div>
    
    <TaskList
      :tasks="filteredTasks"
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
      <TaskForm
        :initial-data="editingTask"
        :projects="projects"
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
          Вы уверены, что хотите удалить задачу 
          <span class="font-semibold">{{ deletingTask?.name }}</span>?
        </p>
        <p class="text-sm text-red-600">
          ⚠️ Задачу можно удалить только если на нее нет проводок времени.
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
import TaskList from '@/components/tasks/TaskList.vue';
import TaskForm from '@/components/tasks/TaskForm.vue';
import Modal from '@/components/common/Modal.vue';
import ErrorMessage from '@/components/common/ErrorMessage.vue';
import { tasksApi } from '@/api/tasks';
import { projectsApi } from '@/api/projects';

export default {
  name: 'TasksView',
  components: { TaskList, TaskForm, Modal, ErrorMessage },
  setup() {
    const tasks = ref([]);
    const projects = ref([]);
    const loading = ref(false);
    const error = ref(null);
    const filterProjectId = ref('');
    const modalOpen = ref(false);
    const editingTask = ref(null);
    const submitting = ref(false);
    const formError = ref(null);
    const deleteModalOpen = ref(false);
    const deletingTask = ref(null);
    const deleting = ref(false);

    const modalTitle = computed(() => 
      editingTask.value ? '✏️ Редактировать задачу' : '➕ Создать задачу'
    );

    const filteredTasks = computed(() => {
      if (!filterProjectId.value) return tasks.value;
      return tasks.value.filter(t => t.projectId === filterProjectId.value);
    });

    const loadData = async () => {
      loading.value = true;
      error.value = null;
      try {
        const [tasksRes, projectsRes] = await Promise.all([
          tasksApi.getAll(),
          projectsApi.getAll(),
        ]);
        tasks.value = tasksRes.data;
        projects.value = projectsRes.data;
      } catch (err) {
        error.value = err;
        console.error('Error loading data:', err);
      } finally {
        loading.value = false;
      }
    };

    const loadTasks = async () => {
      loading.value = true;
      error.value = null;
      try {
        const response = await tasksApi.getAll();
        tasks.value = response.data;
      } catch (err) {
        error.value = err;
        console.error('Error loading tasks:', err);
      } finally {
        loading.value = false;
      }
    };

    const openCreateModal = () => {
      editingTask.value = null;
      formError.value = null;
      modalOpen.value = true;
    };

    const openEditModal = (id) => {
      const task = tasks.value.find(t => t.id === id);
      if (task) {
        editingTask.value = { ...task };
        formError.value = null;
        modalOpen.value = true;
      }
    };

    const closeModal = () => {
      modalOpen.value = false;
      editingTask.value = null;
      formError.value = null;
    };

    const handleSubmit = async (data) => {
      submitting.value = true;
      formError.value = null;
      
      try {
        if (editingTask.value) {
          await tasksApi.update(editingTask.value.id, data);
        } else {
          await tasksApi.create(data);
        }
        await loadTasks();
        closeModal();
      } catch (err) {
        formError.value = err.detail || 'Ошибка сохранения задачи';
        console.error('Error saving task:', err);
      } finally {
        submitting.value = false;
      }
    };

    const confirmDelete = (id) => {
      const task = tasks.value.find(t => t.id === id);
      if (task) {
        deletingTask.value = task;
        deleteModalOpen.value = true;
      }
    };

    const handleDelete = async () => {
      if (!deletingTask.value) return;
      
      deleting.value = true;
      try {
        await tasksApi.delete(deletingTask.value.id);
        await loadTasks();
        deleteModalOpen.value = false;
      } catch (err) {
        error.value = err;
        console.error('Error deleting task:', err);
        deleteModalOpen.value = false;
      } finally {
        deleting.value = false;
        deletingTask.value = null;
      }
    };

    onMounted(loadData);

    return {
      tasks,
      projects,
      loading,
      error,
      filterProjectId,
      filteredTasks,
      modalOpen,
      editingTask,
      modalTitle,
      submitting,
      formError,
      deleteModalOpen,
      deletingTask,
      deleting,
      openCreateModal,
      openEditModal,
      closeModal,
      handleSubmit,
      confirmDelete,
      handleDelete,
      loadTasks,
    };
  },
};
</script>