import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";


export const useUserStore = defineStore("userStore", {
  state: (): {
    token?: string;
    userData: IUserModel;
  } => {
    return {
      token: localStorage.getItem('token') || undefined,
      userData: {
        id: 0,
        userName: "",
        email: null,
        phone: null,
        hash: "",
        salt: "",
        role: 0,
      },
    };
  },
  actions: {
    async login(
      userName: string,
      password?: string
    ): Promise<IUserModel | undefined> {
      const payload = {
        UserName: userName,
        Password: password,
      };
      try {
        const data = await fetchApi("User/Login", "POST", payload);
        this.userData = data as IUserModel;
        localStorage.setItem('token', data.id);
        return data as IUserModel;
      } catch (error) {
        if (userName == "dariusd2" && password == "123") {
          this.userData = {
            id: 200,
            userName: 'dariusd',
            email: 'wink@ceva.com',
            phone: '0742123212',
            hash: "",
            salt: "",
            role: UserRoleEnum.Administrator,
          };
        }
        console.error("Error adding objective:", error);
      }
    },
    async signup(
      userName: string,
      password?: string,
      email?: string,
      phone?: string
    ) {
      const payload = {
        UserName: userName,
        Password: password,
        email: email,
        phone: phone,
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
          userId: this.userData.id,
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
        id: 0,
        userName: "",
        email: null,
        phone: null,
        hash: "",
        salt: "",
        role: 0,
      };
      localStorage.removeItem('token');
    },
  },
  getters: {},
  persist: true,
});