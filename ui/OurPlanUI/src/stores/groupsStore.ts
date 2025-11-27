import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IGroupModel } from "../interfaces";
import type { IServiceResult } from "../types/InteralInterfaces";

export const useGroupsStore = defineStore("groupsStore", {
  state: (): {
    group?: IGroupModel;
    groups: IGroupModel[];
  } => {
    return {
      group: undefined,
      groups: [],
    };
  },
  persist: {
    enabled: true,
    strategies: [
      {
        key: 'groupsStore',
        storage: localStorage,
      },
    ],
  } as any,
  actions: {
    async getUserGroups(): Promise<IServiceResult<IGroupModel> | undefined> {
      try {
        const data = await fetchApi("Group", "GET");
        var rez = data as IServiceResult<IGroupModel>;
        this.group = rez.result;
        this.groups = rez.result ? [rez.result] : [];
      } catch (error) {
        console.error("Error getting user groups:", error);
        return undefined;
      }
    },
    async createGroup(name?: string): Promise<IGroupModel | undefined> {
      try {
        // GroupModel DTO structure - adjust based on your backend DTO
        const body: { Name?: string } = {};
        if (name) {
          body.Name = name;
        }
        const data = await fetchApi("Group", "POST", body);
        
        // Handle different response formats
        let group: IGroupModel | undefined;
        if (!data) {
          // Empty response - refresh groups to get the newly created group
          await this.getUserGroups();
          return this.group;
        } else if (Array.isArray(data)) {
          // If it's an array, take the first one
          group = data[0];
        } else if ((data as IServiceResult<IGroupModel>).result) {
          // Wrapped in IServiceResult
          group = (data as IServiceResult<IGroupModel>).result;
        } else if ((data as IGroupModel).id) {
          // Direct IGroupModel
          group = data as IGroupModel;
        }
        
        if (group) {
          this.group = group;
          // Add to groups list
          if (!this.groups.find(g => g.id === group!.id)) {
            this.groups.push(group);
          }
        } else {
          // If we couldn't parse the response, refresh groups list
          await this.getUserGroups();
        }
        
        return this.group;
      } catch (error) {
        console.error("Error creating group:", error);
        throw error;
      }
    },
    async generateGroupToken(groupId: number): Promise<string | undefined> {
      try {
        const data = await fetchApi("GroupToken/groupId", "GET", undefined, { groupId });
        var rez = data as IServiceResult<string | undefined>;
        return rez.result;
      } catch (error) {
        console.error("Error generating group token:", error);
        throw error;
      }
    },
    async joinGroup(token: string): Promise<void> {
      try {
        if (!token || !token.trim()) {
          throw { ValidationMessage: "Token cannot be empty" };
        }
        
        // GroupToken/join expects a string token in the body
        // [FromBody] string in ASP.NET Core expects the string to be JSON-encoded
        // JSON.stringify("token") produces "\"token\"" which is correct
        await fetchApi("GroupToken/join", "POST", token);
        
        // Backend returns Ok({ message: "User successfully joined the group" }) on success
        // or BadRequest(result.ValidationMessage) on failure
        // If we get here without an error, the join was successful
        // Refresh group data after joining
        await this.getUserGroups();
      } catch (error: any) {
        console.error("Error joining group:", error);
        // Extract ValidationMessage if present (from backend BadRequest)
        // or use message from error object
        const validationMessage = error?.ValidationMessage || error?.message || "Failed to join group";
        throw { ValidationMessage: validationMessage, ...error };
      }
    },
  },
  getters: {},
});
