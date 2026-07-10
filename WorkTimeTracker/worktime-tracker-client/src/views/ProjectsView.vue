<template>
  <div>
    <h1 class="text-2xl font-bold mb-6">📁 Проекты</h1>
    
    <ErrorMessage 
      v-if="error" 
      :error="error" 
      @close="error = null"
    />
    
    <ProjectList
      :projects="projects"
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
      <ProjectForm
        :initial-data="editingProject"
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
          Вы уверены, что хотите удалить проект 
          <span class="font-semibold">{{ deletingProject?.name }}</span>?
        </p>
        <p class="text-sm text-red-600">
          ⚠️ Проект можно удалить только если у него нет активных задач.
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
import ProjectList from '@/components/projects/ProjectList.vue';
import ProjectForm from '@/components/projects/ProjectForm.vue';
import Modal from '@/components/common/Modal.vue';
import ErrorMessage from '@/components/common/ErrorMessage.vue';
import { projectsApi } from '@/api/projects';

export default {
  name: 'ProjectsView',
  components: { ProjectList, ProjectForm, Modal, ErrorMessage },
  setup() {
    const projects = ref([]);
    const loading = ref(false);
    const error = ref(null);
    const modalOpen = ref(false);
    const editingProject = ref(null);
    const submitting = ref(false);
    const formError = ref(null);
    const deleteModalOpen = ref(false);
    const deletingProject = ref(null);
    const deleting = ref(false);

    const modalTitle = computed(() => 
      editingProject.value ? '✏️ Редактировать проект' : '➕ Создать проект'
    );

    const loadProjects = async () => {
      loading.value = true;
      error.value = null;
      try {
        const response = await projectsApi.getAll();
        projects.value = response.data;
      } catch (err) {
        error.value = err;
        console.error('Error loading projects:', err);
      } finally {
        loading.value = false;
      }
    };

    const openCreateModal = () => {
      editingProject.value = null;
      formError.value = null;
      modalOpen.value = true;
    };

    const openEditModal = (id) => {
      const project = projects.value.find(p => p.id === id);
      if (project) {
        editingProject.value = { ...project };
        formError.value = null;
        modalOpen.value = true;
      }
    };

    const closeModal = () => {
      modalOpen.value = false;
      editingProject.value = null;
      formError.value = null;
    };

    const handleSubmit = async (data) => {
      submitting.value = true;
      formError.value = null;
      
      try {
        if (editingProject.value) {
          await projectsApi.update(editingProject.value.id, data);
        } else {
          await projectsApi.create(data);
        }
        await loadProjects();
        closeModal();
      } catch (err) {
        formError.value = err.detail || 'Ошибка сохранения проекта';
        console.error('Error saving project:', err);
      } finally {
        submitting.value = false;
      }
    };

    const confirmDelete = (id) => {
      const project = projects.value.find(p => p.id === id);
      if (project) {
        deletingProject.value = project;
        deleteModalOpen.value = true;
      }
    };

    const handleDelete = async () => {
      if (!deletingProject.value) return;
      
      deleting.value = true;
      try {
        await projectsApi.delete(deletingProject.value.id);
        await loadProjects();
        deleteModalOpen.value = false;
      } catch (err) {
        error.value = err;
        console.error('Error deleting project:', err);
        deleteModalOpen.value = false;
      } finally {
        deleting.value = false;
        deletingProject.value = null;
      }
    };

    onMounted(loadProjects);

    return {
      projects,
      loading,
      error,
      modalOpen,
      editingProject,
      modalTitle,
      submitting,
      formError,
      deleteModalOpen,
      deletingProject,
      deleting,
      openCreateModal,
      openEditModal,
      closeModal,
      handleSubmit,
      confirmDelete,
      handleDelete,
    };
  },
};
</script>