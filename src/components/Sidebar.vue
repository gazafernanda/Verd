<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/user'
import { X, Home, Sun, Leaf, Sprout, MessageCircle, User, LogOut } from 'lucide-vue-next'

const router = useRouter()
const user = useUserStore()
const emit = defineEmits(['close'])

function logout() {
  user.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <aside class="relative w-[260px] h-full bg-surface border-r border-border flex flex-col py-8">
    <!-- Mobile close button -->
    <button
      @click="emit('close')"
      class="lg:hidden absolute top-4 right-4 p-1.5 rounded-lg text-text-muted hover:bg-bg-app transition-colors"
    >
      <X width="20" height="20" />
    </button>

    <div class="px-8 mb-10">
      <div class="flex items-center gap-3">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="#1a5641" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/></svg>
        <span class="text-[1.25rem] font-bold text-primary">Verd</span>
      </div>
    </div>

    <nav>
      <ul class="flex flex-col gap-2 px-5">
        <li>
          <router-link
            to="/"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Home class="shrink-0" width="24" height="24" />
            Dashboard
          </router-link>
        </li>
        <li>
          <router-link
            to="/weather"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Sun class="shrink-0" width="24" height="24" />
            Weather
          </router-link>
        </li>
        <li>
          <router-link
            to="/plants"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Sprout class="shrink-0" width="24" height="24" />
            My Plants
          </router-link>
        </li>
        <li>
          <router-link
            to="/recommendation"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Leaf class="shrink-0" width="24" height="24" />
            Recommendation
          </router-link>
        </li>
        <li>
          <router-link
            to="/chat"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <MessageCircle class="shrink-0" width="24" height="24" />
            Chat AI
          </router-link>
        </li>
        <li>
          <router-link
            to="/profile"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <User class="shrink-0" width="24" height="24" />
            Profile
          </router-link>
        </li>
      </ul>
    </nav>

    <div class="flex-1"></div>

    <div class="px-5">
      <button @click="logout" class="w-full flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main">
        <LogOut class="shrink-0" width="24" height="24" />
        Logout
      </button>
    </div>
  </aside>
</template>
