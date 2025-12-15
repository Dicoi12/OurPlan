import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IUserModel } from "../interfaces";
import type { IToken } from "../types/InteralInterfaces";
import { getCookie } from "./helper";

// Constante pentru localStorage keys
const USER_DATA_KEY = "userData";

// Funcții helper pentru localStorage
function saveUserDataToStorage(userData: IUserModel): void {
  try {
    localStorage.setItem(USER_DATA_KEY, JSON.stringify(userData));
  } catch (error) {
    console.error("Error saving user data to localStorage:", error);
  }
}

function loadUserDataFromStorage(): IUserModel | null {
  try {
    const stored = localStorage.getItem(USER_DATA_KEY);
    if (stored) {
      const userData = JSON.parse(stored);
      // Convertim CreatedAt din string în Date
      if (userData.CreatedAt) {
        userData.CreatedAt = new Date(userData.CreatedAt);
      }
      return userData;
    }
  } catch (error) {
    console.error("Error loading user data from localStorage:", error);
  }
  return null;
}

function removeUserDataFromStorage(): void {
  try {
    localStorage.removeItem(USER_DATA_KEY);
  } catch (error) {
    console.error("Error removing user data from localStorage:", error);
  }
}

export const useUserStore = defineStore("userStore", {
  state: (): {
    token?: string;
    userData: IUserModel;
    isLoading: boolean;
  } => {
    // Încercăm să încărcăm userData din localStorage
    const storedUserData = loadUserDataFromStorage();
    
    return {
      token: getCookie("token"),
      userData: storedUserData || {
        id: 0,
        username: "",
        email: "",
        createdAt: new Date() as Date,
      },
      isLoading: false,
    };
  },
  getters: {
    isAuthenticated: (state) => !!state.token,
    hasUserData: (state) => state.userData.id > 0 && state.userData.username !== "",
  },
  actions: {
    async login(
      email: string,
      password?: string
    ): Promise<IToken | undefined> {
      const payload = {
        Email: email,
        Password: password,
      };
      try {
        const data = await fetchApi("Auth/Login", "POST", payload);
        document.cookie = `token=${data.token}; path=/; max-age=3600; samesite=strict`;
        console.log("Token set in cookie:", document.cookie);
        this.syncTokenFromCookie();
        
        // După login, încărcăm datele utilizatorului
        await this.loadCurrentUser();
        
        return data;
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
        const data = await fetchApi("Auth/Register", "POST", payload);
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
    async loadCurrentUser(): Promise<IUserModel | null> {
      // Verificăm dacă există token
      if (!this.token) {
        this.syncTokenFromCookie();
      }
      
      if (!this.token) {
        console.log("No token found, cannot load user data");
        return null;
      }

      // Dacă avem deja userData valid, nu mai facem request
      if (this.hasUserData) {
        return this.userData;
      }

      this.isLoading = true;
      try {
        const data = await fetchApi("User/me", "GET");
        const userData = data as IUserModel;
        
        // Convertim CreatedAt din string în Date dacă este necesar
        if (typeof userData.createdAt === 'string') {
          userData.createdAt = new Date(userData.createdAt);
        }
        
        this.userData = userData;
        saveUserDataToStorage(userData);
        return userData;
      } catch (error) {
        console.error("Error loading current user:", error);
        // Dacă request-ul eșuează (ex: token invalid), ștergem datele
        this.userData = {
          id: 0,
          username: "",
          email: "",
          createdAt: new Date() as Date,
        };
        removeUserDataFromStorage();
        return null;
      } finally {
        this.isLoading = false;
      }
    },
    
    logout() {
      this.userData = {
        id: 0,
        username: "",
        email: "",
        createdAt: new Date() as Date,
      };
      // Ștergem token din cookie
      document.cookie = "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
      // Ștergem userData din localStorage
      removeUserDataFromStorage();
      this.token = undefined;
    },
    
    syncTokenFromCookie() {
      this.token = getCookie('token') || undefined;
    },

  },
});