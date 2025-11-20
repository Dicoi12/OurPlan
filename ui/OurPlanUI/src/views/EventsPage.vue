<template>
  <div class="p-4 min-h-screen events-page-background">
    <!-- Event Modal -->
    <EventModal
      :isOpen="isModalOpen"
      :initialDate="selectedDate"
      :initialHour="selectedHour"
      @close="closeModal"
      @save="handleSaveEvent"
    />

    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="mb-6">
        <h1 class="text-3xl font-bold events-text-dark mb-2">Calendar</h1>
        <p class="events-text-muted">View and manage events for your group</p>
      </div>

      <!-- View Selector and Navigation -->
      <div class="mb-6 flex items-center justify-between flex-wrap gap-4">
        <div class="flex items-center gap-2 bg-white rounded-xl p-1 shadow-sm border border-gray-100">
          <button
            v-for="view in views"
            :key="view.value"
            @click="currentView = view.value"
            :class="[
              'px-4 py-2 rounded-lg font-medium transition-all duration-200',
              currentView === view.value
                ? 'events-btn-active text-white'
                : 'events-text-muted hover:bg-gray-50'
            ]"
          >
            <i :class="view.icon" class="mr-2"></i>
            {{ view.label }}
          </button>
        </div>

        <div class="flex items-center gap-3">
          <button
            @click="previousPeriod"
            class="p-2 rounded-lg events-icon-bg hover:bg-blue-200 transition-colors"
          >
            <i class="pi pi-chevron-left events-primary"></i>
          </button>
          <span class="font-semibold events-text-dark min-w-[200px] text-center">
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
            class="px-4 py-2 rounded-lg events-btn-secondary font-medium transition-all duration-200"
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
        <div v-if="currentView === 'day'" class="day-view">
          <div class="grid grid-cols-1">
            <div class="border-r border-gray-200">
              <div class="sticky top-0 bg-white z-10 border-b border-gray-200 p-4">
                <h2 class="text-xl font-bold events-text-dark">
                  {{ formatDate(currentDate, 'full') }}
                </h2>
              </div>
              <div class="relative" style="height: 1440px;">
                <!-- Time labels -->
                <div
                  v-for="hour in hours"
                  :key="hour"
                  class="absolute left-0 w-20 p-2 text-sm events-text-muted font-medium border-b border-gray-100"
                  :style="{ top: `${hour * 60}px`, height: '60px' }"
                >
                  {{ formatHour(hour) }}
                </div>
                <!-- Clickable areas for each hour - placed behind events -->
                <div
                  v-for="hour in hours"
                  :key="`click-${hour}`"
                  class="absolute left-20 right-0 border-b border-gray-100 cursor-pointer hover:bg-blue-50 transition-colors"
                  :style="{ top: `${hour * 60}px`, height: '60px', zIndex: 1 }"
                  @click="openModalForHour(hour)"
                ></div>
                <!-- Events container - placed on top -->
                <div class="absolute left-20 right-0" style="height: 1440px; z-index: 10;">
                  <div
                    v-for="event in getEventsForDayFiltered(getCurrentDateString())"
                    :key="event.Id"
                    :style="getEventStyle(event)"
                    class="absolute left-2 right-2 rounded-lg p-2 shadow-sm cursor-pointer hover:shadow-md transition-shadow"
                    :class="getEventColorClass(event)"
                    @click.stop
                  >
                    <div class="font-semibold text-white text-sm truncate">{{ event.Title }}</div>
                    <div class="text-white text-xs opacity-90">
                      {{ formatTime(event.StartDate) }} - {{ formatTime(event.EndDate) }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Week View -->
        <div v-if="currentView === 'week'" class="week-view">
          <div class="grid grid-cols-8 border-b border-gray-200 bg-gray-50">
            <div class="p-3 border-r border-gray-200"></div>
            <div
              v-for="day in weekDays"
              :key="day.date"
              class="p-3 text-center border-r border-gray-200 last:border-r-0"
            >
              <div class="text-xs events-text-muted mb-1">{{ day.dayName }}</div>
              <div
                :class="[
                  'w-10 h-10 mx-auto rounded-full flex items-center justify-center font-semibold text-sm',
                  day.isToday
                    ? 'events-btn-active text-white'
                    : 'events-text-dark bg-white'
                ]"
              >
                {{ day.dayNumber }}
              </div>
            </div>
          </div>
          <div class="relative overflow-x-auto" style="height: 1440px;">
            <!-- Time labels column -->
            <div
              v-for="hour in hours"
              :key="hour"
              class="absolute left-0 w-20 p-2 text-sm events-text-muted font-medium border-r border-gray-200 bg-gray-50 border-b border-gray-100"
              :style="{ top: `${hour * 60}px`, height: '60px' }"
            >
              {{ formatHour(hour) }}
            </div>
            <!-- Day columns -->
            <div
              v-for="(day, dayIndex) in weekDays"
              :key="day.date"
              class="absolute border-r border-gray-100 last:border-r-0"
              :style="{ 
                left: `${80 + (dayIndex * 12.85)}%`, 
                width: '12.85%',
                height: '1440px'
              }"
            >
              <!-- Clickable hour areas -->
              <div
                v-for="hour in hours"
                :key="`${day.date}-${hour}`"
                class="absolute cursor-pointer hover:bg-blue-50 transition-colors border-b border-gray-100 z-0"
                :style="{ top: `${hour * 60}px`, height: '60px', left: 0, right: 0 }"
                @click="openModalForDayAndHour(day.date ?? '', hour)"
              ></div>
              <!-- Events for this day -->
              <div
                v-for="event in getEventsForDayFiltered(day.date ?? '')"
                :key="event.Id"
                :style="getEventStyle(event)"
                class="absolute left-1 right-1 rounded-lg p-2 shadow-sm cursor-pointer hover:shadow-md transition-shadow z-10"
                :class="getEventColorClass(event)"
                @click.stop
              >
                <div class="font-semibold text-white text-xs truncate">{{ event.Title }}</div>
                <div class="text-white text-xs opacity-90">
                  {{ formatTime(event.StartDate) }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Month View -->
        <div v-if="currentView === 'month'" class="month-view">
          <div class="grid grid-cols-7 border-b border-gray-200 bg-gray-50">
            <div
              v-for="dayName in dayNames"
              :key="dayName"
              class="p-3 text-center font-semibold events-text-muted text-sm border-r border-gray-200 last:border-r-0"
            >
              {{ dayName }}
            </div>
          </div>
          <div class="grid grid-cols-7">
            <div
              v-for="day in monthDays"
              :key="day.date"
              :class="[
                'min-h-[120px] p-2 border-r border-b border-gray-100 cursor-pointer hover:bg-blue-50 transition-colors',
                day.isCurrentMonth
                  ? 'bg-white'
                  : 'bg-gray-50',
                day.isToday
                  ? 'border-2 border-blue-500'
                  : ''
              ]"
              @click="openModalForDay(day.date ?? '')"
            >
              <div
                :class="[
                  'text-sm font-semibold mb-1',
                  day.isToday
                    ? 'events-primary'
                    : day.isCurrentMonth
                    ? 'events-text-dark'
                    : 'events-text-muted'
                ]"
              >
                {{ day.dayNumber }}
              </div>
              <div class="space-y-1">
                <div
                  v-for="event in getEventsForDay(day.date || '')"
                  :key="event.Id"
                  class="text-xs p-1.5 rounded cursor-pointer hover:shadow-md transition-shadow truncate"
                  :class="getEventColorClass(event)"
                >
                  <div class="font-semibold text-white">{{ event.Title }}</div>
                  <div class="text-white opacity-90">{{ formatTime(event.StartDate) }}</div>
                </div>
                <div
                  v-if="getEventsForDay(day.date || '').length > 3"
                  class="text-xs events-text-muted cursor-pointer hover:underline"
                >
                  +{{ getEventsForDay(day.date || '').length - 3 }} more
                </div>
              </div>
            </div>
          </div>
        </div>

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
import { ref, computed, onMounted } from "vue";
import { useGroupsStore } from "../stores/groupsStore";
import router from "../router";
import EventModal from "../components/EventModal.vue";

