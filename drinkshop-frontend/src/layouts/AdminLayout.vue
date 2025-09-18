<template>
  <div class="admin-container">
    <!-- 遮罩：行動裝置展開側邊欄時顯示，點擊可關閉 -->
    <div v-if="sidebarOpen" class="sidebar-backdrop" @click="closeSidebar"></div>
    <!-- 側邊欄 -->
    <aside class="sidebar" :class="{ collapsed: sidebarCollapsed, open: sidebarOpen }">
      <div class="sidebar-header">
        <h2><i class="fas fa-tint"></i> 清涼飲品</h2>
        <p>後台管理系統</p>
      </div>
      
      <nav class="sidebar-nav">
        <ul>
          <li>
            <router-link 
              to="/admin" 
              class="nav-link" 
              :class="{ active: isDashboardActive }"
            >
              <i class="fas fa-tachometer-alt"></i>
              <span>儀表板</span>
            </router-link>
          </li>
          <li>
            <router-link 
              to="/admin/products" 
              class="nav-link" 
              :class="{ active: isProductsActive }"
            >
              <i class="fas fa-box"></i>
              <span>產品管理</span>
            </router-link>
          </li>
          <li>
            <router-link 
              to="/admin/orders" 
              class="nav-link" 
              :class="{ active: isOrdersActive }"
            >
              <i class="fas fa-shopping-cart"></i>
              <span>訂單管理</span>
            </router-link>
          </li>
          <li>
            <router-link 
              to="/admin/users" 
              class="nav-link" 
              :class="{ active: isUsersActive }"
            >
              <i class="fas fa-users"></i>
              <span>用戶管理</span>
            </router-link>
          </li>
        </ul>
      </nav>
      
      <div class="sidebar-footer">
        <div class="user-info">
          <span>{{ authStore.currentUser?.username || '管理員' }}</span>
          <button @click="handleLogout" class="btn-logout">
            <i class="fas fa-sign-out-alt"></i> 登出
          </button>
        </div>
      </div>
    </aside>

    <!-- 主要內容區 -->
    <main class="main-content">
      <!-- 頂部導航 -->
      <header class="top-nav">
        <div class="nav-left">
          <button @click="toggleSidebar" class="sidebar-toggle">
            <i class="fas fa-bars"></i>
          </button>
          <h1>{{ pageTitle }}</h1>
        </div>
        <div class="nav-right">
          <span class="current-time">{{ currentTime }}</span>
          <router-link to="/" class="btn-home">
            <i class="fas fa-home"></i>
            前台首頁
          </router-link>
        </div>
      </header>

      <!-- 內容區域 -->
      <div class="content">
        <router-view />
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const sidebarCollapsed = ref(false)
const sidebarOpen = ref(false)
const currentTime = ref('')

// 頁面標題映射
const pageTitles = {
  'AdminDashboard': '儀表板',
  'AdminProducts': '產品管理',
  'AdminOrders': '訂單管理',
  'AdminUsers': '用戶管理'
}

const pageTitle = computed(() => {
  return pageTitles[route.name] || '管理後台'
})

// 精確控制導航高亮狀態
const isDashboardActive = computed(() => {
  return route.path === '/admin'
})

const isProductsActive = computed(() => {
  return route.path === '/admin/products'
})

const isOrdersActive = computed(() => {
  return route.path === '/admin/orders'
})

const isUsersActive = computed(() => {
  return route.path === '/admin/users'
})

// 切換側邊欄（桌機：收合寬度；行動：抽屜開關）
const toggleSidebar = () => {
  if (window.innerWidth <= 1024) {
    sidebarOpen.value = !sidebarOpen.value
  } else {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }
}

const closeSidebar = () => {
  sidebarOpen.value = false
}

// 登出
const handleLogout = () => {
  if (confirm('確定要登出嗎？')) {
    authStore.logout()
    router.push('/')
  }
}

// 更新時間
const updateTime = () => {
  const now = new Date()
  currentTime.value = now.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

let timeInterval = null
const handleResize = () => {
  if (window.innerWidth > 1024) {
    // 返回桌機視窗時關閉行動抽屜
    sidebarOpen.value = false
  }
}

onMounted(() => {
  updateTime()
  timeInterval = setInterval(updateTime, 1000)
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval)
  }
  window.removeEventListener('resize', handleResize)
})

// 路由切換時，自動關閉行動抽屜
watch(() => route.fullPath, () => {
  if (window.innerWidth <= 1024) {
    sidebarOpen.value = false
  }
})
</script>

