<template>
  <div class="week-view overflow-x-auto">
    <div class="min-w-[600px]">
      <div class="grid grid-cols-8 border-b border-gray-200 events-bg-light">
        <div class="p-2 sm:p-3 border-r border-gray-200"></div>
        <div
          v-for="day in weekDays"
          :key="day.date"
          class="p-2 sm:p-3 text-center border-r border-gray-200 last:border-r-0"
        >
          <div class="text-xs events-text-muted mb-1">{{ day.dayName }}</div>
          <div
            :class="[
              'w-8 h-8 sm:w-10 sm:h-10 mx-auto rounded-full flex items-center justify-center font-semibold text-xs sm:text-sm',
              day.isToday
                ? 'events-btn-active text-white'
                : 'events-text-dark events-card-bg'
            ]"
          >
            {{ day.dayNumber }}
          </div>
        </div>
      </div>
    </div>
    <div class="relative overflow-x-auto min-w-[600px]" style="height: 1440px;">
      <!-- Time labels column -->
      <div
        v-for="hour in hours"
        :key="hour"
        class="absolute left-0 w-16 sm:w-20 p-1 sm:p-2 text-xs sm:text-sm events-text-muted font-medium border-r border-gray-200 events-bg-light border-b border-gray-100"
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
          @click="handleDayHourClick(day.date ?? '', hour)"
        ></div>
        <!-- Events for this day -->
        <div
          v-for="event in getEventsForDay(day.date ?? '')"
          :key="event.id"
          :style="getEventStyle(event)"
          class="absolute left-0.5 right-0.5 sm:left-1 sm:right-1 rounded-lg p-1 sm:p-2 shadow-sm cursor-pointer hover:shadow-md transition-shadow z-10"
          :class="getEventColorClass(event)"
          @click.stop="handleEventClick(event)"
        >
          <div class="font-semibold text-white text-[10px] sm:text-xs truncate">
            {{ (event as any).title || (event as any).Title }}
          </div>
          <div class="text-white text-[10px] sm:text-xs opacity-90 hidden sm:block">
            {{ formatTime((event as any).startDate || (event as any).StartDate) }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed } from 'vue';
import type { IEventModel } from '../../interfaces';

interface Props {
  currentDate: Date;
  events: IEventModel[];
}

interface Emits {
  (e: 'day-hour-click', date: string, hour: number): void;
  (e: 'event-click', event: IEventModel): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const hours = Array.from({ length: 24 }, (_, i) => i);
const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

const getWeekStart = (date: Date): Date => {
  const d = new Date(date);
  const day = d.getDay();
  const diff = d.getDate() - day;
  return new Date(d.setDate(diff));
};

const weekDays = computed(() => {
  const start = getWeekStart(props.currentDate);
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

const getEventsForDay = (dateStr: string) => {
  if (!dateStr) return [];
  return props.events.filter((event) => {
    const e = event as any;
    const startDate = e.startDate || e.StartDate;
    if (!startDate) return false;
    const eventDate = new Date(startDate);
    const eventDateStr = eventDate.toISOString().split("T")[0];
    return eventDateStr === dateStr;
  });
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
  const startDate = event?.startDate;
  const endDate = event?.endDate;
  
  if (!startDate || !endDate) {
    return { top: '0px', height: '30px' };
  }
  
  const start = new Date(startDate);
  const end = new Date(endDate);
  
  const startMinutes = start.getHours() * 60 + start.getMinutes();
  const endMinutes = end.getHours() * 60 + end.getMinutes();
  const duration = endMinutes - startMinutes;
  
  const top = startMinutes;
  const height = Math.max(duration, 30);
  
  return {
    top: `${top}px`,
    height: `${height}px`,
  };
};

const handleDayHourClick = (date: string, hour: number) => {
  emit('day-hour-click', date, hour);
};

const handleEventClick = (event: IEventModel) => {
  emit('event-click', event);
};
</script>

<style scoped>
.week-view {
  min-height: 400px;
}

@media (min-width: 640px) {
  .week-view {
    min-height: 600px;
  }
}
</style>