const groupsStore = useGroupsStore();

const currentView = ref<"day" | "week" | "month">("week");
const currentDate = ref(new Date());
const isLoading = ref(false);
const errorMessage = ref("");
const isModalOpen = ref(false);
const selectedDate = ref<Date>(new Date());
const selectedHour = ref<number | undefined>(undefined);

// Helper function to create dates
const createDate = (daysOffset: number, hours: number, minutes: number = 0): Date => {
  const date = new Date();
  date.setDate(date.getDate() + daysOffset);
  date.setHours(hours, minutes, 0, 0);
  return date;
};

// Hardcoded events for testing (Teams-like calendar)
const hardcodedEvents = ref([
  {
    Id: 1,
    Title: "Team Standup",
    StartDate: createDate(0, 9, 0),
    EndDate: createDate(0, 9, 30),
    CreatedByUserId: 1,
    IsShared: true,
    color: "blue"
  },
  {
    Id: 2,
    Title: "Client Meeting",
    StartDate: createDate(0, 10, 30),
    EndDate: createDate(0, 11, 30),
    CreatedByUserId: 2,
    IsShared: true,
    color: "green"
  },
  {
    Id: 3,
    Title: "Lunch Break",
    StartDate: createDate(0, 12, 0),
    EndDate: createDate(0, 13, 0),
    CreatedByUserId: 1,
    IsShared: false,
    color: "orange"
  },
  {
    Id: 4,
    Title: "Project Review",
    StartDate: createDate(0, 14, 0),
    EndDate: createDate(0, 15, 30),
    CreatedByUserId: 3,
    IsShared: true,
    color: "purple"
  },
  {
    Id: 5,
    Title: "Code Review",
    StartDate: createDate(0, 16, 0),
    EndDate: createDate(0, 17, 0),
    CreatedByUserId: 2,
    IsShared: true,
    color: "red"
  },
  // Tomorrow's events
  {
    Id: 6,
    Title: "Sprint Planning",
    StartDate: createDate(1, 9, 0),
    EndDate: createDate(1, 10, 30),
    CreatedByUserId: 1,
    IsShared: true,
    color: "blue"
  },
  {
    Id: 7,
    Title: "Design Workshop",
    StartDate: createDate(1, 11, 0),
    EndDate: createDate(1, 12, 30),
    CreatedByUserId: 2,
    IsShared: true,
    color: "green"
  },
  // Next week events
  {
    Id: 8,
    Title: "Quarterly Review",
    StartDate: createDate(7, 10, 0),
    EndDate: createDate(7, 12, 0),
    CreatedByUserId: 1,
    IsShared: true,
    color: "purple"
  },
]);

