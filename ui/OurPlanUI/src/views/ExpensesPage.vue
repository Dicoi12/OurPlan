<template>
  <div class="p-4 min-h-screen expenses-page-background">
    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="mb-6">
        <h1 class="text-3xl font-bold expenses-text-dark mb-2">Cheltuieli</h1>
        <p class="expenses-text-muted">Gestionează și urmărește cheltuielile tale</p>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
        <div class="expenses-card rounded-xl p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm expenses-text-muted mb-1">Total Luna Curentă</p>
              <p class="text-2xl font-bold expenses-text-dark">{{ formatCurrency(currentMonthTotal) }}</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-blue-100 flex items-center justify-center">
              <i class="pi pi-wallet text-blue-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="expenses-card rounded-xl p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm expenses-text-muted mb-1">Număr Cheltuieli</p>
              <p class="text-2xl font-bold expenses-text-dark">{{ currentMonthExpenses.length }}</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-green-100 flex items-center justify-center">
              <i class="pi pi-list text-green-600 text-xl"></i>
            </div>
          </div>
        </div>

        <div class="expenses-card rounded-xl p-4 shadow-sm">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm expenses-text-muted mb-1">Medie Zilnică</p>
              <p class="text-2xl font-bold expenses-text-dark">{{ formatCurrency(dailyAverage) }}</p>
            </div>
            <div class="w-12 h-12 rounded-lg bg-purple-100 flex items-center justify-center">
              <i class="pi pi-chart-line text-purple-600 text-xl"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Chart Section -->
        <div class="lg:col-span-2">
          <div class="expenses-card rounded-2xl shadow-lg p-6">
            <div class="mb-4">
              <h2 class="text-xl font-bold expenses-text-dark mb-2">Grafic Cheltuieli - {{ currentMonthLabel }}</h2>
              <p class="text-sm expenses-text-muted">Distribuție pe categorii</p>
            </div>
            <div class="chart-container" style="height: 400px;">
              <canvas ref="chartCanvas"></canvas>
            </div>
          </div>
        </div>

        <!-- Add Expense Form -->
        <div class="lg:col-span-1">
          <div class="expenses-card rounded-2xl shadow-lg p-6">
            <h2 class="text-xl font-bold expenses-text-dark mb-4">Adaugă Cheltuială</h2>
            <form @submit.prevent="handleAddExpense" class="space-y-4">
              <div>
                <label class="block text-sm font-medium expenses-text-dark mb-2">
                  Titlu
                </label>
                <input
                  v-model="newExpense.title"
                  type="text"
                  required
                  class="w-full px-4 py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="ex: Cumpărături"
                />
              </div>

              <div>
                <label class="block text-sm font-medium expenses-text-dark mb-2">
                  Sumă (RON)
                </label>
                <input
                  v-model.number="newExpense.amount"
                  type="number"
                  step="0.01"
                  min="0"
                  required
                  class="w-full px-4 py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="0.00"
                />
              </div>

              <div>
                <label class="block text-sm font-medium expenses-text-dark mb-2">
                  Categorie
                </label>
                <select
                  v-model="newExpense.category"
                  required
                  class="w-full px-4 py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  <option value="">Selectează categoria</option>
                  <option value="Mâncare">Mâncare</option>
                  <option value="Transport">Transport</option>
                  <option value="Divertisment">Divertisment</option>
                  <option value="Utilități">Utilități</option>
                  <option value="Sănătate">Sănătate</option>
                  <option value="Shopping">Shopping</option>
                  <option value="Altele">Altele</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium expenses-text-dark mb-2">
                  Dată
                </label>
                <input
                  v-model="newExpense.date"
                  type="date"
                  required
                  class="w-full px-4 py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label class="block text-sm font-medium expenses-text-dark mb-2">
                  Descriere (opțional)
                </label>
                <textarea
                  v-model="newExpense.description"
                  rows="3"
                  class="w-full px-4 py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Detalii suplimentare..."
                ></textarea>
              </div>

              <button
                type="submit"
                class="w-full py-3 rounded-lg expenses-btn-active text-white font-medium transition-all duration-200 hover:shadow-lg"
              >
                <i class="pi pi-plus mr-2"></i>
                Adaugă Cheltuială
              </button>
            </form>
          </div>
        </div>
      </div>

      <!-- Expenses List -->
      <div class="mt-6">
        <div class="expenses-card rounded-2xl shadow-lg p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold expenses-text-dark">Istoric Cheltuieli</h2>
            <div class="flex items-center gap-2">
              <select
                v-model="selectedMonth"
                @change="updateChart"
                class="px-4 py-2 rounded-lg border border-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option v-for="month in availableMonths" :key="month.value" :value="month.value">
                  {{ month.label }}
                </option>
              </select>
            </div>
          </div>

          <div v-if="filteredExpenses.length === 0" class="text-center py-12">
            <i class="pi pi-inbox text-5xl expenses-text-muted opacity-50 mb-4"></i>
            <p class="expenses-text-muted">Nu există cheltuieli pentru această perioadă</p>
          </div>

          <div v-else class="space-y-3">
            <div
              v-for="expense in filteredExpenses"
              :key="expense.id"
              class="flex items-center justify-between p-4 rounded-lg border border-gray-100 hover:bg-gray-50 transition-colors"
            >
              <div class="flex items-center gap-4 flex-1">
                <div
                  class="w-12 h-12 rounded-lg flex items-center justify-center"
                  :class="getCategoryColorClass(expense.category)"
                >
                  <i :class="getCategoryIcon(expense.category)" class="text-white text-xl"></i>
                </div>
                <div class="flex-1">
                  <h3 class="font-semibold expenses-text-dark">{{ expense.title }}</h3>
                  <p class="text-sm expenses-text-muted">
                    {{ expense.category }} • {{ formatDate(expense.date) }}
                  </p>
                  <p v-if="expense.description" class="text-sm expenses-text-muted mt-1">
                    {{ expense.description }}
                  </p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-xl font-bold expenses-text-dark">{{ formatCurrency(expense.amount) }}</p>
                <button
                  @click="handleDeleteExpense(expense.id)"
                  class="mt-2 text-red-500 hover:text-red-700 transition-colors"
                  title="Șterge cheltuiala"
                >
                  <i class="pi pi-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, watch, nextTick } from "vue";
