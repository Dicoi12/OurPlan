<template>
  <div class="p-2 sm:p-4 min-h-screen events-page-background events-force-light">
    <!-- Event Modal -->
    <EventModal
      :isOpen="isModalOpen"
      :initialDate="selectedDate"
      :initialHour="selectedHour"
      :editingEvent="editingEvent"
      @close="closeModal"
      @save="handleSaveEvent"
      @delete="handleDeleteEvent"
    />

    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="mb-4 sm:mb-6">
        <h1 class="text-2xl sm:text-3xl font-bold events-text-dark mb-1 sm:mb-2">Calendar</h1>
        <p class="events-text-muted text-sm sm:text-base">View and manage events for your group</p>
      </div>

      <!-- View Selector and Navigation -->
      <div class="mb-4 sm:mb-6 flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-3 sm:gap-4">
        <div class="flex items-center gap-1 sm:gap-2 events-card-bg rounded-xl p-1 shadow-sm border border-gray-100 overflow-x-auto">
          <button
            v-for="view in views"
            :key="view.value"
            @click="changeView(view.value)"
            :class="[
              'px-3 sm:px-4 py-2 rounded-lg font-medium transition-all duration-200 whitespace-nowrap text-sm sm:text-base',
              currentView === view.value
                ? 'events-btn-active text-white'
                : 'events-text-muted hover:bg-gray-50'
            ]"
          >
            <i :class="view.icon" class="mr-1 sm:mr-2"></i>
            <span>{{ view.label }}</span>
          </button>
        </div>

        <div class="flex items-center justify-center gap-2 sm:gap-3">
          <button
            @click="previousPeriod"
            class="p-2 rounded-lg events-icon-bg hover:bg-blue-200 transition-colors"
          >
            <i class="pi pi-chevron-left events-primary"></i>
          </button>
          <span class="font-semibold events-text-dark min-w-[120px] sm:min-w-[200px] text-center text-sm sm:text-base">
            {{ currentPeriodLabel }}
          </span>
          <button
            @click="nextPeriod"
            class="p-2 rounded-lg events-icon-bg hover:bg-blue-200 transition-colors"
          >
            <i class="pi pi-chevron-right events-primary"></i>
          </button>
          <button
            @click="goToToday"
            class="px-3 sm:px-4 py-2 rounded-lg events-btn-secondary font-medium transition-all duration-200 text-sm sm:text-base"
          >
            Today
          </button>
        </div>
      </div>

      <!-- Error message -->
      <div
        v-if="errorMessage"
        class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm flex items-center gap-2"
      >
        <i class="pi pi-exclamation-triangle"></i>
        <span>{{ errorMessage }}</span>
      </div>

      <!-- Calendar Views -->
      <div class="events-card rounded-2xl shadow-lg overflow-hidden">
        <!-- Day View -->
        <DayView
          v-if="currentView === 'day'"
          :current-date="currentDate"
          :events="displayEvents"
          @hour-click="openModalForHour"
          @event-click="handleEventClick"
        />

        <!-- Week View -->
        <WeekView
          v-if="currentView === 'week'"
          :current-date="currentDate"
          :events="displayEvents"
          @day-hour-click="openModalForDayAndHour"
          @event-click="handleEventClick"
        />

        <!-- Month View -->
        <MonthView
          v-if="currentView === 'month'"
          :current-date="currentDate"
          :events="displayEvents"
          @day-click="openModalForDay"
          @event-click="handleEventClick"
        />

        <!-- Empty state -->
        <div
          v-if="!isLoading && (!displayEvents || displayEvents.length === 0)"
          class="text-center py-12"
        >
          <i class="pi pi-calendar text-5xl events-text-muted opacity-50 mb-4"></i>
          <p class="events-text-muted">No events found for this period</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, watch, onBeforeMount } from "vue";
import { useGroupsStore } from "../stores/groupsStore";
import { useEventStore } from "../stores/eventStore";
import { useUserStore } from "../stores/userStore";
import router from "../router";
import EventModal from "../components/EventModal.vue";
import DayView from "../components/views/DayView.vue";
import WeekView from "../components/views/WeekView.vue";
import MonthView from "../components/views/MonthView.vue";
import type { IEventModel } from "../interfaces";

