<template>
  <div class="month-view overflow-x-auto">
    <div class="min-w-[320px]">
      <div class="grid grid-cols-7 border-b border-gray-200 events-bg-light">
        <div
          v-for="dayName in dayNames"
          :key="dayName"
          class="p-1 sm:p-3 text-center font-semibold events-text-muted text-xs sm:text-sm border-r border-gray-200 last:border-r-0"
        >
          <span class="hidden sm:inline">{{ dayName }}</span>
          <span class="sm:hidden">{{ dayName.charAt(0) }}</span>
        </div>
      </div>
      <div class="grid grid-cols-7">
        <div
          v-for="day in monthDays"
          :key="day.date"
          :class="[
            'min-h-[60px] sm:min-h-[120px] p-1 sm:p-2 border-r border-b border-gray-100 cursor-pointer hover:bg-blue-50 transition-colors',
            day.isCurrentMonth
              ? 'events-card-bg'
              : 'events-bg-light',
            day.isToday
              ? 'border-2 border-blue-500'
              : ''
          ]"
          @click="handleDayClick(day.date ?? '')"
        >
          <div
            :class="[
              'text-xs sm:text-sm font-semibold mb-0.5 sm:mb-1',
              day.isToday
                ? 'events-primary'
                : day.isCurrentMonth
                ? 'events-text-dark'
                : 'events-text-muted'
            ]"
          >
            {{ day.dayNumber }}
          </div>
          <div class="space-y-0.5 sm:space-y-1">
            <div
              v-for="event in getEventsForDay(day.date || '').slice(0, 2)"
              :key="event.id"
              class="text-xs p-0.5 sm:p-1.5 rounded cursor-pointer hover:shadow-md transition-shadow truncate"
              :class="getEventColorClass(event)"
              @click.stop="handleEventClick(event)"
            >
              <div class="font-semibold text-white truncate text-[10px] sm:text-xs">
                {{ (event as any).title || (event as any).Title }}
              </div>
              <div class="text-white opacity-90 hidden sm:block">
                {{ formatTime((event as any).startDate || (event as any).StartDate) }}
              </div>
            </div>
            <div
              v-if="getEventsForDay(day.date || '').length > 2"
              class="text-[10px] sm:text-xs events-text-muted cursor-pointer hover:underline"
            >
              +{{ getEventsForDay(day.date || '').length - 2 }} more
            </div>
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
  (e: 'day-click', date: string): void;
  (e: 'event-click', event: IEventModel): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

const monthDays = computed(() => {
  const year = props.currentDate.getFullYear();
  const month = props.currentDate.getMonth();
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

const getEventsForDay = (date: string) => {
  return props.events.filter((event) => {
    const e = event as any;
    const startDate = e?.startDate;
    if (!startDate) return false;
    const eventDate = new Date(startDate);
    const eventDateStr = eventDate.toISOString().split("T")[0];
    return eventDateStr === date;
  });
};

const formatTime = (date: Date | string): string => {
  const d = typeof date === "string" ? new Date(date) : date;
  return d.toLocaleTimeString("en-US", { hour: "2-digit", minute: "2-digit" });
};

const getEventColorClass = (event: any): string => {
  const colors: Record<string, string> = {
    blue: "bg-blue-500",
    green: "bg-green-500",
    orange: "bg-orange-500",
    purple: "bg-purple-500",
    red: "bg-red-500",
  };
  return colors[event?.color || "blue"] || "bg-gray-500";
};

const handleDayClick = (date: string) => {
  emit('day-click', date);
};

const handleEventClick = (event: IEventModel) => {
  emit('event-click', event);
};
</script>

<style scoped>
.month-view {
  min-height: 300px;
}

@media (min-width: 640px) {
  .month-view {
    min-height: 500px;
  }
}
</style>