import { useExpensesStore } from "../stores/expensesStore";
import { useUserStore } from "../stores/userStore";
import type { IExpenseModel } from "../interfaces";
import { Chart, registerables } from "chart.js";

Chart.register(...registerables);

const expensesStore = useExpensesStore();
const userStore = useUserStore();

const chartCanvas = ref<HTMLCanvasElement | null>(null);
let chartInstance: Chart | null = null;

const newExpense = ref<Partial<IExpenseModel>>({
  title: "",
  amount: 0,
  category: "",
  date: new Date().toISOString().split("T")[0],
  description: "",
});

const selectedMonth = ref<string>("");

const currentDate = new Date();
const currentYear = currentDate.getFullYear();
const currentMonth = currentDate.getMonth();

// Generăm lista de luni disponibile (ultimele 6 luni + luna curentă)
const availableMonths = computed(() => {
  const months = [];
  const today = new Date();
  for (let i = 6; i >= 0; i--) {
    const date = new Date(today.getFullYear(), today.getMonth() - i, 1);
    months.push({
      value: `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}`,
      label: date.toLocaleDateString("ro-RO", { month: "long", year: "numeric" }),
    });
  }
  return months;
});

// Setăm luna curentă ca default
onMounted(() => {
  selectedMonth.value = `${currentYear}-${String(currentMonth + 1).padStart(2, "0")}`;
  initializeDummyData();
  nextTick(() => {
    updateChart();
  });
});

const selectedYear = computed(() => {
  return parseInt(selectedMonth.value.split("-")[0]);
});

const selectedMonthIndex = computed(() => {
  return parseInt(selectedMonth.value.split("-")[1]) - 1;
});

const currentMonthExpenses = computed(() => {
  return expensesStore.getExpensesForMonth(selectedYear.value, selectedMonthIndex.value);
});

const currentMonthTotal = computed(() => {
  return expensesStore.getTotalForMonth(selectedYear.value, selectedMonthIndex.value);
});

const dailyAverage = computed(() => {
  const daysInMonth = new Date(selectedYear.value, selectedMonthIndex.value + 1, 0).getDate();
  return daysInMonth > 0 ? currentMonthTotal.value / daysInMonth : 0;
});

const currentMonthLabel = computed(() => {
  const date = new Date(selectedYear.value, selectedMonthIndex.value, 1);
  return date.toLocaleDateString("ro-RO", { month: "long", year: "numeric" });
});

const filteredExpenses = computed(() => {
  return currentMonthExpenses.value;
});

const initializeDummyData = () => {
  // Adăugăm date dummy doar dacă nu există deja cheltuieli
  if (expensesStore.expenses.length === 0) {
    const categories = ["Mâncare", "Transport", "Divertisment", "Utilități", "Sănătate", "Shopping"];
    const titles = [
      "Cumpărături alimentare",
      "Benzină",
      "Cinema",
      "Factură electricitate",
      "Consult medic",
      "Haine noi",
      "Restaurant",
      "Taxi",
      "Netflix",
      "Factură apă",
    ];

    const today = new Date();
    for (let i = 0; i < 15; i++) {
      const date = new Date(today);
      date.setDate(date.getDate() - Math.floor(Math.random() * 30));
      
      const expense: IExpenseModel = {
        id: i + 1,
        title: titles[Math.floor(Math.random() * titles.length)],
        amount: Math.round((Math.random() * 500 + 10) * 100) / 100,
        category: categories[Math.floor(Math.random() * categories.length)],
        date: date,
        createdByUserId: userStore.userData?.id || 1,
        description: i % 3 === 0 ? "Cheltuială recurentă" : undefined,
      };
      expensesStore.addExpense(expense);
    }
  }
};