<style scoped>
.admin-container {
  display: flex;
  height: 100vh;
  background-color: #f8fafc;
}

.sidebar-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.35);
  z-index: 998;
}

.sidebar {
  width: 280px;
  background: linear-gradient(180deg, #1e3a8a 0%, #1e40af 100%);
  color: white;
  display: flex;
  flex-direction: column;
  transition: width 0.3s ease;
  position: relative;
  z-index: 100;
}

.sidebar.collapsed {
  width: 70px;
}

.sidebar-header {
  padding: 2rem 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  text-align: center;
}

.sidebar-header h2 {
  margin: 0 0 0.5rem 0;
  font-size: 1.5rem;
  font-weight: 700;
}

.sidebar-header p {
  margin: 0;
  font-size: 0.875rem;
  opacity: 0.8;
}

.sidebar.collapsed .sidebar-header h2 {
  font-size: 1rem;
}

.sidebar.collapsed .sidebar-header p {
  display: none;
}

.sidebar-nav {
  flex: 1;
  padding: 1rem 0;
}

.sidebar-nav ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.sidebar-nav li {
  margin-bottom: 0.5rem;
}

.nav-link {
  display: flex;
  align-items: center;
  padding: 1rem 1.5rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  transition: all 0.2s;
  position: relative;
  border-right: 3px solid transparent;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
}

/* 只有我們手動控制的 active class 會被應用 */
.nav-link.active {
  background-color: rgba(255, 255, 255, 0.15) !important;
  color: white !important;
  border-right-color: #fbbf24 !important;
}

.nav-link i {
  margin-right: 1rem;
  font-size: 1.125rem;
  width: 20px;
  text-align: center;
}

.sidebar.collapsed .nav-link span {
  display: none;
}

.sidebar.collapsed .nav-link {
  justify-content: center;
  padding: 1rem;
}

.sidebar.collapsed .nav-link i {
  margin-right: 0;
}

.sidebar-footer {
  padding: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
}

.user-info span {
  font-weight: 600;
  text-align: center;
}

.btn-logout {
  background: rgba(239, 68, 68, 0.2);
  border: 1px solid rgba(239, 68, 68, 0.3);
  color: #fecaca;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.875rem;
  width: 100%;
}

.btn-logout:hover {
  background: rgba(239, 68, 68, 0.3);
  border-color: rgba(239, 68, 68, 0.5);
}

.sidebar.collapsed .user-info span {
  display: none;
}

.sidebar.collapsed .btn-logout {
  padding: 0.5rem;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 手機/窄螢幕：側邊欄改為抽屜模式 */
@media (max-width: 1024px) {
  .sidebar {
    position: fixed;
    top: 0;
    left: 0;
    height: 100vh;
  width: 50vw;
  max-width: 200px;
  min-width: 120px;
    z-index: 999;
    transform: translateX(-100%);
    transition: transform 0.3s ease;
  }
  .sidebar.open {
    transform: translateX(0);
  }
  .main-content {
    margin-left: 0 !important;
  }
}

.top-nav {
  background: white;
  border-bottom: 1px solid #e5e7eb;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.nav-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.sidebar-toggle {
  background: none;
  border: none;
  font-size: 1.25rem;
  color: #6b7280;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 0.375rem;
  transition: all 0.2s;
}

.sidebar-toggle:hover {
  background-color: #f3f4f6;
  color: #374151;
}

.nav-left h1 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
}

.nav-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.current-time {
  font-size: 0.875rem;
  color: #6b7280;
  font-family: 'Courier New', monospace;
}

.btn-home {
  background: #10b981;
  color: white;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.btn-home:hover {
  background: #059669;
  transform: translateY(-1px);
}

.content {
  flex: 1;
  padding: 2rem;
  overflow-y: auto;
  background-color: #f8fafc;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .sidebar {
    position: fixed;
    left: 0;
    top: 0;
    height: 100vh;
    transform: translateX(-100%);
    transition: transform 0.3s ease;
  }

  .sidebar.collapsed {
    width: 280px;
    transform: translateX(0);
  }

  .main-content {
    width: 100%;
    margin-left: 0;
  }

  .top-nav {
    padding: 1rem;
  }

  .nav-left h1 {
    font-size: 1.25rem;
  }

  .nav-right {
    gap: 0.5rem;
  }

  .current-time {
    display: none;
  }

  .content {
    padding: 1rem;
  }
}

@media (max-width: 480px) {
  .btn-home span {
    display: none;
  }
}
</style>