const views = [
  { value: "day" as const, label: "Day", icon: "pi pi-calendar" },
  { value: "week" as const, label: "Week", icon: "pi pi-calendar-times" },
  { value: "month" as const, label: "Month", icon: "pi pi-calendar-plus" },
];

const hours = Array.from({ length: 24 }, (_, i) => i);
const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

const displayEvents = computed(() => {
  return hardcodedEvents.value;
});

// Helper to get current date as string
const getCurrentDateString = (): string => {
  if (!currentDate.value) {
    const today = new Date().toISOString().split('T')[0];
    return today || '';
  }
  try {
    const date = currentDate.value instanceof Date 
      ? currentDate.value 
      : new Date(currentDate.value);
    if (isNaN(date.getTime())) {
      const today = new Date().toISOString().split('T')[0];
      return today || '';
    }
    const dateStr = date.toISOString().split('T')[0];
    return dateStr || '';
  } catch {
    const today = new Date().toISOString().split('T')[0];
    return today || '';
  }
};

// Helper to get events for a specific day
const getEventsForDayFiltered = (dateStr: string) => {
  if (!dateStr) return [];
  return displayEvents.value.filter((e: any) => {
    if (!e.StartDate) return false;
    const eventDate = new Date(e.StartDate);
    const eventDateStr = eventDate.toISOString().split('T')[0];
    return eventDateStr === dateStr;
  });
};

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

const weekDays = computed(() => {
  const start = getWeekStart(currentDate.value);
  return Array.from({ length: 7 }, (_, i) => {
    const date = new Date(start);
    date.setDate(date.getDate() + i);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const dayDate = new Date(date);
    dayDate.setHours(0, 0, 0, 0);
    return {
      date: dayDate.toISOString().split("T")[0],
      dayName: dayNames[date.getDay()],
      dayNumber: date.getDate(),
      isToday: dayDate.getTime() === today.getTime(),
    };
  });
});

