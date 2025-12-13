<template>
  <div class="day-view">
    <div class="grid grid-cols-1">
      <div class="border-r border-gray-200">
        <div class="sticky top-0 events-card-bg z-10 border-b border-gray-200 p-2 sm:p-4">
          <h2 class="text-base sm:text-xl font-bold events-text-dark">
            {{ formatDate(currentDate, 'full') }}
          </h2>
        </div>
        <div class="relative" style="height: 1440px;">
          <!-- Time labels -->
          <div
            v-for="hour in hours"
            :key="hour"
            class="absolute left-0 w-14 sm:w-20 p-1 sm:p-2 text-xs sm:text-sm events-text-muted font-medium border-b border-gray-100 z-20"
            :style="{ top: `${hour * 60}px`, height: '60px' }"
          >
            {{ formatHour(hour) }}
          </div>
          <!-- Clickable areas for each hour - placed behind events but clickable -->
          <div
            v-for="hour in hours"
            :key="`click-${hour}`"
            class="absolute left-14 sm:left-20 right-0 border-b border-gray-100 cursor-pointer hover:bg-blue-50 transition-colors z-0"
            :style="{ top: `${hour * 60}px`, height: '60px' }"
            @click="handleHourClick(hour)"
            title="Click to add event"
          ></div>
          <!-- Linie roșie pentru ora curentă -->
          <div
            v-if="isToday"
            class="absolute current-time-line"
            :style="nowLineStyle"
          >
            <span class="current-time-dot"></span>
          </div>
          <!-- Events container - placed on top but allows clicks through empty areas -->
          <div class="absolute left-14 sm:left-20 right-0 pointer-events-none" style="height: 1440px; z-index: 10;">
            <div
              v-for="event in eventsForDay"
              :key="event.id"
              :style="getEventStyleWithLayout(event)"
              class="absolute rounded-lg p-1 sm:p-2 shadow-sm cursor-pointer hover:shadow-md transition-shadow pointer-events-auto"
              :class="[getEventColorClass(event), { 'event-current': isEventOngoing(event) }]"
              @click.stop="handleEventClick(event)"
            >
              <div class="font-semibold text-white text-xs sm:text-sm truncate">
                {{ (event as any).title || (event as any).Title }}
              </div>
              <div class="text-white text-[10px] sm:text-xs opacity-90">
                {{ formatTime((event as any).startDate || (event as any).StartDate) }} - 
                {{ formatTime((event as any).endDate || (event as any).EndDate) }}
              </div>
              <div v-if="(event as any).user?.username" class="text-white text-[10px] sm:text-xs opacity-75 mt-0.5 flex items-center gap-1">
                <i class="pi pi-user text-[8px] sm:text-[10px]"></i>
                {{ (event as any).user.username }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, onUnmounted, ref } from 'vue';
import type { IEventModel } from '../../interfaces';
import { useUserStore } from '../../stores/userStore';

interface Props {
  currentDate: Date;
  events: IEventModel[];
}

interface Emits {
  (e: 'hour-click', hour: number): void;
  (e: 'event-click', event: IEventModel): void;
}

