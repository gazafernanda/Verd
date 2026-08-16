import { createRouter, createWebHashHistory } from 'vue-router'

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    // Auth routes (no sidebar layout)
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
      meta: { hideLayout: true, guestOnly: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
      meta: { hideLayout: true, guestOnly: true },
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: () => import('../views/ForgotPasswordView.vue'),
      meta: { hideLayout: true },
    },
    {
      // Reached from an emailed link, which may well be opened in a browser
      // where nobody is signed in — so neither guest-only nor auth-only.
      path: '/reset-password',
      name: 'reset-password',
      component: () => import('../views/ResetPasswordView.vue'),
      meta: { hideLayout: true },
    },
    {
      // Serves both the emailed link and the "check your inbox" screen shown
      // straight after registering, so it must stay reachable while unverified.
      path: '/verify-email',
      name: 'verify-email',
      component: () => import('../views/VerifyEmailView.vue'),
      meta: { hideLayout: true },
    },

    // App routes (require auth, and a verified email address)
    {
      path: '/',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/weather',
      name: 'weather',
      component: () => import('../views/WeatherView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/recommendation',
      name: 'recommendation',
      component: () => import('../views/RecommendationView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/notifications',
      name: 'notifications',
      component: () => import('../views/NotificationsView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      // Left open to unverified accounts so they can still see who they're
      // signed in as and sign out.
      path: '/profile',
      name: 'profile',
      component: () => import('../views/ProfileView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/plants',
      name: 'plants',
      component: () => import('../views/PlantsView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/plants/new',
      name: 'add-plant',
      component: () => import('../views/AddPlantView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/plants/history',
      name: 'plant-history',
      component: () => import('../views/PlantHistoryView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/plants/history/:id',
      name: 'plant-history-detail',
      component: () => import('../views/PlantHistoryDetailView.vue'),
      meta: { requiresAuth: true, requiresVerified: true },
    },
    {
      path: '/admin',
      name: 'admin',
      component: () => import('../views/AdminView.vue'),
      meta: { requiresAuth: true, requiresVerified: true, requiresAdmin: true },
    },
  ],
})

router.beforeEach((to) => {
  const isAuthenticated = !!localStorage.getItem('verd_token')

  if (to.meta.requiresAuth && !isAuthenticated) {
    return { name: 'login' }
  }
  if (to.meta.guestOnly && isAuthenticated) {
    return { name: 'dashboard' }
  }

  // Convenience only — the API enforces verification on every core request.
  // Defaults to verified so a cache miss doesn't strand an established user.
  if (
    to.meta.requiresVerified &&
    isAuthenticated &&
    localStorage.getItem('verd_verified') === 'false'
  ) {
    return { name: 'verify-email' }
  }

  // Convenience only — the API enforces the role on every admin request.
  if (to.meta.requiresAdmin && localStorage.getItem('verd_role') !== 'Admin') {
    return { name: 'dashboard' }
  }
})

export default router
