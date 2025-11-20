import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IServiceResult } from "../types/InteralInterfaces";
import type { IEventModel } from "../interfaces";

export const useEventStore = defineStore("eventStore", {
  state: (): {
    events?: IEventModel[];
  } => {
    return {
      events: undefined,
    };
  },
  actions: {
    async getEventsForCurrentUser(): Promise<IEventModel[] | undefined> {
      try {
        const data = await fetchApi("Event", "GET");
        // Handle both direct array and wrapped in IServiceResult
        const events = Array.isArray(data) ? data : (data as IServiceResult<IEventModel[]>).result || [];
        this.events = events;
        return this.events;
      } catch (error) {
        console.error("Error getting events for current user:", error);
        this.events = [];
        return [];
      }
    },
    async getEventsForGroup(groupId: number): Promise<IEventModel[] | undefined> {
      try {
        const data = await fetchApi(`Event/group/${groupId}/today`, "GET");
        // Handle both direct array and wrapped in IServiceResult
        const events = Array.isArray(data) ? data : (data as IServiceResult<IEventModel[]>).result || [];
        this.events = events;
        return this.events;
      } catch (error) {
        console.error("Error getting events for group:", error);
        this.events = [];
        return [];
      }
    },
    async addEvent() {
      try {
        const data = await fetchApi("Event", "POST");
        return data as IServiceResult<IEventModel | undefined>;
      } catch (error) {
        console.error("Error adding event:", error);
        throw error;
      }
    },
  },
  getters: {},
});
