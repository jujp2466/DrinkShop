<template>
  <div class="dashboard">
    <!-- 統計卡片 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon products">
          <i class="fas fa-box"></i>
        </div>
        <div class="stat-info">
          <h3>{{ totalProducts }}</h3>
          <p>總產品數</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon orders">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <div class="stat-info">
          <h3>{{ totalOrders }}</h3>
          <p>總訂單數</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon users">
          <i class="fas fa-users"></i>
        </div>
        <div class="stat-info">
          <h3>{{ totalUsers }}</h3>
          <p>註冊用戶</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon revenue">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="stat-info">
          <h3>NT$ {{ totalRevenue.toLocaleString() }}</h3>
          <p>總營收</p>
        </div>
      </div>
    </div>

    <!-- 快捷操作 -->
    <div class="dashboard-grid">
      <div class="dashboard-card">
        <h2>快捷操作</h2>
        <div class="quick-actions">
          <router-link to="/admin/products" class="action-btn products">
            <i class="fas fa-plus"></i>
            新增產品
          </router-link>
          
          <router-link to="/admin/orders" class="action-btn orders">
            <i class="fas fa-list"></i>
            查看訂單
          </router-link>
          
          <router-link to="/admin/users" class="action-btn users">
            <i class="fas fa-user-plus"></i>
            管理用戶
          </router-link>
          
          <button @click="refreshData" class="action-btn refresh">
            <i class="fas fa-refresh"></i>
            刷新數據
          </button>
        </div>
      </div>

      <!-- 最近訂單 -->
      <div class="dashboard-card">
        <h2>最近訂單</h2>
        <div v-if="recentOrders.length === 0" class="no-data">
          <i class="fas fa-inbox"></i>
          <p>暫無訂單數據</p>
        </div>
        <div v-else class="recent-orders">
          <div v-for="order in recentOrders" :key="order.id" class="order-item">
            <div class="order-info">
              <span class="order-id">#{{ order.id.slice(-6) }}</span>
              <span class="order-customer">{{ order.customerName }}</span>
            </div>
            <div class="order-details">
              <span class="order-amount">NT$ {{ order.totalAmount.toLocaleString() }}</span>
              <span class="order-status" :class="order.status">
                {{ getStatusText(order.status) }}
              </span>
            </div>
          </div>
          
          <router-link to="/admin/orders" class="view-all-btn">
            查看所有訂單 <i class="fas fa-arrow-right"></i>
          </router-link>
        </div>
      </div>
    </div>

    <!-- 系統狀態 -->
    <div class="dashboard-grid">
      <div class="dashboard-card">
        <h2>系統狀態</h2>
        <div class="system-status">
          <div class="status-item">
            <div class="status-indicator online"></div>
            <span>API 服務</span>
            <span class="status-text online">正常運行</span>
          </div>
          
          <div class="status-item">
            <div class="status-indicator online"></div>
            <span>數據庫</span>
            <span class="status-text online">連接正常</span>
          </div>
          
          <div class="status-item">
            <div class="status-indicator online"></div>
            <span>前端服務</span>
            <span class="status-text online">運行中</span>
          </div>
        </div>
      </div>

      <!-- 熱門產品 -->
      <div class="dashboard-card">
        <h2>熱門產品</h2>
        <div v-if="popularProducts.length === 0" class="no-data">
          <i class="fas fa-chart-line"></i>
          <p>暫無銷售數據</p>
        </div>
        <div v-else class="popular-products">
          <div v-for="product in popularProducts" :key="product.id" class="product-item">
            <div class="product-image">
              <img :src="imageSrc(product)" :alt="product.name">
            </div>
            <div class="product-info">
              <h4>{{ product.name }}</h4>
              <p class="product-sales">銷售 {{ product.sales || 0 }} 件</p>
            </div>
            <div class="product-price">
              NT$ {{ product.price }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useProductStore } from '@/stores/product'

const productStore = useProductStore()

// 統計數據
const totalProducts = ref(0)
const totalOrders = ref(0)
const totalUsers = ref(0)
const totalRevenue = ref(0)

// 最近訂單
const recentOrders = ref([])

// 熱門產品
const popularProducts = ref([])

// 狀態文字映射
const getStatusText = (status) => {
  const statusMap = {
    'pending': '待處理',
    'processing': '處理中',
    'shipped': '已出貨',
    'delivered': '已送達',
    'cancelled': '已取消'
  }
  return statusMap[status] || '未知'
}

// 刷新數據
const refreshData = async () => {
  try {
    // 獲取產品數據
    await productStore.fetchProducts()
    totalProducts.value = productStore.products.length
    
    // 模擬其他數據
    totalOrders.value = Math.floor(Math.random() * 500) + 100
    totalUsers.value = Math.floor(Math.random() * 200) + 50
    totalRevenue.value = Math.floor(Math.random() * 1000000) + 500000
    
    // 模擬最近訂單
    recentOrders.value = generateMockOrders()
    
    // 設置熱門產品（從現有產品中選取）
    popularProducts.value = productStore.products.slice(0, 5).map(product => ({
      ...product,
      sales: Math.floor(Math.random() * 100) + 10
    }))
    
  } catch (error) {
    console.error('刷新數據失敗:', error)
  }
}

// 生成模擬訂單數據
const generateMockOrders = () => {
  const orders = []
  const statuses = ['pending', 'processing', 'shipped', 'delivered']
  const names = ['王小明', '李美玲', '張志偉', '陳雅芬', '林俊德']
  
  for (let i = 0; i < 5; i++) {
    orders.push({
      id: `order-${Date.now()}-${i}`,
      customerName: names[Math.floor(Math.random() * names.length)],
      totalAmount: Math.floor(Math.random() * 2000) + 500,
      status: statuses[Math.floor(Math.random() * statuses.length)],
      createdAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000)
    })
  }
  
  return orders.sort((a, b) => b.createdAt - a.createdAt)
}

