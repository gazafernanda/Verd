<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/user'
import { TriangleAlert, Eye, EyeOff, RefreshCw, CheckCircle } from 'lucide-vue-next'

const router = useRouter()
const user = useUserStore()

const displayName = ref('')
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
    await user.register(displayName.value, email.value, password.value)
    user.logout()
    router.push({ name: 'login' })
  } catch (e: unknown) {
    const msg = e instanceof Error ? e.message : 'Something went wrong.'
    error.value = msg
    isServerError.value = msg.includes('server') || msg.includes('reach')
  } finally {
    loading.value = false
  }
}

function continueDemo() {
  user.loginDemo(displayName.value || 'Alex Rivera')
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
          <CheckCircle width="32" height="32" color="#ffffff" />
        </div>
        <h1 class="text-4xl font-extrabold text-white leading-tight mb-4">
          Start growing<br>with confidence.
        </h1>
        <p class="text-lg leading-relaxed max-w-sm" style="color: rgba(255,255,255,0.7)">
          Join thousands of plant lovers who use Verd to keep their gardens thriving year-round.
        </p>

        <!-- Steps -->
        <div class="mt-10 flex flex-col gap-5">
          <div v-for="(step, i) in ['Create your account', 'Add your plants', 'Get personalized care']" :key="step"
               class="flex items-center gap-4">
            <div class="w-7 h-7 rounded-full border-2 flex items-center justify-center shrink-0 text-xs font-bold"
                 style="border-color: rgba(255,255,255,0.3); color: rgba(255,255,255,0.7)">
              {{ i + 1 }}
            </div>
            <span class="text-sm font-medium" style="color: rgba(255,255,255,0.85)">{{ step }}</span>
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

        <h2 class="text-[2rem] font-extrabold text-text-main mb-2 tracking-tight">Create your account</h2>
        <p class="text-text-muted mb-8">Start your plant care journey today</p>

        <!-- Error -->
        <div v-if="error" class="mb-6 rounded-xl overflow-hidden border text-sm font-medium"
          :class="isServerError ? 'border-amber-200 bg-amber-50' : 'border-red-200 bg-red-50'">
          <div class="px-4 py-3 flex items-start gap-2"
            :class="isServerError ? 'text-amber-700' : 'text-red-600'">
            <TriangleAlert width="16" height="16" class="shrink-0 mt-px" />
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
          <!-- Name -->
          <div class="flex flex-col gap-2">
            <label class="text-sm font-bold text-text-main">Display name</label>
            <input
              v-model="displayName"
              type="text"
              placeholder="Your name"
              required
              autocomplete="name"
              class="px-4 py-3.5 rounded-xl border border-border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-accent-green focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
            />
          </div>

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
            <label class="text-sm font-bold text-text-main">Password <span class="text-text-light font-normal">(min. 8 characters)</span></label>
            <div class="relative">
              <input
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="••••••••"
                required
                minlength="8"
                autocomplete="new-password"
                class="w-full px-4 py-3.5 pr-12 rounded-xl border border-border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-accent-green focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute right-4 top-1/2 -translate-y-1/2 text-text-light hover:text-text-muted transition-colors"
              >
                <Eye v-if="!showPassword" width="18" height="18" />
                <EyeOff v-else width="18" height="18" />
              </button>
            </div>

            <!-- Password strength indicator -->
            <div v-if="password.length > 0" class="flex gap-1.5 mt-1">
              <div v-for="i in 4" :key="i" class="h-1 flex-1 rounded-full transition-all duration-300"
                :class="password.length >= i * 2 ? (password.length >= 8 ? 'bg-success-green' : 'bg-[#f59e0b]') : 'bg-border'"></div>
            </div>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            :disabled="loading"
            class="mt-2 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px disabled:opacity-60 disabled:cursor-not-allowed disabled:translate-y-0 flex items-center justify-center gap-2"
          >
            <RefreshCw v-if="loading" class="animate-spin" width="18" height="18" />
            {{ loading ? 'Creating account…' : 'Create account' }}
          </button>
        </form>

        <p class="mt-8 text-center text-sm text-text-muted">
          Already have an account?
          <router-link to="/login" class="font-bold text-accent-green hover:text-accent-green-hover transition-colors">
            Sign in
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>
