<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50"
    @click.self="closeModal"
  >
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md mx-4 transform transition-all">
      <div class="p-6">
        <!-- Header -->
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-2xl font-bold events-text-dark">New Event</h2>
          <button
            @click="closeModal"
            class="p-2 rounded-lg hover:bg-gray-100 transition-colors"
          >
            <i class="pi pi-times text-gray-500"></i>
          </button>
        </div>

        <!-- Error message -->
        <div
          v-if="errorMessage"
          class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm flex items-center gap-2"
        >
          <i class="pi pi-exclamation-triangle"></i>
          <span>{{ errorMessage }}</span>
        </div>

        <!-- Form -->
        <form @submit.prevent="saveEvent" class="space-y-4">
          <!-- Title -->
          <div>
            <label class="block text-sm font-medium events-text-dark mb-2">
              Title <span class="text-red-500">*</span>
            </label>
            <input
              v-model="eventData.title"
              type="text"
              placeholder="Enter event title"
              required
              class="w-full px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all"
            />
          </div>

          <!-- Start Date & Time -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium events-text-dark mb-2">
                Start Date <span class="text-red-500">*</span>
              </label>
              <input
                v-model="startDate"
                type="date"
                required
                class="w-full px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all"
              />
            </div>
            <div>
              <label class="block text-sm font-medium events-text-dark mb-2">
                Start Time <span class="text-red-500">*</span>
              </label>
              <input
                v-model="startTime"
                type="time"
                required
                class="w-full px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all"
              />
            </div>
          </div>

          <!-- End Date & Time -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium events-text-dark mb-2">
                End Date <span class="text-red-500">*</span>
              </label>
              <input
                v-model="endDate"
                type="date"
                required
                class="w-full px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all"
              />
            </div>
            <div>
              <label class="block text-sm font-medium events-text-dark mb-2">
                End Time <span class="text-red-500">*</span>
              </label>
              <input
                v-model="endTime"
                type="time"
                required
                class="w-full px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all"
              />
            </div>
          </div>

          <!-- Color Selection -->
          <div>
            <label class="block text-sm font-medium events-text-dark mb-2">
              Color
            </label>
            <div class="flex gap-2">
              <button
                v-for="color in colors"
                :key="color.value"
                type="button"
                @click="eventData.color = color.value"
                :class="[
                  'w-10 h-10 rounded-lg border-2 transition-all',
                  color.class,
                  eventData.color === color.value
                    ? 'border-gray-800 scale-110'
                    : 'border-gray-200 hover:border-gray-400'
                ]"
                :title="color.label"
              ></button>
            </div>
          </div>

          <!-- Is Shared -->
          <div class="flex items-center gap-3">
            <input
              v-model="eventData.isShared"
              type="checkbox"
              id="isShared"
              class="w-5 h-5 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <label for="isShared" class="text-sm font-medium events-text-dark">
              Share with group
            </label>
          </div>

          <!-- Actions -->
          <div class="flex gap-3 pt-4">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 px-4 py-2 border-2 border-gray-200 rounded-xl font-medium events-text-dark hover:bg-gray-50 transition-all"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="isSaving"
              class="flex-1 px-4 py-2 events-btn-primary text-white rounded-xl font-medium transition-all disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i v-if="isSaving" class="pi pi-spin pi-spinner mr-2"></i>
              <i v-else class="pi pi-check mr-2"></i>
              {{ isSaving ? "Saving..." : "Save Event" }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch } from "vue";

interface EventData {
  title: string;
  startDate: Date;
  endDate: Date;
  isShared: boolean;
  color: string;
}

const props = defineProps<{
  isOpen: boolean;
  initialDate?: Date;
  initialHour?: number;
}>();

const emit = defineEmits<{
  close: [];
  save: [event: EventData];
}>();

const eventData = ref<EventData>({
  title: "",
  startDate: new Date(),
  endDate: new Date(),
  isShared: true,
  color: "blue",
});

const startDate = ref("");
const startTime = ref("");
const endDate = ref("");
const endTime = ref("");
const isSaving = ref(false);
const errorMessage = ref("");

const colors = [
  { value: "blue", label: "Blue", class: "bg-blue-500" },
  { value: "green", label: "Green", class: "bg-green-500" },
  { value: "orange", label: "Orange", class: "bg-orange-500" },
  { value: "purple", label: "Purple", class: "bg-purple-500" },
  { value: "red", label: "Red", class: "bg-red-500" },
];

const formatDateForInput = (date: Date): string => {
  return date.toISOString().split("T")[0] || "";
};

const formatTimeForInput = (date: Date): string => {
  const hours = date.getHours().toString().padStart(2, "0");
  const minutes = date.getMinutes().toString().padStart(2, "0");
  return `${hours}:${minutes}` || "";
};

const initializeForm = () => {
  const date = props.initialDate || new Date();
  const hour = props.initialHour !== undefined ? props.initialHour : date.getHours();
  
  // Set start date/time
  const start = new Date(date);
  start.setHours(hour, 0, 0, 0);
  startDate.value = formatDateForInput(start);
  startTime.value = formatTimeForInput(start);
  
  // Set end date/time (1 hour later by default)
  const end = new Date(start);
  end.setHours(hour + 1, 0, 0, 0);
  endDate.value = formatDateForInput(end);
  endTime.value = formatTimeForInput(end);
  
  eventData.value.title = "";
  eventData.value.isShared = true;
  eventData.value.color = "blue";
  errorMessage.value = "";
};

watch(() => props.isOpen, (newValue) => {
  if (newValue) {
    initializeForm();
  }
});

watch(() => props.initialDate, () => {
  if (props.isOpen) {
    initializeForm();
  }
});

const closeModal = () => {
  emit("close");
};

const saveEvent = () => {
  errorMessage.value = "";
  
  // Validate dates
  const start = new Date(`${startDate.value}T${startTime.value}`);
  const end = new Date(`${endDate.value}T${endTime.value}`);
  
  if (end <= start) {
    errorMessage.value = "End date/time must be after start date/time";
    return;
  }
  
  isSaving.value = true;
  
  eventData.value.startDate = start;
  eventData.value.endDate = end;
  
  // Emit the event data
  emit("save", { ...eventData.value });
  
  // Reset after a short delay to show loading state
  setTimeout(() => {
    isSaving.value = false;
  }, 500);
};
</script>

<style scoped>
.events-text-dark {
  color: var(--color-text-dark);
}

.events-text-muted {
  color: var(--color-text-muted);
}

.events-btn-primary {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
}

.events-btn-primary:hover:not(:disabled) {
  background: linear-gradient(to right, var(--color-accent-blue), var(--color-primary));
}
</style>