const groupsStore = useGroupsStore();
const eventStore = useEventStore();
const userStore = useUserStore();
const currentView = ref<"day" | "week" | "month">("day");
const currentDate = ref(new Date());
const isLoading = ref(false);
const errorMessage = ref("");
const isModalOpen = ref(false);
const selectedDate = ref<Date>(new Date());
const selectedHour = ref<number | undefined>(undefined);
const editingEvent = ref<IEventModel | null>(null);

onBeforeMount(async() => {
    loadEvents();
    console.log(eventStore.events);
});

// Eliminăm onBeforeMount - evenimentele se vor încărca în onMounted pentru grup
const views = [
  { value: "day" as const, label: "Day", icon: "pi pi-calendar" },
  { value: "week" as const, label: "Week", icon: "pi pi-calendar-times" },
  { value: "month" as const, label: "Month", icon: "pi pi-calendar-plus" },
];

const displayEvents = computed(() => {
  return eventStore.events || [];
});

const currentPeriodLabel = computed(() => {
  if (currentView.value === "day") {
    return formatDate(currentDate.value, "full");
  } else if (currentView.value === "week") {
    const start = getWeekStart(currentDate.value);
    const end = new Date(start);
    end.setDate(end.getDate() + 6);
    return `${formatDate(start, "short")} - ${formatDate(end, "short")}`;
  } else {
    return formatDate(currentDate.value, "month");
  }
});


const getWeekStart = (date: Date): Date => {
  const d = new Date(date);
  const day = d.getDay();
  const diff = d.getDate() - day;
  return new Date(d.setDate(diff));
};

