<template>
  <div class="p-2 sm:p-4 min-h-screen tasks-page-background tasks-force-light">
    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="mb-4 sm:mb-6">
        <h1 class="text-2xl sm:text-3xl font-bold tasks-text-dark mb-1 sm:mb-2">Task-uri</h1>
        <p class="tasks-text-muted text-sm sm:text-base">Gestionează și urmărește task-urile tale</p>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-3 sm:gap-4 mb-4 sm:mb-6">
        <div class="tasks-card rounded-xl p-3 sm:p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm tasks-text-muted mb-1">Total Task-uri</p>
              <p class="text-xl sm:text-2xl font-bold tasks-text-dark">{{ tasksStore.tasks.length }}</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-lg bg-blue-100 flex items-center justify-center">
              <i class="pi pi-list text-blue-600 text-lg sm:text-xl"></i>
            </div>
          </div>
        </div>

        <div class="tasks-card rounded-xl p-3 sm:p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm tasks-text-muted mb-1">În Așteptare</p>
              <p class="text-xl sm:text-2xl font-bold tasks-text-dark">{{ tasksStore.pendingTasks.length }}</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-lg bg-yellow-100 flex items-center justify-center">
              <i class="pi pi-clock text-yellow-600 text-lg sm:text-xl"></i>
            </div>
          </div>
        </div>

        <div class="tasks-card rounded-xl p-3 sm:p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm tasks-text-muted mb-1">În Progres</p>
              <p class="text-xl sm:text-2xl font-bold tasks-text-dark">{{ tasksStore.inProgressTasks.length }}</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-lg bg-orange-100 flex items-center justify-center">
              <i class="pi pi-spin pi-spinner text-orange-600 text-lg sm:text-xl"></i>
            </div>
          </div>
        </div>

        <div class="tasks-card rounded-xl p-3 sm:p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm tasks-text-muted mb-1">Finalizate</p>
              <p class="text-xl sm:text-2xl font-bold tasks-text-dark">{{ tasksStore.completedTasks.length }}</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-lg bg-green-100 flex items-center justify-center">
              <i class="pi pi-check-circle text-green-600 text-lg sm:text-xl"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Tasks List -->
        <div class="lg:col-span-2">
          <div class="tasks-card rounded-2xl shadow-lg p-4 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h2 class="text-lg sm:text-xl font-bold tasks-text-dark">Lista Task-uri</h2>
              <div class="flex items-center gap-2">
                <select
                  v-model="filterStatus"
                  class="px-3 py-1.5 sm:px-4 sm:py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
                >
                  <option value="all">Toate</option>
                  <option value="pending">În Așteptare</option>
                  <option value="in_progress">În Progres</option>
                  <option value="completed">Finalizate</option>
                </select>
                <select
                  v-model="filterPriority"
                  class="px-3 py-1.5 sm:px-4 sm:py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
                >
                  <option value="all">Toate Prioritățile</option>
                  <option value="high">Prioritate Mare</option>
                  <option value="medium">Prioritate Medie</option>
                  <option value="low">Prioritate Mică</option>
                </select>
              </div>
            </div>

            <div v-if="filteredTasks.length === 0" class="text-center py-12">
              <i class="pi pi-inbox text-5xl tasks-text-muted opacity-50 mb-4"></i>
              <p class="tasks-text-muted">Nu există task-uri</p>
            </div>

            <div v-else class="space-y-3">
              <div
                v-for="task in filteredTasks"
                :key="task.id"
                class="flex items-start gap-3 sm:gap-4 p-3 sm:p-4 rounded-lg border border-gray-100 hover:bg-gray-50 transition-all duration-200 task-item"
                :class="{
                  'opacity-60': task.isCompleted,
                  'border-l-4': true,
                  'border-l-red-500': task.priority === 'high' && !task.isCompleted,
                  'border-l-yellow-500': task.priority === 'medium' && !task.isCompleted,
                  'border-l-green-500': task.priority === 'low' && !task.isCompleted,
                  'border-l-gray-400': task.isCompleted,
                }"
              >
                <div class="flex items-start gap-2 sm:gap-3 flex-1">
                  <input
                    type="checkbox"
                    :checked="task.isCompleted"
                    @change="handleToggleComplete(task.id)"
                    class="mt-1 w-5 h-5 rounded border-gray-300 text-blue-600 focus:ring-blue-500 cursor-pointer"
                  />
                  <div class="flex-1 min-w-0">
                    <h3
                      class="font-semibold tasks-text-dark text-sm sm:text-base mb-1"
                      :class="{ 'line-through': task.isCompleted }"
                    >
                      {{ task.title }}
                    </h3>
                    <p v-if="task.description" class="text-xs sm:text-sm tasks-text-muted mb-2">
                      {{ task.description }}
                    </p>
                    <div class="flex flex-wrap items-center gap-2 text-xs sm:text-sm">
                      <span
                        class="px-2 py-1 rounded-full font-medium"
                        :class="getPriorityClass(task.priority)"
                      >
                        {{ getPriorityLabel(task.priority) }}
                      </span>
                      <span
                        class="px-2 py-1 rounded-full font-medium"
                        :class="getStatusClass(task.status)"
                      >
                        {{ getStatusLabel(task.status) }}
                      </span>
                      <span v-if="task.dueDate" class="tasks-text-muted flex items-center gap-1">
                        <i class="pi pi-calendar"></i>
                        {{ formatDate(task.dueDate) }}
                      </span>
                    </div>
                  </div>
                </div>
                <div class="flex items-center gap-2 flex-shrink-0">
                  <button
                    v-if="!task.isCompleted"
                    @click="handleStartTask(task.id)"
                    class="p-2 rounded-lg hover:bg-blue-100 transition-colors"
                    title="Începe task-ul"
                  >
                    <i class="pi pi-play text-blue-600"></i>
                  </button>
                  <button
                    @click="handleDeleteTask(task.id)"
                    class="p-2 rounded-lg hover:bg-red-100 transition-colors"
                    title="Șterge task-ul"
                  >
                    <i class="pi pi-trash text-red-600"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Add Task Form -->
        <div class="lg:col-span-1">
          <div class="tasks-card rounded-2xl shadow-lg p-4 sm:p-6">
            <h2 class="text-lg sm:text-xl font-bold tasks-text-dark mb-4">Adaugă Task</h2>
            <form @submit.prevent="handleAddTask" class="space-y-3 sm:space-y-4">
              <div>
                <label class="block text-xs sm:text-sm font-medium tasks-text-dark mb-1 sm:mb-2">
                  Titlu *
                </label>
                <input
                  v-model="newTask.title"
                  type="text"
                  required
                  class="w-full px-3 py-2 sm:px-4 sm:py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                  placeholder="ex: Finalizează raportul"
                />
              </div>

              <div>
                <label class="block text-xs sm:text-sm font-medium tasks-text-dark mb-1 sm:mb-2">
                  Descriere
                </label>
                <textarea
                  v-model="newTask.description"
                  rows="3"
                  class="w-full px-3 py-2 sm:px-4 sm:py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base resize-none"
                  placeholder="Detalii despre task..."
                ></textarea>
              </div>

              <div>
                <label class="block text-xs sm:text-sm font-medium tasks-text-dark mb-1 sm:mb-2">
                  Prioritate
                </label>
                <select
                  v-model="newTask.priority"
                  class="w-full px-3 py-2 sm:px-4 sm:py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                >
                  <option value="low">Prioritate Mică</option>
                  <option value="medium">Prioritate Medie</option>
                  <option value="high">Prioritate Mare</option>
                </select>
              </div>

              <div>
                <label class="block text-xs sm:text-sm font-medium tasks-text-dark mb-1 sm:mb-2">
                  Termen Limita (opțional)
                </label>
                <input
                  v-model="newTask.dueDate"
                  type="date"
                  class="w-full px-3 py-2 sm:px-4 sm:py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                />
              </div>

              <button
                type="submit"
                class="w-full py-2.5 sm:py-3 rounded-lg tasks-btn-active text-white font-medium transition-all duration-200 hover:shadow-lg text-sm sm:text-base"
              >
                <i class="pi pi-plus mr-2"></i>
                Adaugă Task
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from "vue";
import { useTasksStore } from "../stores/tasksStore";
import { useUserStore } from "../stores/userStore";
import type { ITaskModel } from "../interfaces";

