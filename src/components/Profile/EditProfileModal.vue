<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../../stores/user'
import { UserRound, Camera, Trash2, X } from 'lucide-vue-next'

const emit = defineEmits<{ close: [] }>()
const { t } = useI18n()
const user = useUserStore()

const displayNameInput = ref(user.displayName)
const avatarInput = ref(user.avatarUrl)
const error = ref('')
const saving = ref(false)
const saved = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

// Resize/compress the chosen image to a small square JPEG data URL so it
// stays well within the API's size limit and localStorage budget.
function resizeImage(file: File, size = 256): Promise<string> {
  return new Promise((resolve, reject) => {
    const img = new Image()
    img.onload = () => {
      const canvas = document.createElement('canvas')
      canvas.width = size
      canvas.height = size
      const ctx = canvas.getContext('2d')
      if (!ctx) return reject(new Error('no canvas context'))
      // cover-fit: crop to a centered square
      const scale = Math.max(size / img.width, size / img.height)
      const w = img.width * scale
      const h = img.height * scale
      ctx.drawImage(img, (size - w) / 2, (size - h) / 2, w, h)
      resolve(canvas.toDataURL('image/jpeg', 0.85))
    }
    img.onerror = () => reject(new Error('bad image'))
    img.src = URL.createObjectURL(file)
  })
}

async function onFileChange(e: Event) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return
  error.value = ''
  if (file.size > 8 * 1024 * 1024) {
    error.value = t('profile.photoTooLarge')
    return
  }
  try {
    avatarInput.value = await resizeImage(file)
  } catch {
    error.value = t('profile.photoTooLarge')
  }
}

function removePhoto() {
  avatarInput.value = ''
  if (fileInput.value) fileInput.value.value = ''
}

async function save() {
  saving.value = true
  try {
    await user.saveSettings({
      displayName: displayNameInput.value.trim() || user.displayName,
      avatarUrl: avatarInput.value,
    })
    saved.value = true
    setTimeout(() => emit('close'), 900)
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <!-- Backdrop -->
  <div
    class="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4"
    @click.self="emit('close')"
  >
    <!-- Modal card -->
    <div class="bg-surface rounded-2xl shadow-xl w-full max-w-[440px] overflow-hidden">

      <!-- Header -->
      <div class="flex items-center justify-between px-6 pt-6 pb-4">
        <div class="flex items-center gap-2">
          <UserRound class="text-success-green" width="20" height="20" />
          <h2 class="text-[1.1rem] font-bold text-text-main">{{ t('profile.editProfileTitle') }}</h2>
        </div>
        <button
          @click="emit('close')"
          class="w-8 h-8 rounded-full bg-bg-app flex items-center justify-center text-text-muted hover:bg-[#e6e8eb] transition-colors"
        >
          <X width="16" height="16" />
        </button>
      </div>

      <div class="px-6 pb-6 flex flex-col gap-6">
        <!-- Avatar -->
        <div class="flex items-center gap-5">
          <div class="relative w-24 h-24 rounded-full border-4 border-border bg-bg-app flex items-center justify-center shrink-0 overflow-hidden">
            <img v-if="avatarInput" :src="avatarInput" :alt="displayNameInput" class="w-[88px] h-[88px] rounded-full object-cover" />
            <UserRound v-else class="text-text-light" width="48" height="48" stroke-width="1.5" />
          </div>
          <div class="flex flex-col gap-2">
            <div class="flex gap-2">
              <button
                @click="fileInput?.click()"
                class="inline-flex items-center gap-2 px-4 py-2 rounded-xl text-[0.85rem] font-semibold bg-bg-app text-text-main hover:bg-[#e6e8eb] transition-colors"
              >
                <Camera width="15" height="15" />
                {{ t('profile.changePhoto') }}
              </button>
              <button
                v-if="avatarInput"
                @click="removePhoto"
                class="inline-flex items-center justify-center w-9 h-9 rounded-xl text-red-500 bg-bg-app hover:bg-[#fce8e8] transition-colors"
                :aria-label="t('profile.removePhoto')"
              >
                <Trash2 width="15" height="15" />
              </button>
            </div>
            <p class="text-[0.75rem] text-text-muted m-0">{{ t('profile.photoHint') }}</p>
          </div>
          <input ref="fileInput" type="file" accept="image/*" class="hidden" @change="onFileChange" />
        </div>

        <p v-if="error" class="text-[0.8rem] text-red-500 -mt-3">{{ error }}</p>

        <!-- Display name -->
        <div class="flex flex-col gap-2">
          <label class="text-[0.85rem] font-extrabold text-text-main">{{ t('profile.displayName') }}</label>
          <input
            v-model="displayNameInput"
            type="text"
            class="px-4 py-3 rounded-md border border-border bg-bg-app font-[inherit] text-[0.95rem] text-text-main outline-none focus:border-success-green focus:bg-surface transition-colors"
          />
        </div>

        <!-- Actions -->
        <div class="flex justify-end gap-3 pt-1">
          <button
            @click="emit('close')"
            class="px-5 py-2.5 rounded-[24px] text-[0.9rem] font-bold border border-border text-text-muted hover:bg-bg-app transition-colors"
          >
            {{ t('common.cancel') }}
          </button>
          <button
            @click="save"
            :disabled="saving || saved"
            class="px-6 py-2.5 rounded-[24px] text-[0.9rem] font-bold text-white transition-colors disabled:opacity-90"
            :class="saved ? 'bg-primary' : 'bg-success-green hover:bg-[#2ea06e]'"
          >
            {{ saved ? t('common.saved') : t('common.saveChanges') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
