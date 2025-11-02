import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IUserModel } from "../interfaces";

export const useUserStore = defineStore("userStore", {
  state: (): {
    token?: string;
    userData: IUserModel;
  } => {
    return {
      token: localStorage.getItem('token') || undefined,
      userData: {
        Id: 0,
        Username: "",
        Email: "",
        CreatedAt: new Date(),
      },
    };
  },
  actions: {
    async login(
      email: string,
      password?: string
    ): Promise<IUserModel | undefined> {
      const payload = {
        Email: email,
        Password: password,
      };
      try {
        const data = await fetchApi("User/Login", "POST", payload);
        this.userData = data as IUserModel;
        localStorage.setItem('token', data.id);
        return data as IUserModel;
      } catch (error) {
        console.error("Error logging in:", error);
      }
    },
    async signup(
      userName: string,
      password?: string,
      email?: string
    ) {
      const payload = {
        UserName: userName,
        Password: password,
        email: email
      };
      try {
        const data = await fetchApi("User/SignUp", "POST", payload);
        return data;
      } catch (error) {
        console.error("Error creating user:", error);
      }
    },
    async changePassword(oldPassword: string, newPassword: string) {
      try {
        let payload = {
          userId: this.userData.Id,
          oldPassword: oldPassword,
          newPassword: newPassword
        };
        const data = await fetchApi("User/ChangePassword", "POST", payload);
        return data as boolean;
      } catch (error) {
        console.error("Error adding objective:", error);
      }

    },
    logout() {
      this.userData = {
        Id: 0,
        Username: "",
        Email: "",
        CreatedAt: new Date(),
      };
      localStorage.removeItem('token');
    },
  },
  getters: {},
});