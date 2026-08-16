<script setup lang="ts">
/**
 * A help icon that explains the element next to it.
 *
 * Opens on hover and focus on desktop, and on tap on touch devices — where hover
 * doesn't exist, so a hover-only tooltip would be unreachable. The bubble is
 * placed above the icon by default and flips below when there isn't room, so it
 * never lands on top of the thing it's describing.
 */
import { ref, computed, onUnmounted, useTemplateRef } from 'vue'
import { CircleHelp } from 'lucide-vue-next'

const props = withDefaults(
  defineProps<{
    /** Tooltip body. Keep to two sentences — this is a hint, not documentation. */
    label: string
    /** Preferred side. Flips automatically when it would overflow the viewport. */
    placement?: 'top' | 'bottom'
    size?: number
  }>(),
  { placement: 'top', size: 14 },
)

const open = ref(false)
const trigger = useTemplateRef<HTMLButtonElement>('trigger')

/** Resolved after measuring, so the bubble can flip away from a screen edge. */
const side = ref<'top' | 'bottom'>(props.placement)

/** Horizontal nudge that keeps a wide bubble inside the viewport. */
const shift = ref(0)

const MARGIN = 12
const BUBBLE_WIDTH = 240
const BUBBLE_ESTIMATED_HEIGHT = 88

function position() {
  const el = trigger.value
  if (!el) return

  const rect = el.getBoundingClientRect()

  // Flip when the preferred side doesn't have room for the bubble.
  const roomAbove = rect.top
  const roomBelow = window.innerHeight - rect.bottom
  side.value =
    props.placement === 'top'
      ? roomAbove >= BUBBLE_ESTIMATED_HEIGHT + MARGIN || roomAbove >= roomBelow
        ? 'top'
        : 'bottom'
      : roomBelow >= BUBBLE_ESTIMATED_HEIGHT + MARGIN || roomBelow >= roomAbove
        ? 'bottom'
        : 'top'

  // Centre on the icon, then pull back inside whichever edge it would cross.
  const centre = rect.left + rect.width / 2
  const half = BUBBLE_WIDTH / 2
  let offset = 0
  if (centre - half < MARGIN) offset = MARGIN - (centre - half)
  else if (centre + half > window.innerWidth - MARGIN)
    offset = window.innerWidth - MARGIN - (centre + half)
  shift.value = offset
}

function show() {
  position()
  open.value = true
}

function hide() {
  open.value = false
}

/**
 * Tap toggles on touch devices. Bound to click rather than a touch event so it
 * also covers keyboard activation and stylus input.
 */
function toggle(event: Event) {
  event.stopPropagation()
  if (open.value) {
    hide()
    return
  }
  show()
  // Dismiss on the next tap anywhere, the way a native tooltip would.
  document.addEventListener('click', hide, { once: true })
}

const bubbleClasses = computed(() =>
  side.value === 'top' ? 'bottom-full mb-2' : 'top-full mt-2',
)

onUnmounted(() => document.removeEventListener('click', hide))
</script>

<template>
  <span class="relative inline-flex align-middle">
    <button
      ref="trigger"
      type="button"
      :aria-label="label"
      :aria-expanded="open"
      class="inline-flex items-center justify-center rounded-full text-text-light hover:text-success-green focus:text-success-green focus:outline-none focus-visible:ring-2 focus-visible:ring-success-green/40 transition-colors"
      @mouseenter="show"
      @mouseleave="hide"
      @focus="show"
      @blur="hide"
      @click="toggle"
    >
      <CircleHelp :width="size" :height="size" />
    </button>

    <Transition name="tip">
      <span
        v-if="open"
        role="tooltip"
        class="absolute left-1/2 z-50 w-[240px] rounded-xl bg-text-main px-3 py-2.5 text-[0.78rem] font-medium leading-relaxed text-white shadow-lg pointer-events-none"
        :class="bubbleClasses"
        :style="{ transform: `translateX(calc(-50% + ${shift}px))` }"
      >
        {{ label }}
        <!-- Arrow, nudged back the same amount as the bubble so it stays on the icon -->
        <span
          class="absolute left-1/2 h-2 w-2 rotate-45 bg-text-main"
          :class="side === 'top' ? '-bottom-1' : '-top-1'"
          :style="{ transform: `translateX(calc(-50% - ${shift}px)) rotate(45deg)` }"
        ></span>
      </span>
    </Transition>
  </span>
</template>

<style scoped>
.tip-enter-active,
.tip-leave-active {
  transition:
    opacity 0.12s ease,
    transform 0.12s ease;
}
.tip-enter-from,
.tip-leave-to {
  opacity: 0;
}
</style>