interface EventWithLayout extends IEventModel {
  _layout?: {
    column: number;
    totalColumns: number;
    left: number;
    width: number;
  };
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const userStore = useUserStore();
const now = ref(new Date());
let tickId: number | undefined;

const hours = Array.from({ length: 24 }, (_, i) => i);

const isToday = computed(() => {
  const date = props.currentDate instanceof Date ? props.currentDate : new Date(props.currentDate);
  const today = new Date();
  return date.toDateString() === today.toDateString();
});

const currentMinutes = computed(() => now.value.getHours() * 60 + now.value.getMinutes());
const nowLineStyle = computed(() => ({ top: `${currentMinutes.value}px` }));

onMounted(() => {
  // Actualizează poziția liniei de timp curente o dată pe minut
  tickId = window.setInterval(() => {
    now.value = new Date();
  }, 60_000);
});

onUnmounted(() => {
  if (tickId) {
    clearInterval(tickId);
  }
});

const getCurrentDateString = (): string => {
  if (!props.currentDate) {
    const today = new Date().toISOString().split('T')[0];
    return today || '';
  }
  try {
    const date = props.currentDate instanceof Date 
      ? props.currentDate 
      : new Date(props.currentDate);
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

const eventsForDay = computed(() => {
  const dateStr = getCurrentDateString();
  if (!dateStr) return [];
  const filtered = props.events.filter((e: any) => {
    const startDate = e.startDate || e.StartDate;
    if (!startDate) return false;
    const eventDate = new Date(startDate);
    const eventDateStr = eventDate.toISOString().split('T')[0];
    return eventDateStr === dateStr;
  });
  
  // Calculate layout for overlapping events
  return calculateEventLayout(filtered);
});

// Helper function to check if two events overlap
const eventsOverlap = (event1: any, event2: any): boolean => {
  const start1 = new Date(event1.startDate || event1.StartDate);
  const end1 = new Date(event1.endDate || event1.EndDate);
  const start2 = new Date(event2.startDate || event2.StartDate);
  const end2 = new Date(event2.endDate || event2.EndDate);
  
  return start1 < end2 && start2 < end1;
};

// Calculate layout for events (Teams-style overlapping)
const calculateEventLayout = (events: any[]): EventWithLayout[] => {
  if (events.length === 0) return [];
  
  // Sort events by start time
  const sorted = [...events].sort((a, b) => {
    const startA = new Date(a.startDate || a.StartDate).getTime();
    const startB = new Date(b.startDate || b.StartDate).getTime();
    return startA - startB;
  });
  
  // Group overlapping events
  const groups: any[][] = [];
  
  for (const event of sorted) {
    let placed = false;
    
    // Try to place event in existing group
    for (const group of groups) {
      // Check if event overlaps with any event in this group
      const overlaps = group.some(e => eventsOverlap(e, event));
      
      if (overlaps) {
        group.push(event);
        placed = true;
        break;
      }
    }
    
    // If not placed, create new group
    if (!placed) {
      groups.push([event]);
    }
  }
  
  // Calculate column positions for each group
  const result: EventWithLayout[] = [];
  
  for (const group of groups) {
    if (group.length === 1) {
      // Single event, takes full width
      const event = { ...group[0] } as EventWithLayout;
      event._layout = {
        column: 0,
        totalColumns: 1,
        left: 0,
        width: 100,
      };
      result.push(event);
    } else {
      // Multiple overlapping events - need to assign columns
      // Sort group by start time again
      const sortedGroup = [...group].sort((a, b) => {
        const startA = new Date(a.startDate || a.StartDate).getTime();
        const startB = new Date(b.startDate || b.StartDate).getTime();
        return startA - startB;
      });
      
      // Assign columns using greedy algorithm
      const columns: any[][] = [];
      
      for (const event of sortedGroup) {
        let placedInColumn = false;
        
        // Try to place in existing column
        for (let i = 0; i < columns.length; i++) {
          const column = columns[i];
          // Check if event doesn't overlap with ANY event in column
          const overlapsWithAny = column?.some(e => eventsOverlap(e, event));
          if (!overlapsWithAny) {
            column?.push(event);
            placedInColumn = true;
            break;
          }
        }
        
        // If not placed, create new column
        if (!placedInColumn) {
          columns.push([event]);
        }
      }
      
      // Now assign positions based on columns
      const totalColumns = columns.length;
      
      for (let colIndex = 0; colIndex < columns.length; colIndex++) {
        const column = columns[colIndex];
        
        for (const event of column ?? []) {
          const eventWithLayout = { ...event } as EventWithLayout;
          eventWithLayout._layout = {
            column: colIndex,
            totalColumns: totalColumns,
            left: (colIndex / totalColumns) * 100,
            width: 100 / totalColumns,
          };
          result.push(eventWithLayout);
        }
      }
    }
  }
  
  return result;
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
  // Verificăm dacă evenimentul este al utilizatorului curent
  const isMyEvent = event.createdByUserId === userStore.userData?.id;
  
  // Evenimentele utilizatorului curent sunt albastre, ale altora sunt verzi
  if (isMyEvent) {
    return "bg-blue-500";
  } else {
    return "bg-green-500";
  }
};

const getEventStyleWithLayout = (event: EventWithLayout): any => {
  const startDate = event?.startDate;
  const endDate = event?.endDate;
  
  if (!startDate || !endDate) {
    return { top: '0px', height: '30px', left: '4px', right: '4px' };
  }
  
  const start = new Date(startDate);
  const end = new Date(endDate);
  
  const startMinutes = start.getHours() * 60 + start.getMinutes();
  const endMinutes = end.getHours() * 60 + end.getMinutes();
  const duration = endMinutes - startMinutes;
  
  const top = startMinutes;
  const height = Math.max(duration, 30);
  
  // Get layout information
  const layout = event._layout;
  
  if (layout && layout.totalColumns > 1) {
    // Multiple columns - calculate position and width
    const leftPercent = layout.left;
    const widthPercent = layout.width;
    const margin = 2; // 2px margin between events
    
    return {
      top: `${top}px`,
      height: `${height}px`,
      left: `calc(${leftPercent}% + ${margin}px)`,
      width: `calc(${widthPercent}% - ${margin * 2}px)`,
    };
  } else {
    // Single event or no layout - full width with margins
    return {
      top: `${top}px`,
      height: `${height}px`,
      left: '4px',
      right: '4px',
    };
  }
};

const isEventOngoing = (event: any): boolean => {
  if (!isToday.value) return false;
  const start = new Date(event.startDate || event.StartDate);
  const end = new Date(event.endDate || event.EndDate);
  const current = now.value;
  return start <= current && current <= end;
};

const handleHourClick = (hour: number) => {
  emit('hour-click', hour);
};

const handleEventClick = (event: IEventModel) => {
  emit('event-click', event);
};
</script>

<style scoped>
.day-view {
  min-height: 400px;
}

.current-time-line {
  position: absolute;
  left: 0;
  right: 0;
  height: 2px;
  background: #ef4444; /* roșu */
  z-index: 30;
  transform: translateY(-1px);
}

.current-time-line .current-time-dot {
  position: absolute;
  left: 12px;
  top: 50%;
  width: 10px;
  height: 10px;
  background: #ef4444;
  border-radius: 9999px;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.25);
}

.event-current {
  filter: brightness(1.15);
  box-shadow: 0 0 0 2px rgba(239, 68, 68, 0.3);
}

@media (min-width: 640px) {
  .day-view {
    min-height: 600px;
  }
}
</style>

