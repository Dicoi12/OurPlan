import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IGroupModel } from "../interfaces";

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
        this.group = data as IGroupModel;
        return this.group;
      } catch (error) {
        console.error("Error logging in:", error);
      }
    },
  },
  getters: {},
});
