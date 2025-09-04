<template>
  <div id="app" class="app-layout">
    <!-- 頁頭 -->
    <header class="header">
      <div class="logo">清涼飲品</div>
      <p>您的最佳飲料選擇</p>
    </header>

    <!-- 導航欄 -->
    <nav class="nav">
      <ul>
        <li><router-link to="/">首頁</router-link></li>
        <li><router-link to="/products">產品</router-link></li>
        <li><router-link to="/about">關於我們</router-link></li>
        <li><router-link to="/contact">聯絡我們</router-link></li>
        <li v-if="authStore.isAdmin">
          <router-link to="/admin">後台管理</router-link>
        </li>
      </ul>
      
      <!-- 用戶操作區 -->
      <div class="user-actions">
        <div v-if="authStore.isAuthenticated" class="user-info">
          <span>歡迎，{{ authStore.currentUser?.userName }}</span>
          <button @click="handleLogout" class="btn btn-secondary">登出</button>
        </div>
        <div v-else>
          <button @click="openLoginModal" class="btn btn-secondary">登入</button>
          <button @click="openRegisterModal" class="btn">註冊</button>
        </div>
        
        <!-- 購物車圖標 -->
        <div class="cart-icon" @click="openCartModal">
          🛒 <span v-if="cartStore.totalItems > 0" class="cart-count">{{ cartStore.totalItems }}</span>
        </div>
      </div>
    </nav>

    <!-- 主要內容區 -->
    <main class="main-content">
      <slot />
    </main>

    <!-- 購物車側邊欄 -->
    <CartSidebar />

    <!-- 登入/註冊模態框 -->
    <AuthModals ref="authModalsRef" />
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import CartSidebar from '@/components/CartSidebar.vue'
import AuthModals from '@/components/AuthModals.vue'

const authStore = useAuthStore()
const cartStore = useCartStore()

// AuthModals 組件引用
const authModalsRef = ref(null)

onMounted(() => {
  // 初始化認證狀態
  authStore.initAuth()
  // 初始化購物車
  cartStore.initCart()
})

watch(() => authStore.currentUser, (val) => {
})

const handleLogout = () => {
  authStore.logout()
}

const openLoginModal = () => {
  if (authModalsRef.value) {
    authModalsRef.value.openLogin()
  } else {
    console.error('authModalsRef 未初始化')
  }
}

const openRegisterModal = () => {
  if (authModalsRef.value) {
    authModalsRef.value.openRegister()
  }
}

const openCartModal = () => {
  cartStore.toggleCart()
}
</script>

<style scoped>
/* 這裡的樣式已經在全域 style.css 中定義 */
.main-content {
  flex: 1;
  min-height: calc(100vh - 160px);
}

/* 額外的局部樣式 */
@media (max-width: 768px) {
  .main-content {
    min-height: calc(100vh - 200px);
  }
}
</style>