const formatDate = (date: Date, format: "full" | "short" | "month"): string => {
  if (format === "month") {
    return date.toLocaleDateString("en-US", { month: "long", year: "numeric" });
  } else if (format === "short") {
    return date.toLocaleDateString("en-US", { month: "short", day: "numeric" });
  } else {
    return date.toLocaleDateString("en-US", {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  }
};

const previousPeriod = () => {
  const newDate = new Date(currentDate.value);
  if (currentView.value === "day") {
    newDate.setDate(newDate.getDate() - 1);
  } else if (currentView.value === "week") {
    newDate.setDate(newDate.getDate() - 7);
  } else {
    newDate.setMonth(newDate.getMonth() - 1);
  }
  currentDate.value = newDate;
  // Reîncarcă evenimentele pentru data curentă
  if (groupsStore.group?.id) {
    loadEvents();
  }
};

const nextPeriod = () => {
  const newDate = new Date(currentDate.value);
  if (currentView.value === "day") {
    newDate.setDate(newDate.getDate() + 1);
  } else if (currentView.value === "week") {
    newDate.setDate(newDate.getDate() + 7);
  } else {
    newDate.setMonth(newDate.getMonth() + 1);
  }
  currentDate.value = newDate;
  // Reîncarcă evenimentele pentru data curentă
  if (groupsStore.group?.id) {
    loadEvents();
  }
};

const changeView = (view: "day" | "week" | "month") => {
  currentView.value = view;
  // Reîncarcă evenimentele pentru noul view
  if (groupsStore.group?.id) {
    loadEvents();
  }
};

const goToToday = () => {
  currentDate.value = new Date();
  // Reîncarcă evenimentele pentru data curentă
  if (groupsStore.group?.id) {
    loadEvents();
  }
};

const openModalForHour = (hour: number) => {
  editingEvent.value = null; // Reset editing mode
  selectedDate.value = new Date(currentDate.value);
  selectedHour.value = hour;
  isModalOpen.value = true;
};

const openModalForDayAndHour = (date: string, hour: number) => {
  editingEvent.value = null; // Reset editing mode
  selectedDate.value = new Date(date);
  selectedHour.value = hour;
  isModalOpen.value = true;
};

const openModalForDay = (date: string) => {
  editingEvent.value = null; // Reset editing mode
  selectedDate.value = new Date(date);
  selectedHour.value = new Date().getHours(); // Default to current hour
  isModalOpen.value = true;
};

const handleEventClick = (event: IEventModel) => {
  editingEvent.value = event;
  // Setează data și ora pentru modal
  const eventStartDate = new Date(event.startDate);
  selectedDate.value = eventStartDate;
  selectedHour.value = eventStartDate.getHours();
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
  editingEvent.value = null;
};

const handleSaveEvent = async (eventData: any) => {
  try {
    isLoading.value = true;
    errorMessage.value = "";
    
    if (editingEvent.value && eventData.id) {
      // Modul de editare - actualizează evenimentul existent
      const eventToUpdate: IEventModel = {
        id: eventData.id,
        title: eventData.title,
        startDate: eventData.startDate,
        endDate: eventData.endDate,
        createdByUserId: editingEvent.value.createdByUserId, // Păstrează creatorul original
        isShared: eventData.isShared,
      };
      
      await eventStore.updateEvent(eventToUpdate);
    } else {
      // Modul de creare - adaugă eveniment nou
      const eventToAdd: IEventModel = {
        id: 0, // Va fi setat de server
        title: eventData.title,
        startDate: eventData.startDate,
        endDate: eventData.endDate,
        createdByUserId: userStore.userData.id,
        isShared: eventData.isShared,
      };
      
      // Adaugă evenimentul în coadă (va fi procesat cap-coadă)
      await eventStore.queueEvent(eventToAdd);
    }
    
    // Evenimentele vor fi actualizate automat de store după procesare
    
    // Close modal
    closeModal();
  } catch (error: any) {
    errorMessage.value = error?.message || "Failed to save event";
  } finally {
    isLoading.value = false;
  }
};

const handleDeleteEvent = async (eventId: number) => {
  try {
    isLoading.value = true;
    errorMessage.value = "";
    
    await eventStore.deleteEvent(eventId);
    
    // Evenimentele vor fi actualizate automat de store după ștergere
    
    // Close modal
    closeModal();
  } catch (error: any) {
    errorMessage.value = error?.message || "Failed to delete event";
  } finally {
    isLoading.value = false;
  }
};

const loadEvents = async () => {
  if (!groupsStore.group?.id) {
    errorMessage.value = "No group found. Please join or create a group first.";
    console.warn("No group found, cannot load events");
    return;
  }

  isLoading.value = true;
  errorMessage.value = "";
  try {
    console.log("Loading events for group:", groupsStore.group.id, "View:", currentView.value, "Date:", currentDate.value);
    // Trimitem view-mode-ul curent și data curentă
    const events = await eventStore.getEventsForGroup(
      groupsStore.group.id,
      currentView.value,
      currentDate.value
    );
    console.log("Events loaded:", events?.length || 0, "events");
  } catch (error: any) {
    console.error("Error loading events:", error);
    errorMessage.value = error?.message || "Failed to load events";
  } finally {
    isLoading.value = false;
  }
};

// Watch pentru a reîncărca evenimentele când se schimbă data
// View-ul este gestionat de funcția changeView pentru a evita dublă încărcare
watch(currentDate, () => {
  if (groupsStore.group?.id) {
    loadEvents();
  }
}, { deep: true, immediate: false });

// Watch separat pentru view (ca backup, dar changeView este principalul handler)
watch(currentView, () => {
  if (groupsStore.group?.id) {
    loadEvents();
  }
}, { immediate: false });

onMounted(async () => {
  // Check if user has a group
  await groupsStore.getUserGroups();
  
  if (!groupsStore.group) {
    // Redirect to groups page if no group
    router.push("/groups");
    return;
  }

  // Load events for the group
  await loadEvents();
});
</script>

<style scoped>
/* Force light color scheme */
.events-force-light {
  color-scheme: light;
}

.events-page-background {
  background: linear-gradient(to bottom right, var(--color-background), var(--color-card), var(--color-background));
  background-color: var(--color-background) !important;
}

.events-text-dark {
  color: var(--color-text-dark) !important;
}

.events-text-muted {
  color: var(--color-text-muted) !important;
}

.events-btn-active {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
  color: #FFFFFF !important;
}

.events-btn-secondary {
  background: var(--color-background) !important;
  color: var(--color-text-dark) !important;
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.events-btn-secondary:hover {
  background: rgba(45, 125, 210, 0.1) !important;
  border-color: var(--color-primary);
}

.events-card {
  background: var(--color-card) !important;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.events-card-bg {
  background-color: var(--color-card) !important;
}

.events-bg-light {
  background-color: #F9FAFB !important;
}

.events-icon-bg {
  background: rgba(45, 125, 210, 0.1) !important;
}

.events-primary {
  color: var(--color-primary) !important;
}

.events-border-primary {
  border-color: var(--color-primary);
}

/* Custom breakpoint for extra small screens */
@media (max-width: 400px) {
  .xs\:inline {
    display: inline !important;
  }
}
</style>