onMounted(() => {
  refreshData()
})

// 統一圖片來源處理：支援 imageUrl / image / 相對路徑與預設圖；茶類優先用茶圖
const imageSrc = (item) => {
  const candidate = item?.imageUrl || item?.image || ''
  const name = (item?.name || '').toLowerCase()
  const category = (item?.category || '').toLowerCase()
  const isTea = /茶|tea/.test(name) || /茶|tea/.test(category)
  if (!candidate) return isTea
    ? 'https://images.unsplash.com/photo-1488900128323-21503983a07e'
    : 'https://images.unsplash.com/photo-1504674900247-0877df9cc836'
  if (candidate.startsWith('http://') || candidate.startsWith('https://')) return candidate
  if (candidate.startsWith('/')) return candidate
  return candidate
}
</script>

<style scoped>
.dashboard {
  max-width: 1400px;
  margin: 0 auto;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  padding: 1.5rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  gap: 1rem;
  transition: transform 0.2s, box-shadow 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
}

.stat-icon.products {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.stat-icon.orders {
  background: linear-gradient(135deg, #10b981, #059669);
}

.stat-icon.users {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
}

.stat-icon.revenue {
  background: linear-gradient(135deg, #f59e0b, #d97706);
}

.stat-info h3 {
  font-size: 1.75rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0 0 0.25rem 0;
}

.stat-info p {
  color: #6b7280;
  margin: 0;
  font-size: 0.875rem;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.dashboard-card {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.dashboard-card h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 1.5rem 0;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid #f3f4f6;
}

.quick-actions {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 1rem;
}

.action-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1.5rem 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  text-decoration: none;
  color: #374151;
  background: #f9fafb;
  transition: all 0.2s;
  cursor: pointer;
  font-weight: 500;
}

.action-btn:hover {
  border-color: #2563eb;
  background: #eff6ff;
  color: #2563eb;
  transform: translateY(-2px);
}

.action-btn.products:hover {
  border-color: #3b82f6;
  background: #eff6ff;
  color: #3b82f6;
}

.action-btn.orders:hover {
  border-color: #10b981;
  background: #f0fdf4;
  color: #10b981;
}

.action-btn.users:hover {
  border-color: #8b5cf6;
  background: #faf5ff;
  color: #8b5cf6;
}

.action-btn.refresh:hover {
  border-color: #f59e0b;
  background: #fffbeb;
  color: #f59e0b;
}

.action-btn i {
  font-size: 1.5rem;
}

.no-data {
  text-align: center;
  padding: 2rem;
  color: #9ca3af;
}

.no-data i {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  display: block;
}

.recent-orders {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border: 1px solid #f3f4f6;
  border-radius: 0.5rem;
  transition: background-color 0.2s;
}

.order-item:hover {
  background-color: #f9fafb;
}

.order-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.order-id {
  font-weight: 600;
  color: #2563eb;
  font-size: 0.875rem;
}

.order-customer {
  color: #6b7280;
  font-size: 0.875rem;
}

.order-details {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
}

.order-amount {
  font-weight: 600;
  color: #1f2937;
}

.order-status {
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-weight: 500;
}

.order-status.pending {
  background: #fef3c7;
  color: #92400e;
}

.order-status.processing {
  background: #dbeafe;
  color: #1e40af;
}

.order-status.shipped {
  background: #d1fae5;
  color: #065f46;
}

.order-status.delivered {
  background: #dcfce7;
  color: #166534;
}

.order-status.cancelled {
  background: #fee2e2;
  color: #991b1b;
}

.view-all-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background: #f3f4f6;
  border-radius: 0.5rem;
  text-decoration: none;
  color: #374151;
  font-weight: 500;
  transition: all 0.2s;
  margin-top: 1rem;
}

.view-all-btn:hover {
  background: #e5e7eb;
  color: #1f2937;
}

.system-status {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.status-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.5rem;
}

.status-indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
}

.status-indicator.online {
  background: #10b981;
  box-shadow: 0 0 10px rgba(16, 185, 129, 0.3);
}

.status-indicator.offline {
  background: #ef4444;
  box-shadow: 0 0 10px rgba(239, 68, 68, 0.3);
}

.status-text.online {
  color: #10b981;
  font-weight: 500;
  margin-left: auto;
}

.status-text.offline {
  color: #ef4444;
  font-weight: 500;
  margin-left: auto;
}

.popular-products {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.product-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border: 1px solid #f3f4f6;
  border-radius: 0.5rem;
  transition: background-color 0.2s;
}

.product-item:hover {
  background-color: #f9fafb;
}

.product-image {
  width: 50px;
  height: 50px;
  flex-shrink: 0;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 0.375rem;
}

.product-info {
  flex: 1;
}

.product-info h4 {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 0.25rem 0;
}

.product-sales {
  font-size: 0.75rem;
  color: #6b7280;
  margin: 0;
}

.product-price {
  font-weight: 600;
  color: #2563eb;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1rem;
  }

  .dashboard-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .dashboard-card {
    padding: 1.5rem;
  }

  .quick-actions {
    grid-template-columns: repeat(2, 1fr);
  }

  .action-btn {
    padding: 1rem;
  }
}

@media (max-width: 480px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }

  .stat-card {
    padding: 1rem;
  }

  .stat-icon {
    width: 50px;
    height: 50px;
  }

  .stat-info h3 {
    font-size: 1.5rem;
  }

  .dashboard-card {
    padding: 1rem;
  }

  .quick-actions {
    grid-template-columns: 1fr;
  }

  .order-item, .product-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .order-details, .product-price {
    align-self: stretch;
    text-align: right;
  }
}
</style>
