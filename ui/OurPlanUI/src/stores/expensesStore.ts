import { defineStore } from "pinia";
import type { IExpenseModel } from "../interfaces";

export const useExpensesStore = defineStore("expensesStore", {
  state: (): {
    expenses: IExpenseModel[];
  } => {
    return {
      expenses: [],
    };
  },
  actions: {
    addExpense(expense: IExpenseModel) {
      this.expenses.push(expense);
      // Sortăm după dată, cele mai recente primele
      this.expenses.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
    },
    removeExpense(id: number) {
      this.expenses = this.expenses.filter((e) => e.id !== id);
    },
    getExpensesForMonth(year: number, month: number): IExpenseModel[] {
      return this.expenses.filter((expense) => {
        const expenseDate = new Date(expense.date);
        return expenseDate.getFullYear() === year && expenseDate.getMonth() === month;
      });
    },
    getTotalForMonth(year: number, month: number): number {
      return this.getExpensesForMonth(year, month).reduce((total, expense) => total + expense.amount, 0);
    },
    getExpensesByCategoryForMonth(year: number, month: number): Record<string, number> {
      const expenses = this.getExpensesForMonth(year, month);
      const byCategory: Record<string, number> = {};
      
      expenses.forEach((expense) => {
        if (byCategory[expense.category]) {
          byCategory[expense.category] += expense.amount;
        } else {
          byCategory[expense.category] = expense.amount;
        }
      });
      
      return byCategory;
    },
  },
  getters: {
    totalExpenses: (state) => {
      return state.expenses.reduce((total, expense) => total + expense.amount, 0);
    },
  },
});

