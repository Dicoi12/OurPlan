<template>
  <div class="p-4 flex justify-center min-h-screen groups-page-background">
    <!-- If no group exists -->
    <div
      v-if="!groupsStore.group"
      class="w-full max-w-md h-min bg-white shadow-2xl rounded-2xl p-8 text-center border border-gray-100 transform transition-all duration-300 hover:shadow-3xl hover:scale-[1.02]"
    >
      <div class="mb-6">
        <div class="inline-flex items-center justify-center w-20 h-20 groups-icon-container rounded-full mb-4 shadow-lg">
          <i class="pi pi-users text-4xl text-white"></i>
        </div>
      </div>

      <h2 class="text-2xl font-bold mb-3 groups-text-dark">
        You're not in any group yet
      </h2>

      <p class="groups-text-muted mb-8 text-sm leading-relaxed">
        Create a new group to start collaborating, or enter a group code to join an existing group.
      </p>

      <!-- Error message -->
      <div
        v-if="errorMessage"
        class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm flex items-center gap-2"
      >
        <i class="pi pi-exclamation-triangle"></i>
        <span>{{ errorMessage }}</span>
      </div>

      <!-- Success message -->
      <div
        v-if="successMessage"
        class="mb-4 p-3 bg-green-50 border border-green-200 rounded-xl text-green-700 text-sm flex items-center gap-2"
      >
        <i class="pi pi-check-circle"></i>
        <span>{{ successMessage }}</span>
      </div>

      <!-- Create group button -->
      <button
        class="w-full groups-btn-primary text-white py-3 rounded-xl mb-4 flex items-center justify-center gap-2 transition-all duration-200 shadow-lg hover:shadow-xl font-medium transform hover:scale-[1.02] active:scale-[0.98] disabled:transform-none disabled:cursor-not-allowed"
        @click="createGroup"
        :disabled="isLoading"
      >
        <i v-if="!isLoading" class="pi pi-plus-circle text-lg"></i>
        <i v-else class="pi pi-spin pi-spinner text-lg"></i>
        {{ isLoading ? "Creating..." : "Create a new group" }}
      </button>

      <div class="flex items-center gap-4 mb-4">
        <div class="flex-1 border-t border-gray-200"></div>
        <span class="text-xs groups-text-muted font-medium">OR</span>
        <div class="flex-1 border-t border-gray-200"></div>
      </div>

      <!-- Input + Join button -->
      <div class="flex flex-col gap-3">
        <div class="relative">
          <i class="pi pi-key absolute left-3 top-1/2 transform -translate-y-1/2 groups-text-muted opacity-60"></i>
          <input
            v-model="groupCode"
            placeholder="Enter group code"
            class="groups-input rounded-xl px-10 py-3 w-full transition-all duration-200 disabled:cursor-not-allowed"
            :disabled="isLoading"
          />
        </div>

        <button
          class="groups-btn-accent text-white py-3 rounded-xl flex items-center justify-center gap-2 transition-all duration-200 shadow-lg hover:shadow-xl font-medium transform hover:scale-[1.02] active:scale-[0.98] disabled:transform-none disabled:cursor-not-allowed"
          @click="joinGroup"
          :disabled="isLoading || !groupCode"
        >
          <i v-if="!isLoading" class="pi pi-sign-in text-lg"></i>
          <i v-else class="pi pi-spin pi-spinner text-lg"></i>
          {{ isLoading ? "Joining..." : "Join group" }}
        </button>
      </div>
    </div>

    <!-- If group exists -->
    <div v-else class="w-full max-w-2xl">
      <div class="text-center mb-6">
        <div class="inline-flex items-center justify-center w-16 h-16 groups-icon-container rounded-full mb-4 shadow-lg">
          <i class="pi pi-users text-3xl text-white"></i>
        </div>
        <h1 class="text-3xl font-bold groups-text-dark mb-2">
          Your Group
        </h1>
        <p class="groups-text-muted text-sm">Manage your group and members</p>
      </div>

      <div class="groups-card shadow-2xl rounded-2xl p-8 transform transition-all duration-300 hover:shadow-3xl">
        <div class="flex items-center justify-between mb-6 pb-6 border-b groups-border">
          <div>
            <h2 class="text-2xl font-bold groups-text-dark mb-2">
              {{ groupsStore.group.name }}
            </h2>
            <p class="groups-text-muted text-sm flex items-center gap-2">
              <i class="pi pi-users text-xs"></i>
              <span>{{ groupsStore.group.userGroups?.length || 0 }} member{{ (groupsStore.group.userGroups?.length || 0) !== 1 ? 's' : '' }}</span>
            </p>
          </div>
          <div class="flex items-center justify-center w-16 h-16 groups-icon-bg rounded-full">
            <i class="pi pi-check-circle text-2xl groups-primary"></i>
          </div>
        </div>

        <!-- Error/Success messages -->
        <div
          v-if="errorMessage"
          class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm flex items-center gap-2"
        >
          <i class="pi pi-exclamation-triangle"></i>
          <span>{{ errorMessage }}</span>
        </div>

        <div
          v-if="successMessage"
          class="mb-4 p-3 bg-green-50 border border-green-200 rounded-xl text-green-700 text-sm flex items-center gap-2"
        >
          <i class="pi pi-check-circle"></i>
          <span>{{ successMessage }}</span>
        </div>

        <!-- Generate Token Section -->
        <div class="mb-6 p-4 bg-gradient-to-r from-blue-50 to-cyan-50 rounded-xl border border-blue-100">
          <div class="flex items-center justify-between mb-3">
            <div>
              <h3 class="font-semibold groups-text-dark mb-1">Invite Members</h3>
              <p class="text-xs groups-text-muted">Generate a code to share with others</p>
            </div>
            <button
              @click="generateToken"
              :disabled="isGeneratingToken"
              class="groups-btn-primary text-white px-4 py-2 rounded-lg flex items-center gap-2 text-sm font-medium transition-all duration-200 hover:shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i v-if="!isGeneratingToken" class="pi pi-key"></i>
              <i v-else class="pi pi-spin pi-spinner"></i>
              {{ isGeneratingToken ? "Generating..." : "Generate Code" }}
            </button>
          </div>
          
          <div v-if="groupToken" class="mt-3 p-3 bg-white rounded-lg border-2 border-dashed border-blue-200">
            <div class="flex items-center justify-between gap-3">
              <div class="flex-1">
                <p class="text-xs groups-text-muted mb-1">Group Invite Code</p>
                <p class="text-lg font-mono font-bold groups-text-dark break-all">{{ groupToken }}</p>
              </div>
              <button
                @click="copyToken"
                class="flex-shrink-0 p-2 groups-icon-bg rounded-lg hover:bg-blue-200 transition-colors"
                title="Copy code"
              >
                <i class="pi pi-copy groups-primary"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="space-y-4">
          <div class="flex items-center gap-3 p-4 groups-info-bg rounded-xl">
            <div class="flex items-center justify-center w-10 h-10 groups-icon-bg rounded-lg">
              <i class="pi pi-id-card groups-primary"></i>
            </div>
            <div class="flex-1">
              <p class="text-xs groups-text-muted mb-1">Group ID</p>
              <p class="text-sm font-semibold groups-text-dark">{{ groupsStore.group.id }}</p>
            </div>
          </div>

          <div class="flex items-center gap-3 p-4 groups-info-bg rounded-xl">
            <div class="flex items-center justify-center w-10 h-10 groups-icon-bg rounded-lg">
              <i class="pi pi-user groups-accent"></i>
            </div>
            <div class="flex-1">
              <p class="text-xs groups-text-muted mb-1">Created by User ID</p>
              <p class="text-sm font-semibold groups-text-dark">{{ groupsStore.group.createdByUserId }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, nextTick } from "vue";
