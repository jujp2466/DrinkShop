import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// 主要頁面
import HomePage from '@/views/HomePage.vue'
import ProductsPage from '@/views/ProductsPage.vue'
import AboutPage from '@/views/AboutPage.vue'
import ContactPage from '@/views/ContactPage.vue'
import CheckoutPage from '@/views/CheckoutPage.vue'

// 管理後台頁面
import AdminLayout from '@/layouts/AdminLayout.vue'
import AdminDashboard from '@/views/admin/AdminDashboard.vue'
import AdminProducts from '@/views/admin/AdminProducts.vue'
import AdminOrders from '@/views/admin/AdminOrders.vue'
import AdminUsers from '@/views/admin/AdminUsers.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: HomePage
  },
  {
    path: '/products',
    name: 'Products',
    component: ProductsPage
  },
  {
    path: '/about',
    name: 'About',
    component: AboutPage
  },
  {
    path: '/contact',
    name: 'Contact',
    component: ContactPage
  },
  {
    path: '/checkout',
    name: 'Checkout',
    component: CheckoutPage,
    meta: { requiresAuth: true }
  },
  // 管理後台路由
  {
    path: '/admin',
    component: AdminLayout,
    meta: { requiresAuth: true, requiresAdmin: true },
    children: [
      {
        path: '',
        name: 'AdminDashboard',
        component: AdminDashboard
      },
      {
        path: 'products',
        name: 'AdminProducts',
        component: AdminProducts
      },
      {
        path: 'orders',
        name: 'AdminOrders',
        component: AdminOrders
      },
      {
        path: 'users',
        name: 'AdminUsers',
        component: AdminUsers
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守衛
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  // 確保認證狀態已初始化
  if (!authStore.isAuthenticated) {
    authStore.initAuth()
  }
  
  // 檢查是否需要登入
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    alert('請先登入後再訪問此頁面')
    next('/')
    return
  }
  
  // 檢查是否需要管理員權限
  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    alert('您沒有權限訪問此頁面')
    next('/')
    return
  }
  
  next()
})

export default router