const monthDays = computed(() => {
  const year = currentDate.value.getFullYear();
  const month = currentDate.value.getMonth();
  const firstDay = new Date(year, month, 1);
  const startDate = new Date(firstDay);
  startDate.setDate(startDate.getDate() - startDate.getDay());
  
  const days = [];
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  
  for (let i = 0; i < 42; i++) {
    const date = new Date(startDate);
    date.setDate(startDate.getDate() + i);
    const dayDate = new Date(date);
    dayDate.setHours(0, 0, 0, 0);
    days.push({
      date: dayDate.toISOString().split("T")[0],
      dayNumber: date.getDate(),
      isCurrentMonth: date.getMonth() === month,
      isToday: dayDate.getTime() === today.getTime(),
    });
  }
  return days;
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

const formatTime = (date: Date | string): string => {
  const d = typeof date === "string" ? new Date(date) : date;
  return d.toLocaleTimeString("en-US", { hour: "2-digit", minute: "2-digit" });
};

const formatHour = (hour: number): string => {
  if (hour === 0) return "12 AM";
  if (hour < 12) return `${hour} AM`;
  if (hour === 12) return "12 PM";
  return `${hour - 12} PM`;
};

const getEventColorClass = (event: any): string => {
  const colors: Record<string, string> = {
    blue: "bg-blue-500",
    green: "bg-green-500",
    orange: "bg-orange-500",
    purple: "bg-purple-500",
    red: "bg-red-500",
  };
  return colors[event.color || "blue"] || "bg-gray-500";
};

const getEventStyle = (event: any): any => {
  const start = new Date(event.StartDate);
  const end = new Date(event.EndDate);
  
  // Calculate minutes from midnight
  const startMinutes = start.getHours() * 60 + start.getMinutes();
  const endMinutes = end.getHours() * 60 + end.getMinutes();
  const duration = endMinutes - startMinutes;
  
  // Each hour is 60px, so each minute is 1px
  const top = startMinutes; // Position in pixels from top of day
  const height = Math.max(duration, 30); // Minimum 30px height
  
  return {
    top: `${top}px`,
    height: `${height}px`,
  };
};

const getEventsForHour = (hour: number) => {
  return displayEvents.value.filter((event) => {
    if (!event.StartDate) return false;
    const start = new Date(event.StartDate);
    const eventDate = start.toISOString().split("T")[0];
    const currentDateStr = currentDate.value.toISOString().split("T")[0];
    // Include events that start in this hour or overlap with this hour
    return eventDate === currentDateStr && (
      start.getHours() === hour || 
      (start.getHours() < hour && new Date(event.EndDate).getHours() >= hour)
    );
  });
};

const getEventsForDayAndHour = (date: string, hour: number) => {
  return displayEvents.value.filter((event) => {
    if (!event.StartDate) return false;
    const start = new Date(event.StartDate);
    const end = new Date(event.EndDate);
    const eventDateStr = start.toISOString().split("T")[0];
    // Include events that start in this hour or overlap with this hour
    return eventDateStr === date && (
      start.getHours() === hour || 
      (start.getHours() < hour && end.getHours() >= hour)
    );
  });
};

const getEventsForDay = (date: string) => {
  return displayEvents.value.filter((event) => {
    if (!event.StartDate) return false;
    const eventDate = new Date(event.StartDate);
    const eventDateStr = eventDate.toISOString().split("T")[0];
    return eventDateStr === date;
  });
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
};

const goToToday = () => {
  currentDate.value = new Date();
};

const openModalForHour = (hour: number) => {
  selectedDate.value = new Date(currentDate.value);
  selectedHour.value = hour;
  isModalOpen.value = true;
};

const openModalForDayAndHour = (date: string, hour: number) => {
  selectedDate.value = new Date(date);
  selectedHour.value = hour;
  isModalOpen.value = true;
};

const openModalForDay = (date: string) => {
  selectedDate.value = new Date(date);
  selectedHour.value = new Date().getHours(); // Default to current hour
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const handleSaveEvent = (eventData: any) => {
  // Generate a new ID for the event
  const newId = Math.max(...hardcodedEvents.value.map(e => e.Id), 0) + 1;
  
  // Add the new event to the list
  hardcodedEvents.value.push({
    Id: newId,
    Title: eventData.Title,
    StartDate: eventData.StartDate,
    EndDate: eventData.EndDate,
    CreatedByUserId: 1, // You can get this from user store
    IsShared: eventData.IsShared,
    color: eventData.color,
  });
  
  // Close modal
  closeModal();
  
  // TODO: When ready, call API to save event
  // await eventStore.createEvent(eventData);
};

onMounted(async () => {
  // Check if user has a group
  await groupsStore.getUserGroups();
  
  if (!groupsStore.group) {
    // Redirect to groups page if no group
    router.push("/groups");
    return;
  }

  // For now, we're using hardcoded events
  // Later, uncomment this to load real events:
  // await loadEvents();
});

// Uncomment this function when ready to load real events
// const loadEvents = async () => {
//   if (!groupsStore.group?.Id) {
//     errorMessage.value = "No group found. Please join or create a group first.";
//     return;
//   }

//   isLoading.value = true;
//   errorMessage.value = "";
//   try {
//     const groupEvents = await eventStore.getEventsForGroup(groupsStore.group.Id);
//     displayEvents.value = groupEvents || [];
//   } catch (error: any) {
//     errorMessage.value = error?.message || "Failed to load events";
//     displayEvents.value = [];
//   } finally {
//     isLoading.value = false;
//   }
// };
</script>

<style scoped>
.events-page-background {
  background: linear-gradient(to bottom right, var(--color-background), var(--color-card), var(--color-background));
}

.events-text-dark {
  color: var(--color-text-dark);
}

.events-text-muted {
  color: var(--color-text-muted);
}

.events-btn-active {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
}

.events-btn-secondary {
  background: var(--color-background);
  color: var(--color-text-dark);
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.events-btn-secondary:hover {
  background: rgba(45, 125, 210, 0.1);
  border-color: var(--color-primary);
}

.events-card {
  background: var(--color-card);
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.events-icon-bg {
  background: rgba(45, 125, 210, 0.1);
}

.events-primary {
  color: var(--color-primary);
}

.events-border-primary {
  border-color: var(--color-primary);
}

.day-view,
.week-view {
  min-height: 600px;
}

.month-view {
  min-height: 500px;
}
</style>