const updateChart = () => {
  if (!chartCanvas.value) return;

  const expensesByCategory = expensesStore.getExpensesByCategoryForMonth(
    selectedYear.value,
    selectedMonthIndex.value
  );

  const categories = Object.keys(expensesByCategory);
  const amounts = Object.values(expensesByCategory);

  const colors = [
    "rgba(45, 125, 210, 0.8)",
    "rgba(34, 197, 94, 0.8)",
    "rgba(251, 146, 60, 0.8)",
    "rgba(168, 85, 247, 0.8)",
    "rgba(239, 68, 68, 0.8)",
    "rgba(59, 130, 246, 0.8)",
    "rgba(236, 72, 153, 0.8)",
  ];

  if (chartInstance) {
    chartInstance.destroy();
  }

  chartInstance = new Chart(chartCanvas.value, {
    type: "doughnut",
    data: {
      labels: categories,
      datasets: [
        {
          label: "Cheltuieli (RON)",
          data: amounts,
          backgroundColor: colors.slice(0, categories.length),
          borderWidth: 2,
          borderColor: "#fff",
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "bottom",
          labels: {
            padding: 15,
            font: {
              size: 12,
            },
          },
        },
        tooltip: {
          callbacks: {
            label: (context) => {
              const label = context.label || "";
              const value = context.parsed || 0;
              const total = amounts.reduce((a, b) => a + b, 0);
              const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
              return `${label}: ${formatCurrency(value)} (${percentage}%)`;
            },
          },
        },
      },
    },
  });
};

watch(selectedMonth, () => {
  nextTick(() => {
    updateChart();
  });
});

watch(
  () => expensesStore.expenses,
  () => {
    nextTick(() => {
      updateChart();
    });
  },
  { deep: true }
);

const handleAddExpense = () => {
  if (!newExpense.value.title || !newExpense.value.amount || !newExpense.value.category || !newExpense.value.date) {
    return;
  }

  const expense: IExpenseModel = {
    id: Date.now(),
    title: newExpense.value.title,
    amount: newExpense.value.amount,
    category: newExpense.value.category,
    date: new Date(newExpense.value.date),
    createdByUserId: userStore.userData?.id || 1,
    description: newExpense.value.description,
  };

  expensesStore.addExpense(expense);

  // Reset form
  newExpense.value = {
    title: "",
    amount: 0,
    category: "",
    date: new Date().toISOString().split("T")[0],
    description: "",
  };
};

const handleDeleteExpense = (id: number) => {
  if (confirm("Ești sigur că vrei să ștergi această cheltuială?")) {
    expensesStore.removeExpense(id);
  }
};

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat("ro-RO", {
    style: "currency",
    currency: "RON",
  }).format(amount);
};

const formatDate = (date: Date | string): string => {
  const d = typeof date === "string" ? new Date(date) : date;
  return d.toLocaleDateString("ro-RO", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
};

const getCategoryColorClass = (category: string): string => {
  const colors: Record<string, string> = {
    Mâncare: "bg-orange-500",
    Transport: "bg-blue-500",
    Divertisment: "bg-purple-500",
    Utilități: "bg-yellow-500",
    Sănătate: "bg-red-500",
    Shopping: "bg-pink-500",
    Altele: "bg-gray-500",
  };
  return colors[category] || "bg-gray-500";
};

const getCategoryIcon = (category: string): string => {
  const icons: Record<string, string> = {
    Mâncare: "pi pi-shopping-bag",
    Transport: "pi pi-car",
    Divertisment: "pi pi-ticket",
    Utilități: "pi pi-bolt",
    Sănătate: "pi pi-heart",
    Shopping: "pi pi-shopping-cart",
    Altele: "pi pi-ellipsis-h",
  };
  return icons[category] || "pi pi-circle";
};
</script>

<style scoped>
.expenses-page-background {
  background: linear-gradient(to bottom right, var(--color-background), var(--color-card), var(--color-background));
}

.expenses-text-dark {
  color: var(--color-text-dark);
}

.expenses-text-muted {
  color: var(--color-text-muted);
}

.expenses-btn-active {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
}

.expenses-card {
  background: var(--color-card);
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.chart-container {
  position: relative;
}
</style>

