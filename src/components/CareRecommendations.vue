<script setup lang="ts">
import CareCard from "./CareCard.vue";
import { usePlantsStore } from "../stores/plants";

const plants = usePlantsStore();
</script>

<template>
  <div class="flex flex-col">
    <div class="flex justify-between items-center mb-6">
      <div class="flex items-center gap-3">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24"
          height="24"
          viewBox="0 0 256 256"
        >
          <path
            fill="#1a5641"
            d="M216,40a16,16,0,0,0-16-16H56A16,16,0,0,0,40,40V216a16,16,0,0,0,16,16H200a16,16,0,0,0,16-16ZM56,40H200V216H56ZM96,160a32,32,0,1,1,32-32A32,32,0,0,1,96,160Zm0-48a16,16,0,1,0,16,16A16,16,0,0,0,96,112Z"
          />
        </svg>
        <h2 class="text-[1.4rem] font-bold text-text-main m-0">
          Care Recommendations
        </h2>
      </div>
      <a
        href="#"
        class="text-[0.95rem] font-bold text-primary transition-colors duration-200 hover:underline hover:text-primary-hover"
        >View Schedule</a
      >
    </div>

    <div class="w-full overflow-x-auto pb-4 -mb-4 scrollbar-none">
      <div class="flex gap-6 w-max pr-6">
        <div
          v-for="plant in plants.plants"
          :key="plant.id"
          class="w-[300px] shrink-0 self-stretch"
        >
          <CareCard
            :category="plant.careCard.category"
            :title="plant.careCard.title"
            :description="plant.careCard.description"
            :image="plant.careCard.image"
            :bgType="plant.careCard.bgType"
          />
        </div>
      </div>
    </div>
  </div>
</template>
