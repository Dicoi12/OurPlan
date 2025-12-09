import { defineStore } from "pinia";
import fetchApi from "../stores/fetch";
import type { IServiceResult } from "../types/InteralInterfaces";
import type { ITaskModel } from "../interfaces";

export const useTasksStore = defineStore("tasksStore", {
  state: (): {
    tasks: ITaskModel[];
  } => {
    return {
      tasks: [],
    };
  },
  actions: {
    async getTasksForCurrentUser(): Promise<ITaskModel[] | undefined> {
      try {
        const data = await fetchApi("Task", "GET");
        // Handle both direct array and wrapped in IServiceResult
        const tasks = Array.isArray(data) ? data : (data as IServiceResult<ITaskModel[]>).result || [];
        this.tasks = tasks;
        return this.tasks;
      } catch (error) {
        console.error("Error getting tasks for current user:", error);
        this.tasks = [];
        return [];
      }
    },
    async getTasksForGroup(groupId: number): Promise<ITaskModel[] | undefined> {
      try {
        const data = await fetchApi(`Task/group/${groupId}`, "GET");
        // Handle both direct array and wrapped in IServiceResult
        const tasks = Array.isArray(data) ? data : (data as IServiceResult<ITaskModel[]>).result || [];
        this.tasks = tasks;
        return this.tasks;
      } catch (error) {
        console.error("Error getting tasks for group:", error);
        this.tasks = [];
        return [];
      }
    },
    async addTask(task: ITaskModel): Promise<IServiceResult<ITaskModel | undefined>> {
      try {
        const data = await fetchApi("Task", "POST", task);
        const result = data as IServiceResult<ITaskModel | undefined>;
        
        // Dacă adăugarea a reușit, actualizează lista de task-uri
        if (result && result.result) {
          await this.getTasksForCurrentUser();
        }
        
        return result;
      } catch (error) {
        console.error("Error adding task:", error);
        throw error;
      }
    },
    async updateTask(task: ITaskModel): Promise<IServiceResult<ITaskModel | undefined>> {
      try {
        const data = await fetchApi(`Task/${task.id}`, "PUT", task);
        const result = data as IServiceResult<ITaskModel | undefined>;
        
        // Dacă actualizarea a reușit, actualizează lista de task-uri
        if (result && result.result) {
          await this.getTasksForCurrentUser();
        }
        
        return result;
      } catch (error) {
        console.error("Error updating task:", error);
        throw error;
      }
    },
    async deleteTask(taskId: number): Promise<void> {
      try {
        await fetchApi(`Task/${taskId}`, "DELETE");
        // Actualizează lista de task-uri
        await this.getTasksForCurrentUser();
      } catch (error) {
        console.error("Error deleting task:", error);
        throw error;
      }
    },
    // Metode helper pentru gestionarea locală (pentru demo)
    addTaskLocal(task: ITaskModel) {
      this.tasks.push(task);
      this.tasks.sort((a, b) => {
        // Sortăm după prioritate și status
        const priorityOrder = { high: 3, medium: 2, low: 1 };
        const statusOrder = { pending: 3, in_progress: 2, completed: 1 };
        
        if (a.status !== b.status) {
          return statusOrder[b.status] - statusOrder[a.status];
        }
        return priorityOrder[b.priority] - priorityOrder[a.priority];
      });
    },
    updateTaskLocal(task: ITaskModel) {
      const index = this.tasks.findIndex((t) => t.id === task.id);
      if (index !== -1) {
        this.tasks[index] = task;
        this.tasks.sort((a, b) => {
          const priorityOrder = { high: 3, medium: 2, low: 1 };
          const statusOrder = { pending: 3, in_progress: 2, completed: 1 };
          
          if (a.status !== b.status) {
            return statusOrder[b.status] - statusOrder[a.status];
          }
          return priorityOrder[b.priority] - priorityOrder[a.priority];
        });
      }
    },
    removeTaskLocal(taskId: number) {
      this.tasks = this.tasks.filter((t) => t.id !== taskId);
    },
    toggleTaskComplete(taskId: number) {
      const task = this.tasks.find((t) => t.id === taskId);
      if (task) {
        task.isCompleted = !task.isCompleted;
        task.status = task.isCompleted ? "completed" : "pending";
        this.updateTaskLocal(task);
      }
    },
  },
  getters: {
    pendingTasks: (state) => {
      return state.tasks.filter((t) => t.status === "pending" && !t.isCompleted);
    },
    inProgressTasks: (state) => {
      return state.tasks.filter((t) => t.status === "in_progress");
    },
    completedTasks: (state) => {
      return state.tasks.filter((t) => t.status === "completed" || t.isCompleted);
    },
    highPriorityTasks: (state) => {
      return state.tasks.filter((t) => t.priority === "high" && !t.isCompleted);
    },
    tasksByPriority: (state) => {
      return {
        high: state.tasks.filter((t) => t.priority === "high"),
        medium: state.tasks.filter((t) => t.priority === "medium"),
        low: state.tasks.filter((t) => t.priority === "low"),
      };
    },
  },
});