const tasksStore = useTasksStore();
const userStore = useUserStore();

const newTask = ref<Partial<ITaskModel>>({
  title: "",
  description: "",
  priority: "medium",
  dueDate: undefined,
  status: "pending",
  isCompleted: false,
});

const filterStatus = ref<string>("all");
const filterPriority = ref<string>("all");

const filteredTasks = computed(() => {
  let tasks = tasksStore.tasks;

  if (filterStatus.value !== "all") {
    tasks = tasks.filter((t) => t.status === filterStatus.value);
  }

  if (filterPriority.value !== "all") {
    tasks = tasks.filter((t) => t.priority === filterPriority.value);
  }

  return tasks;
});

const initializeDummyData = () => {
  // Adăugăm date dummy doar dacă nu există deja task-uri
  if (tasksStore.tasks.length === 0) {
    const titles = [
      "Finalizează raportul trimestrial",
      "Revizuiește documentația API",
      "Organizează întâlnirea cu echipa",
      "Actualizează baza de date",
      "Testează funcționalitățile noi",
      "Scrie documentația pentru utilizatori",
      "Optimizează performanța aplicației",
      "Rezolvă bug-urile raportate",
      "Planifică sprint-ul următor",
      "Actualizează dependențele",
    ];

    const descriptions = [
      "Raportul trebuie să includă toate metricile importante",
      "Verifică că toate endpoint-urile sunt documentate corect",
      "Pregătește agenda și materialele necesare",
      "Asigură-te că backup-ul este actualizat",
      "Rulează toate testele și verifică rezultatele",
      "Include exemple și screenshot-uri",
      "Identifică și rezolvă bottleneck-urile",
      "Prioritizează bug-urile critice",
      "Definește obiectivele și task-urile pentru următorul sprint",
      "Verifică compatibilitatea și actualizează dacă e necesar",
    ];

    const priorities: ("low" | "medium" | "high")[] = ["low", "medium", "high"];
    const statuses: ("pending" | "in_progress" | "completed")[] = ["pending", "in_progress", "completed"];

    const today = new Date();
    for (let i = 0; i < 10; i++) {
      const dueDate = new Date(today);
      dueDate.setDate(dueDate.getDate() + Math.floor(Math.random() * 14) - 3); // -3 până la +11 zile

      const status = statuses[Math.floor(Math.random() * statuses.length)];
      const isCompleted = status === "completed";

      const task: ITaskModel = {
        id: i + 1,
        title: titles[i] as string,
        description: descriptions[i],
        priority: priorities[Math.floor(Math.random() * priorities.length)] as "low" | "medium" | "high",
        status: status as "pending" | "in_progress" | "completed",
        isCompleted: isCompleted,
        dueDate: dueDate,
        createdByUserId: userStore.userData?.id || 1,
        createdAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000), // Ultimele 7 zile
      };

      tasksStore.addTaskLocal(task);
    }
  }
};

