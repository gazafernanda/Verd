# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Development
npm run dev          # Start Vite dev server

# Build
npm run build        # Type-check + build for production
npm run build-only   # Build without type-check
npm run preview      # Preview production build

# Type checking
npm run type-check   # Run vue-tsc

# Linting & formatting
npm run lint         # Run oxlint + eslint (both with --fix)
npm run format       # Format src/ with Prettier
```

Node version requirement: `^20.19.0 || >=22.12.0`

## Architecture

**Verd** is a Vue 3 + TypeScript SPA for plant care management — weather integration, care recommendations, AI chat, and user profiles.

### Stack

- **Vue 3** with Composition API (`<script setup>`)
- **Vue Router 5** — all routes lazy-loaded via dynamic imports
- **Pinia** — stores use composition API style (`ref`, `computed`, functions)
- **Tailwind CSS 3** — utility-first with custom design tokens defined in `tailwind.config.js`
- **Vite 7** — `@` alias maps to `./src`

### Routes (`src/router/index.ts`)

| Path | View |
|---|---|
| `/` | DashboardView |
| `/weather` | WeatherView |
| `/recommendation` | RecommendationView |
| `/chat` | ChatView |
| `/profile` | ProfileView |

### Directory Structure

```
src/
  router/          # Vue Router config
  stores/          # Pinia stores (composition API style)
  views/           # Page-level components (one per route)
  components/
    Chat/          # Chat message & input components
    Weather/       # Weather display components
    Profile/       # Profile-related components
    Recommendation/ # Care recommendation components
    *.vue          # Layout & shared card components (Sidebar, etc.)
  assets/          # Images + main.css (CSS variables/tokens)
  App.vue          # Root: sidebar layout + RouterView
  main.ts          # Entry point
```

### Styling Conventions

- Tailwind utility classes are primary; custom tokens defined in `tailwind.config.js` (colors: `bg-app`, `surface`, `primary`, `accent-green`; border radius: `sm`/`md`/`lg`/`xl`/`2xl`; custom shadows)
- CSS variables in `src/assets/main.css` for design tokens
- Font: Plus Jakarta Sans (loaded from Google Fonts)
- Transitions: `0.2s ease` standard
- Responsive: `max-lg:` breakpoints for sidebar/layout

### Linting

Two linters run in sequence:
1. **oxlint** — fast Rust-powered linter
2. **ESLint** — Vue essential rules + TypeScript + Prettier skip-formatting

ESLint uses flat config format (`eslint.config.ts`).
