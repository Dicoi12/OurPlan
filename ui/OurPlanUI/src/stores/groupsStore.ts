import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IGroupModel } from "../interfaces";
import type { IServiceResult } from "../types/InteralInterfaces";

export const useGroupsStore = defineStore("groupsStore", {
  state: (): {
    group?: IGroupModel;
  } => {
    return {
      group: undefined,
    };
  },
  actions: {
    async getUserGroup(): Promise<IGroupModel | undefined> {
      try {
        const data = await fetchApi("Group", "GET");
        var rez = data as IServiceResult<IGroupModel | undefined>;
        this.group = rez.result;
        return this.group;
      } catch (error) {
        console.error("Error logging in:", error);
      }
    },
  },
  getters: {},
});