onMounted(() => {
  initializeDummyData();
});

const handleAddTask = () => {
  if (!newTask.value.title) {
    return;
  }

  const task: ITaskModel = {
    id: Date.now(),
    title: newTask.value.title,
    description: newTask.value.description,
    priority: newTask.value.priority || "medium",
    status: newTask.value.status || "pending",
    isCompleted: false,
    dueDate: newTask.value.dueDate ? new Date(newTask.value.dueDate) : undefined,
    createdByUserId: userStore.userData?.id || 1,
    createdAt: new Date(),
  };

  tasksStore.addTaskLocal(task);

  // Reset form
  newTask.value = {
    title: "",
    description: "",
    priority: "medium",
    dueDate: undefined,
    status: "pending",
    isCompleted: false,
  };
};

const handleToggleComplete = (taskId: number) => {
  tasksStore.toggleTaskComplete(taskId);
};

const handleStartTask = (taskId: number) => {
  const task = tasksStore.tasks.find((t) => t.id === taskId);
  if (task) {
    task.status = "in_progress";
    tasksStore.updateTaskLocal(task);
  }
};

const handleDeleteTask = (taskId: number) => {
  if (confirm("Ești sigur că vrei să ștergi acest task?")) {
    tasksStore.removeTaskLocal(taskId);
  }
};

const formatDate = (date: Date | string | undefined): string => {
  if (!date) return "";
  const d = typeof date === "string" ? new Date(date) : date;
  return d.toLocaleDateString("ro-RO", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
};

const getPriorityLabel = (priority: string): string => {
  const labels: Record<string, string> = {
    high: "Prioritate Mare",
    medium: "Prioritate Medie",
    low: "Prioritate Mică",
  };
  return labels[priority] || priority;
};

const getPriorityClass = (priority: string): string => {
  const classes: Record<string, string> = {
    high: "bg-red-100 text-red-700",
    medium: "bg-yellow-100 text-yellow-700",
    low: "bg-green-100 text-green-700",
  };
  return classes[priority] || "bg-gray-100 text-gray-700";
};

const getStatusLabel = (status: string): string => {
  const labels: Record<string, string> = {
    pending: "În Așteptare",
    in_progress: "În Progres",
    completed: "Finalizat",
  };
  return labels[status] || status;
};

const getStatusClass = (status: string): string => {
  const classes: Record<string, string> = {
    pending: "bg-yellow-100 text-yellow-700",
    in_progress: "bg-orange-100 text-orange-700",
    completed: "bg-green-100 text-green-700",
  };
  return classes[status] || "bg-gray-100 text-gray-700";
};
</script>

<style scoped>
/* Force light color scheme */
.tasks-force-light {
  color-scheme: light;
}

.tasks-page-background {
  background: linear-gradient(to bottom right, var(--color-background), var(--color-card), var(--color-background));
  background-color: var(--color-background) !important;
}

.tasks-text-dark {
  color: var(--color-text-dark) !important;
}

.tasks-text-muted {
  color: var(--color-text-muted) !important;
}

.tasks-btn-active {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
  color: #ffffff !important;
}

.tasks-card {
  background: var(--color-card) !important;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.task-item {
  transition: all 0.2s ease;
}

.task-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}
</style>