import { useGroupsStore } from "../stores/groupsStore";
import { useEventStore } from "../stores/eventStore";

const eventStore = useEventStore();
const groupsStore = useGroupsStore();

// Group code input for join
const groupCode = ref("");
const isLoading = ref(false);
const errorMessage = ref("");
const successMessage = ref("");
const groupToken = ref("");
const isGeneratingToken = ref(false);

onMounted(async () => {
  await groupsStore.getUserGroups();
  await eventStore.getEventsForCurrentUser();
  
  // Redirect to events page if user has a group
  // Use nextTick to ensure reactive state is updated
  await nextTick();
  
  console.log("Checking for group:", groupsStore.group);

});

// Button logic
const createGroup = async () => {
  isLoading.value = true;
  errorMessage.value = "";
  successMessage.value = "";
  try {
    await groupsStore.createGroup();
    successMessage.value = "Group created successfully!";
    await groupsStore.getUserGroups();
    // Redirect will happen via watch
  } catch (error: any) {
    errorMessage.value = error?.message || error?.ValidationMessage || "Failed to create group";
  } finally {
    isLoading.value = false;
  }
};

const joinGroup = async () => {
  if (!groupCode.value.trim()) {
    errorMessage.value = "Please enter a group code";
    return;
  }
  isLoading.value = true;
  errorMessage.value = "";
  successMessage.value = "";
  try {
    await groupsStore.joinGroup(groupCode.value.trim());
    groupCode.value = "";
    successMessage.value = "Successfully joined the group!";
    await groupsStore.getUserGroups();
    // Redirect will happen via watch
  } catch (error: any) {
    errorMessage.value = error?.message || error?.ValidationMessage || "Failed to join group";
  } finally {
    isLoading.value = false;
  }
};

