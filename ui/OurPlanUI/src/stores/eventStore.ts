import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IServiceResult } from "../types/InteralInterfaces";
import type { IEventModel } from "../interfaces";

interface QueuedEvent {
  event: IEventModel;
  resolve: (value: IServiceResult<IEventModel | undefined>) => void;
  reject: (error: any) => void;
}

export const useEventStore = defineStore("eventStore", {
  state: (): {
    events?: IEventModel[];
    eventQueue: QueuedEvent[];
    isProcessingQueue: boolean;
    lastGroupId?: number;
    lastViewMode?: string;
    lastDate?: Date | string;
  } => {
    return {
      events: undefined,
      eventQueue: [],
      isProcessingQueue: false,
      lastGroupId: undefined,
      lastViewMode: undefined,
      lastDate: undefined,
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
    async getEventsForGroup(
      groupId: number,
      viewMode: string = "day",
      date?: Date | string
    ): Promise<IEventModel[] | undefined> {
      try {
        // Salvează contextul pentru reîncărcare ulterioară
        this.lastGroupId = groupId;
        this.lastViewMode = viewMode;
        this.lastDate = date;
        
        // Formatăm data dacă este furnizată
        let url = `Event/group/${groupId}?viewMode=${viewMode}`;
        if (date) {
          const dateStr =
            typeof date === "string"
              ? date
              : date.toISOString().split("T")[0];
          url += `&date=${dateStr}`;
        }
        console.log("Fetching events from URL:", url);
        const data = await fetchApi(url, "GET");
        console.log("API response:", data);
        // Handle both direct array and wrapped in IServiceResult
        const events = Array.isArray(data) ? data : (data as IServiceResult<IEventModel[]>).result || [];
        this.events = events;
        console.log("Events loaded for group:", groupId, "ViewMode:", viewMode, "Date:", date, "Events count:", events.length);
        return this.events;
      } catch (error) {
        console.error("Error getting events for group:", error);
        this.events = [];
        return [];
      }
    },
    async _addEventToApi(event: IEventModel): Promise<IServiceResult<IEventModel | undefined>> {
      try {
        const data = await fetchApi("Event", "POST", event);
        return data as IServiceResult<IEventModel | undefined>;
      } catch (error) {
        console.error("Error adding event:", error);
        throw error;
      }
    },
    async queueEvent(event: IEventModel): Promise<IServiceResult<IEventModel | undefined>> {
      return new Promise((resolve, reject) => {
        // Adaugă evenimentul în coadă
        this.eventQueue.push({
          event,
          resolve,
          reject,
        });

        // Pornește procesarea cozii dacă nu se procesează deja
        if (!this.isProcessingQueue) {
          this.processQueue();
        }
      });
    },
    async processQueue(): Promise<void> {
      // Dacă se procesează deja sau coada este goală, nu face nimic
      if (this.isProcessingQueue || this.eventQueue.length === 0) {
        return;
      }

      this.isProcessingQueue = true;

      while (this.eventQueue.length > 0) {
        const queuedEvent = this.eventQueue.shift();
        if (!queuedEvent) break;

        try {
          // Procesează evenimentul
          const result = await this._addEventToApi(queuedEvent.event);
          
          // Dacă adăugarea a reușit, actualizează lista de evenimente
          if (result && result.result) {
            // Reîncarcă evenimentele folosind contextul salvat
            if (this.lastGroupId) {
              // Dacă avem context de grup, reîncărcăm pentru grup
              await this.getEventsForGroup(
                this.lastGroupId,
                this.lastViewMode || "day",
                this.lastDate
              );
            } else {
              // Altfel, reîncărcăm pentru utilizator
              await this.getEventsForCurrentUser();
            }
          }

          // Rezolvă promise-ul pentru acest eveniment
          queuedEvent.resolve(result);
        } catch (error) {
          // Reject promise-ul pentru acest eveniment
          queuedEvent.reject(error);
        }
      }

      this.isProcessingQueue = false;
    },
    async addEvent(event: IEventModel) {
      // Păstrăm metoda veche pentru compatibilitate, dar o redirecționăm către coadă
      return this.queueEvent(event);
    },
  },
  getters: {},
});
