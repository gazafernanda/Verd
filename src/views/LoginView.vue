<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/user'

const router = useRouter()
const user = useUserStore()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)
const error = ref('')

const isServerError = ref(false)

async function submit() {
  error.value = ''
  isServerError.value = false
  loading.value = true
  try {
    await user.login(email.value, password.value)
    router.push({ name: 'dashboard' })
  } catch (e: unknown) {
    const msg = e instanceof Error ? e.message : 'Something went wrong.'
    error.value = msg
    isServerError.value = msg.includes('server') || msg.includes('reach')
  } finally {
    loading.value = false
  }
}

function continueDemo() {
  user.loginDemo()
  router.push({ name: 'dashboard' })
}
</script>

<template>
  <div class="min-h-screen flex">
    <!-- Left panel -->
    <div class="hidden lg:flex lg:w-[45%] bg-primary flex-col justify-between p-12 relative overflow-hidden">
      <!-- Background pattern -->
      <div class="absolute inset-0 overflow-hidden pointer-events-none">
        <div class="absolute -top-24 -left-24 w-96 h-96 rounded-full bg-white opacity-[0.03]"></div>
        <div class="absolute top-1/3 -right-32 w-[500px] h-[500px] rounded-full bg-white opacity-[0.04]"></div>
        <div class="absolute -bottom-20 left-1/4 w-72 h-72 rounded-full bg-accent-green opacity-[0.15]"></div>
      </div>

      <!-- Logo -->
      <div class="flex items-center gap-3 relative z-10">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256">
          <path fill="#ffffff" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/>
        </svg>
        <span class="text-2xl font-bold text-white">Verd</span>
      </div>

      <!-- Center content -->
      <div class="relative z-10">
        <div class="w-16 h-16 bg-white bg-opacity-10 rounded-2xl flex items-center justify-center mb-8">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256">
            <path fill="#ffffff" d="M216,40H185.34a71.84,71.84,0,0,0-57.34,28.75A71.84,71.84,0,0,0,70.66,40H40a8,8,0,0,0-8,8v42.66A72.08,72.08,0,0,0,104,162v14H48a8,8,0,0,0-8,8v40a8,8,0,0,0,8,8H208a8,8,0,0,0,8-8V184a8,8,0,0,0-8-8H152V162a72.08,72.08,0,0,0,72-71.34V48A8,8,0,0,0,216,40Z"/>
          </svg>
        </div>
        <h1 class="text-4xl font-extrabold text-white leading-tight mb-4">
          Your garden,<br>smarter.
        </h1>
        <p class="text-white text-opacity-70 text-lg leading-relaxed max-w-sm" style="color: rgba(255,255,255,0.7)">
          AI-powered plant care with real-time weather insights. Know exactly when to water, prune, and feed every plant.
        </p>

        <!-- Feature bullets -->
        <div class="mt-10 flex flex-col gap-4">
          <div v-for="item in ['Hyper-local weather analysis', 'Personalized care schedules', 'AI plant diagnosis & chat']" :key="item"
               class="flex items-center gap-3">
            <div class="w-5 h-5 rounded-full bg-accent-green flex items-center justify-center shrink-0">
              <svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 256 256">
                <path fill="white" d="M173.66,98.34a8,8,0,0,1,0,11.32l-56,56a8,8,0,0,1-11.32,0l-24-24a8,8,0,0,1,11.32-11.32L112,148.69l50.34-50.35A8,8,0,0,1,173.66,98.34Z"/>
              </svg>
            </div>
            <span class="text-sm font-medium" style="color: rgba(255,255,255,0.85)">{{ item }}</span>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <p class="text-xs relative z-10" style="color: rgba(255,255,255,0.4)">© 2025 Verd. All rights reserved.</p>
    </div>

    <!-- Right panel -->
    <div class="flex-1 flex items-center justify-center px-8 py-12 bg-bg-app">
      <div class="w-full max-w-[420px]">
        <!-- Mobile logo -->
        <div class="flex items-center gap-2 mb-10 lg:hidden">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256">
            <path fill="#1a5641" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/>
          </svg>
          <span class="text-xl font-bold text-primary">Verd</span>
        </div>

        <h2 class="text-[2rem] font-extrabold text-text-main mb-2 tracking-tight">Welcome back</h2>
        <p class="text-text-muted mb-8">Sign in to your garden dashboard</p>

        <!-- Error -->
        <div v-if="error" class="mb-6 rounded-xl overflow-hidden border text-sm font-medium"
          :class="isServerError ? 'border-amber-200 bg-amber-50' : 'border-red-200 bg-red-50'">
          <div class="px-4 py-3 flex items-start gap-2"
            :class="isServerError ? 'text-amber-700' : 'text-red-600'">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256" class="shrink-0 mt-px"><path fill="currentColor" d="M236.8,188.09,149.35,36.22a24.76,24.76,0,0,0-42.7,0L19.2,188.09a23.51,23.51,0,0,0,0,23.72A24.35,24.35,0,0,0,40.55,224h174.9a24.35,24.35,0,0,0,21.33-12.19A23.51,23.51,0,0,0,236.8,188.09ZM120,104a8,8,0,0,1,16,0v40a8,8,0,0,1-16,0Zm8,88a12,12,0,1,1,12-12A12,12,0,0,1,128,192Z"/></svg>
            {{ error }}
          </div>
          <div v-if="isServerError" class="px-4 pb-3">
            <button @click="continueDemo"
              class="text-amber-700 font-bold underline underline-offset-2 hover:text-amber-800 transition-colors">
              Continue in demo mode instead →
            </button>
          </div>
        </div>

        <form @submit.prevent="submit" class="flex flex-col gap-5">
          <!-- Email -->
          <div class="flex flex-col gap-2">
            <label class="text-sm font-bold text-text-main">Email address</label>
            <input
              v-model="email"
              type="email"
              placeholder="you@example.com"
              required
              autocomplete="email"
              class="px-4 py-3.5 rounded-xl border border-border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-accent-green focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
            />
          </div>

          <!-- Password -->
          <div class="flex flex-col gap-2">
            <div class="flex justify-between items-center">
              <label class="text-sm font-bold text-text-main">Password</label>
            </div>
            <div class="relative">
              <input
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="••••••••"
                required
                autocomplete="current-password"
                class="w-full px-4 py-3.5 pr-12 rounded-xl border border-border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-accent-green focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute right-4 top-1/2 -translate-y-1/2 text-text-light hover:text-text-muted transition-colors"
              >
                <svg v-if="!showPassword" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 256 256"><path fill="currentColor" d="M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z"/></svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 256 256"><path fill="currentColor" d="M53.92,34.62A8,8,0,1,0,42.08,45.38L61.32,66.55C34,88.84,15.46,120.59,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208a127.11,127.11,0,0,0,53.47-11.72l22.61,24.1A8,8,0,1,0,215.92,209.38ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.16,133.16,0,0,1,25,128c7.21-13.46,38.62-64,103-64a112,112,0,0,1,36.35,6.11Z"/></svg>
              </button>
            </div>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            :disabled="loading"
            class="mt-2 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px disabled:opacity-60 disabled:cursor-not-allowed disabled:translate-y-0 flex items-center justify-center gap-2"
          >
            <svg v-if="loading" class="animate-spin" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 256 256"><path fill="currentColor" d="M232,128a104,104,0,0,1-208,0c0-41,23.81-78.36,60.12-96.06a8,8,0,0,1,7,14.44C60.49,60.61,40,93.07,40,128a88,88,0,0,0,176,0c0-34.93-20.49-67.39-51.12-81.62a8,8,0,0,1,7-14.44C208.19,49.64,232,87,232,128Z"/></svg>
            {{ loading ? 'Signing in…' : 'Sign in' }}
          </button>
        </form>

        <p class="mt-8 text-center text-sm text-text-muted">
          Don't have an account?
          <router-link to="/register" class="font-bold text-accent-green hover:text-accent-green-hover transition-colors">
            Create one
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>
