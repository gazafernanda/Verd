<script setup lang="ts">
import { computed } from 'vue'
import { useWeatherStore } from '../../stores/weather'
import { formatTempValue } from '../../utils/temperature'

const weather = useWeatherStore()

// SVG view box. Width/height are arbitrary "design units" — the SVG scales.
const W = 800
const H = 220
const PAD_X = 16
const PAD_TOP = 36
const PAD_BOTTOM = 36

const chart = computed(() => {
  const series = weather.hourlyTemps
  if (series.length < 2) return null

  const temps = series.map((s) => s.temp)
  const min = Math.min(...temps)
  const max = Math.max(...temps)
  const range = Math.max(1, max - min)

  const innerW = W - PAD_X * 2
  const innerH = H - PAD_TOP - PAD_BOTTOM
  const stepX = innerW / (series.length - 1)

  const points = series.map((s, i) => {
    const x = PAD_X + i * stepX
    // Invert y because SVG's y axis grows downward.
    const y = PAD_TOP + innerH - ((s.temp - min) / range) * innerH
    return { x, y, hour: s.hour, temp: s.temp }
  })

  const linePath = points.map((p, i) => `${i === 0 ? 'M' : 'L'} ${p.x.toFixed(1)} ${p.y.toFixed(1)}`).join(' ')
  const areaPath =
    `M ${points[0]!.x.toFixed(1)} ${(PAD_TOP + innerH).toFixed(1)} ` +
    points.map((p) => `L ${p.x.toFixed(1)} ${p.y.toFixed(1)}`).join(' ') +
    ` L ${points[points.length - 1]!.x.toFixed(1)} ${(PAD_TOP + innerH).toFixed(1)} Z`

  // Six evenly spaced x-axis ticks (matches the original placeholder cadence).
  const tickCount = 6
  const ticks = Array.from({ length: tickCount + 1 }, (_, i) => {
    const idx = Math.round((i / tickCount) * (points.length - 1))
    return { x: points[idx]!.x, label: formatHour(points[idx]!.hour) }
  })

  return { points, linePath, areaPath, ticks, min, max }
})

function formatHour(h: number) {
  if (h === 0) return '12 AM'
  if (h === 12) return '12 PM'
  return h < 12 ? `${h} AM` : `${h - 12} PM`
}
</script>

<template>
  <div v-if="chart" class="w-full">
    <svg :viewBox="`0 0 ${W} ${H}`" class="w-full h-60" preserveAspectRatio="none">
      <defs>
        <linearGradient id="tempFill" x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%" stop-color="#37b27e" stop-opacity="0.25" />
          <stop offset="100%" stop-color="#37b27e" stop-opacity="0" />
        </linearGradient>
      </defs>

      <!-- horizontal grid lines -->
      <line v-for="i in 3" :key="`g${i}`"
        :x1="PAD_X" :x2="W - PAD_X"
        :y1="PAD_TOP + ((H - PAD_TOP - PAD_BOTTOM) * (i - 1)) / 2"
        :y2="PAD_TOP + ((H - PAD_TOP - PAD_BOTTOM) * (i - 1)) / 2"
        stroke="#e2e8e4" stroke-width="1" />

      <!-- area fill -->
      <path :d="chart.areaPath" fill="url(#tempFill)" />

      <!-- line -->
      <path :d="chart.linePath" fill="none" stroke="#37b27e" stroke-width="2.5"
        stroke-linecap="round" stroke-linejoin="round"
        vector-effect="non-scaling-stroke" />

      <!-- per-hour data dots + temperature labels -->
      <g v-for="(p, i) in chart.points" :key="`p${i}`">
        <circle :cx="p.x" :cy="p.y" r="2.5" fill="#37b27e" />
        <text
          :x="p.x" :y="p.y - 10"
          text-anchor="middle"
          font-size="11"
          font-weight="700"
          fill="#1a5641"
        >{{ formatTempValue(p.temp) }}°</text>
      </g>

      <!-- "now" emphasis -->
      <circle :cx="chart.points[0]!.x" :cy="chart.points[0]!.y" r="5" fill="#37b27e" />
      <circle :cx="chart.points[0]!.x" :cy="chart.points[0]!.y" r="9" fill="#37b27e" fill-opacity="0.18" />

      <!-- x-axis tick labels -->
      <text v-for="(tick, i) in chart.ticks" :key="i"
        :x="tick.x" :y="H - 10"
        :text-anchor="i === 0 ? 'start' : i === chart.ticks.length - 1 ? 'end' : 'middle'"
        font-size="11" fill="#94a3a0" font-weight="600">
        {{ tick.label }}
      </text>
    </svg>
  </div>

  <!-- Skeleton while hourly data hasn't arrived yet. -->
  <div v-else class="w-full h-60 flex items-center justify-center text-text-light text-[0.85rem]">
    …
  </div>
</template>
