import { createRouter, createWebHistory } from 'vue-router'
import FormBuilder from '../views/FormBuilder.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'form-builder',
      component: FormBuilder,
    },
    {
      path: '/preview',
      name: 'preview',
      component: () => import('../views/PreviewView.vue'),
    },
    {
      path: '/scheduler',
      name: 'scheduler',
      component: () => import('../views/SchedulerView.vue'),
    },
  ],
})

export default router