const generateToken = async () => {
  if (!groupsStore.group?.id) {
    errorMessage.value = "Group ID is missing";
    return;
  }
  isGeneratingToken.value = true;
  errorMessage.value = "";
  successMessage.value = "";
  try {
    const token = await groupsStore.generateGroupToken(groupsStore.group.id);
    if (token) {
      groupToken.value = token;
      successMessage.value = "Invite code generated successfully!";
    }
  } catch (error: any) {
    errorMessage.value = error?.message || error?.ValidationMessage || "Failed to generate invite code";
  } finally {
    isGeneratingToken.value = false;
  }
};

const copyToken = async () => {
  if (!groupToken.value) return;
  try {
    await navigator.clipboard.writeText(groupToken.value);
    successMessage.value = "Code copied to clipboard!";
    setTimeout(() => {
      successMessage.value = "";
    }, 2000);
  } catch (error) {
    errorMessage.value = "Failed to copy code";
  }
};
</script>

<style scoped>
/* Groups Page Styles using CSS Variables */
.groups-page-background {
  background: linear-gradient(to bottom right, var(--color-background), var(--color-card), var(--color-background));
}

.groups-icon-container {
  background: linear-gradient(to bottom right, var(--color-primary), var(--color-accent-blue));
}

.groups-text-dark {
  color: var(--color-text-dark);
}

.groups-text-muted {
  color: var(--color-text-muted);
}

.groups-btn-primary {
  background: linear-gradient(to right, var(--color-primary), var(--color-accent-blue));
}

.groups-btn-primary:hover:not(:disabled) {
  background: linear-gradient(to right, var(--color-accent-blue), var(--color-primary));
}

.groups-btn-primary:disabled {
  background: #9CA3AF;
}

.groups-btn-accent {
  background: linear-gradient(to right, var(--color-accent-blue), var(--color-highlight-cyan));
}

.groups-btn-accent:hover:not(:disabled) {
  background: linear-gradient(to right, var(--color-highlight-cyan), var(--color-accent-blue));
}

.groups-btn-accent:disabled {
  background: #9CA3AF;
}

.groups-input {
  border: 2px solid rgba(0, 0, 0, 0.1);
  background: var(--color-background);
  color: var(--color-text-dark);
}

.groups-input:focus {
  border-color: var(--color-primary);
  background: var(--color-card);
  outline: none;
  box-shadow: 0 0 0 3px rgba(45, 125, 210, 0.1);
}

.groups-input:disabled {
  background: var(--color-background);
  opacity: 0.6;
}

.groups-card {
  background: var(--color-card);
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.groups-border {
  border-color: rgba(0, 0, 0, 0.1);
}

.groups-info-bg {
  background: var(--color-background);
}

.groups-icon-bg {
  background: rgba(45, 125, 210, 0.1);
}

.groups-primary {
  color: var(--color-primary);
}

.groups-accent {
  color: var(--color-accent-blue);
}

/* Mobile first */
@media (max-width: 600px) {
  div {
    padding-left: 0.5rem;
    padding-right: 0.5rem;
  }
}
</style>
