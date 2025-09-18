<template>
  <div class="admin-container">
    <!-- 遮罩：sidebar 展開時才顯示，點擊自動收合 -->
    <div v-if="sidebarOpen" class="sidebar-backdrop" @click="closeSidebar">
      <!-- debug: 遮罩顯示 -->
    </div>
    <!-- 側邊欄 -->
    <aside class="sidebar" :class="{ 'open': sidebarOpen }">
      <div class="sidebar-header">
        <h2><i class="fas fa-tint"></i> 清涼飲品</h2>
        <p>後台管理系統</p>
      </div>

      <nav class="sidebar-nav">
        <ul>
          <li v-for="tab in tabs" :key="tab.id">
            <a 
              href="#" 
              class="nav-link" 
              :class="{ 'active': activeTab === tab.id }"
              @click.prevent="switchTab(tab.id)"
            >
              <i :class="tab.icon"></i>
              <span>{{ tab.name }}</span>
            </a>
          </li>
        </ul>
      </nav>

      <div class="sidebar-footer">
        <div class="user-info">
          <span>{{ currentUser?.username || '管理員' }}</span>
          <button @click="logout" class="btn-logout">
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
          <h1>{{ currentTabName }}</h1>
        </div>
        <div class="nav-right">
          <span class="current-time">{{ currentTime }}</span>
        </div>
      </header>

      <!-- 內容區域 -->
      <div class="content">
        <!-- 動態組件區域 -->
        <component 
          :is="currentTabComponent" 
          :key="activeTab"
          @show-notification="showNotification"
        ></component>
      </div>
    </main>

    <!-- 通知元素 -->
    <div 
      v-for="(notification, index) in notifications" 
      :key="index" 
      class="notification" 
      :class="`notification-${notification.type}`"
    >
      <span>{{ notification.message }}</span>
      <button @click="removeNotification(index)" class="notification-close">&times;</button>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import AdminDashboard from './AdminDashboard.vue';
import AdminProducts from './AdminProducts.vue';
import AdminOrders from './AdminOrders.vue';
import AdminUsers from './AdminUsers.vue';

export default {
  name: 'AdminPanel',
  components: {
    AdminDashboard,
    AdminProducts,
    AdminOrders,
    AdminUsers
  },
  setup() {
    // 全局變量
    const currentUser = ref(null);
  // 使用共享 axios 實例 (由子組件引入 `src/api.js`)，不在此組件管理 apiBase
    
    // UI狀態
    const sidebarOpen = ref(false);
    const activeTab = ref('dashboard');
    const currentTime = ref(new Date().toLocaleString('zh-TW'));
    const notifications = ref([]);
    
    // 定義頁籤
    const tabs = [
      { id: 'dashboard', name: '儀表板', icon: 'fas fa-tachometer-alt', component: 'AdminDashboard' },
      { id: 'products', name: '產品管理', icon: 'fas fa-box', component: 'AdminProducts' },
      { id: 'orders', name: '訂單管理', icon: 'fas fa-shopping-cart', component: 'AdminOrders' },
      { id: 'users', name: '用戶管理', icon: 'fas fa-users', component: 'AdminUsers' }
    ];
    
    // 計算屬性
    const currentTabName = computed(() => {
      const tab = tabs.find(t => t.id === activeTab.value);
      return tab ? tab.name : '';
    });
    
    const currentTabComponent = computed(() => {
      const tab = tabs.find(t => t.id === activeTab.value);
      return tab ? tab.component : null;
    });
    
    // 方法

    function toggleSidebar() {
      console.log('toggleSidebar called');
      sidebarOpen.value = !sidebarOpen.value;
    }
    function closeSidebar() {
      console.log('closeSidebar called');
      sidebarOpen.value = false;
    }
    
    function switchTab(tabId) {
      activeTab.value = tabId;
    }
    
    function updateCurrentTime() {
      currentTime.value = new Date().toLocaleString('zh-TW');
    }
    
    function showNotification(message, type = 'info') {
      notifications.value.push({ message, type });
      setTimeout(() => {
        if (notifications.value.length > 0) {
          notifications.value.shift();
        }
      }, 5000);
    }
    
    function removeNotification(index) {
      notifications.value.splice(index, 1);
    }
    
    function checkAuth() {
      const userStr = localStorage.getItem('user');
      
      if (!userStr) {
        alert('請先登入才能訪問管理後台');
        window.location.href = '/';
        return false;
      }
      
      try {
        const user = JSON.parse(userStr);
        // 標準化鍵名
        const normalized = {
          id: user.id ?? user.Id ?? null,
          username: user.username ?? user.Username ?? '',
          role: (user.role ?? user.Role ?? '').toString().toLowerCase()
        };
        
        if (normalized.role === 'admin') {
          currentUser.value = normalized;
          return true;
        } else {
          alert(`需要管理員權限才能訪問此頁面。當前角色: ${normalized.role || 'undefined'}`);
          window.location.href = '/';
          return false;
        }
      } catch (error) {
        console.error('Parse user data error:', error);
        localStorage.removeItem('user');
        alert('登入資料異常，請重新登入');
        window.location.href = '/';
        return false;
      }
    }
    
    function logout() {
      if (confirm('確定要登出嗎？')) {
        localStorage.removeItem('user');
        showNotification('已成功登出', 'success');
        setTimeout(() => {
          window.location.href = '/';
        }, 1000);
      }
    }
    
    // 生命週期鉤子
    onMounted(() => {
      checkAuth();
      updateCurrentTime();
      const timeInterval = setInterval(updateCurrentTime, 1000);
      
      // 清理定時器
      onUnmounted(() => {
        clearInterval(timeInterval);
      });
    });
    
  return {
      currentUser,
      sidebarOpen,
      activeTab,
      currentTime,
      tabs,
      notifications,
      currentTabName,
      currentTabComponent,
  toggleSidebar,
  closeSidebar,
  // debug: 確認 return 物件
  // console.log('setup return', { toggleSidebar, closeSidebar });
      switchTab,
      showNotification,
      removeNotification,
      logout
    };
  }
};
</script>

<style scoped>
/* 確保遮罩覆蓋全畫面且在 sidebar 下方 */
.sidebar-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0,0,0,0.35);
  z-index: 1200;
  transition: opacity 0.2s;
}
@media (max-width: 900px) {
  .sidebar {
    z-index: 1300;
  }
}
/* 響應式側邊欄與遮罩 */
.sidebar-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0,0,0,0.35);
  z-index: 998;
  transition: opacity 0.2s;
}

@media (max-width: 900px) {
  .sidebar {
    position: fixed;
    top: 0;
    left: 0;
    height: 100vh;
    width: 75vw;
    max-width: 320px;
    min-width: 180px;
    z-index: 999;
    background: #fff;
    box-shadow: 2px 0 8px rgba(0,0,0,0.08);
    transform: translateX(-100%);
    transition: transform 0.3s;
  }
  .sidebar.open {
    transform: translateX(0);
  }
  .main-content {
    margin-left: 0 !important;
  }
}
/* 全局樣式引用通過 main.js 或在 public/index.html 中引入 admin-styles.css */
/* 此處可以添加組件特定的樣式 */
</style>
