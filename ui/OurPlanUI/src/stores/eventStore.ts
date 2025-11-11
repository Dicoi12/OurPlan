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
        var rez = data as IServiceResult<IEventModel[] | undefined>;
        this.events = rez.result;
        return this.events;
      } catch (error) {
        console.error("Error logging in:", error);
      }
    },
      async addEvent() {
      try {
        const data = await fetchApi("Event", "POST");
        return data as IServiceResult<IEventModel | undefined>;
      } catch (error) {
        console.error("Error logging in:", error);
      }
    },
  },
  getters: {},
});
