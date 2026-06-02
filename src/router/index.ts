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

    // App routes (require auth)
    {
      path: '/',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/weather',
      name: 'weather',
      component: () => import('../views/WeatherView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/recommendation',
      name: 'recommendation',
      component: () => import('../views/RecommendationView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/chat',
      name: 'chat',
      component: () => import('../views/ChatView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../views/ProfileView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/plants',
      name: 'plants',
      component: () => import('../views/PlantsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/plants/new',
      name: 'add-plant',
      component: () => import('../views/AddPlantView.vue'),
      meta: { requiresAuth: true },
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
})

export default router
