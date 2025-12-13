<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 p-2 sm:p-4"
    @click.self="closeModal"
  >
    <div class="modal-container bg-white rounded-2xl shadow-2xl w-full max-w-md mx-2 sm:mx-4 transform transition-all max-h-[95vh] overflow-y-auto">
      <div class="p-4 sm:p-6">
        <!-- Header -->
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-2xl font-bold events-text-dark">{{ isEditing ? 'Edit Event' : 'New Event' }}</h2>
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
              class="modal-input w-full px-3 sm:px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all text-base"
            />
          </div>

          <!-- Start Date & Time -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
            <div>
              <label class="block text-sm font-medium events-text-dark mb-2">
                Start Date <span class="text-red-500">*</span>
              </label>
              <input
                v-model="startDate"
                type="date"
                required
                class="modal-input w-full px-3 sm:px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all text-base"
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
                class="modal-input w-full px-3 sm:px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all text-base"
              />
            </div>
          </div>

          <!-- End Date & Time -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
            <div>
              <label class="block text-sm font-medium events-text-dark mb-2">
                End Date <span class="text-red-500">*</span>
              </label>
              <input
                v-model="endDate"
                type="date"
                required
                class="modal-input w-full px-3 sm:px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all text-base"
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
                class="modal-input w-full px-3 sm:px-4 py-2 border-2 border-gray-200 rounded-xl focus:border-blue-500 focus:outline-none transition-all text-base"
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
              v-if="isEditing"
              type="button"
              @click="handleDelete"
              :disabled="isSaving || isDeleting"
              class="px-4 py-2 bg-red-500 text-white rounded-xl font-medium transition-all disabled:opacity-50 disabled:cursor-not-allowed hover:bg-red-600"
            >
              <i v-if="isDeleting" class="pi pi-spin pi-spinner mr-2"></i>
              <i v-else class="pi pi-trash mr-2"></i>
              {{ isDeleting ? "Deleting..." : "Delete" }}
            </button>
            <button
              type="button"
              @click="closeModal"
              class="flex-1 px-4 py-2 border-2 border-gray-200 rounded-xl font-medium events-text-dark hover:bg-gray-50 transition-all"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="isSaving || isDeleting"
              class="flex-1 px-4 py-2 events-btn-primary text-white rounded-xl font-medium transition-all disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i v-if="isSaving" class="pi pi-spin pi-spinner mr-2"></i>
              <i v-else class="pi pi-check mr-2"></i>
              {{ isSaving ? "Saving..." : (isEditing ? "Update Event" : "Save Event") }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch, computed } from "vue";
import type { IEventModel } from "../interfaces";

interface EventData {
  id?: number;
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
  editingEvent?: IEventModel | null;
}>();

const emit = defineEmits<{
  close: [];
  save: [event: EventData];
  delete: [eventId: number];
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
const isDeleting = ref(false);
const errorMessage = ref("");

const isEditing = computed(() => !!props.editingEvent);

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
  if (props.editingEvent) {
    // Modul de editare - preîncarcă datele evenimentului
    const event = props.editingEvent;
    const start = new Date(event.startDate);
    const end = new Date(event.endDate);
    
    startDate.value = formatDateForInput(start);
    startTime.value = formatTimeForInput(start);
    endDate.value = formatDateForInput(end);
    endTime.value = formatTimeForInput(end);
    
    eventData.value.id = event.id;
    eventData.value.title = event.title;
    eventData.value.isShared = event.isShared;
    eventData.value.color = "blue"; // Default color, poate fi extins mai târziu
  } else {
    // Modul de creare - folosește datele inițiale sau valorile implicite
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
    
    eventData.value.id = undefined;
    eventData.value.title = "";
    eventData.value.isShared = true;
    eventData.value.color = "blue";
  }
  
  errorMessage.value = "";
};

watch(() => props.isOpen, (newValue) => {
  if (newValue) {
    initializeForm();
  }
});

watch(() => props.initialDate, () => {
  if (props.isOpen && !props.editingEvent) {
    initializeForm();
  }
});

watch(() => props.editingEvent, () => {
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

const handleDelete = () => {
  if (!props.editingEvent?.id) {
    return;
  }
  
  if (!confirm("Are you sure you want to delete this event?")) {
    return;
  }
  
  isDeleting.value = true;
  emit("delete", props.editingEvent.id);
  
  // Reset after a short delay to show loading state
  setTimeout(() => {
    isDeleting.value = false;
  }, 500);
};
</script>

<style scoped>
/* Force light color scheme for modal */
.modal-container {
  color-scheme: light;
  background-color: #FFFFFF !important;
  color: var(--color-text-dark) !important;
}

.modal-input {
  color-scheme: light;
  background-color: #FFFFFF !important;
  color: var(--color-text-dark) !important;
}

.modal-input::placeholder {
  color: var(--color-text-muted) !important;
}

/* Ensure date/time inputs work on mobile */
.modal-input::-webkit-calendar-picker-indicator {
  filter: none;
  opacity: 1;
}

.events-text-dark {
  color: var(--color-text-dark) !important;
}

.events-text-muted {
  color: var(--color-text-muted) !important;
}

.events-btn-primary {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
  color: #FFFFFF !important;
}

.events-btn-primary:hover:not(:disabled) {
  background: linear-gradient(to right, var(--color-accent-blue), var(--color-primary));
}

/* Mobile responsive adjustments */
@media (max-width: 640px) {
  .modal-container {
    margin-top: auto;
    margin-bottom: auto;
  }
}
</style>

