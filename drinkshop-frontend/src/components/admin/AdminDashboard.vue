<template>
  <div class="dashboard">
    <!-- 統計卡片 -->
    <div class="dashboard-stats">
      <div class="stat-card" v-for="stat in stats" :key="stat.key">
        <div class="stat-icon" :class="stat.iconClass">
          <i :class="stat.icon"></i>
        </div>
        <div class="stat-info">
          <h3>{{ stat.value }}</h3>
          <p>{{ stat.label }}</p>
        </div>
      </div>
    </div>

    <!-- 最近訂單 -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">最近訂單</h3>
      </div>
      <div class="table-content">
        <div v-if="loading" class="loading">載入中...</div>
        <div v-else-if="recentOrders.length === 0" class="no-data">暫無訂單資料</div>
        <table v-else>
          <thead>
            <tr>
              <th>訂單編號</th>
              <th>用戶</th>
              <th>金額</th>
              <th>狀態</th>
              <th>訂單時間</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in recentOrders" :key="order.id">
              <td>#{{ order.id }}</td>
              <td>{{ order.username }}</td>
              <td>NT$ {{ order.totalAmount?.toLocaleString() || '0' }}</td>
              <td>
                <span class="status-badge" :class="getStatusClass(order.status)">
                  {{ getStatusText(order.status) }}
                </span>
              </td>
              <td>{{ formatDate(order.createdAt) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- 熱門產品 -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">熱門產品</h3>
      </div>
      <div class="table-content">
        <div v-if="loading" class="loading">載入中...</div>
        <div v-else-if="popularProducts.length === 0" class="no-data">暫無產品資料</div>
        <table v-else>
          <thead>
            <tr>
              <th>產品名稱</th>
              <th>價格</th>
              <th>庫存</th>
              <th>銷量</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in popularProducts" :key="product.id">
              <td>{{ product.name }}</td>
              <td>NT$ {{ product.price?.toLocaleString() || '0' }}</td>
              <td>{{ product.stock || 0 }}</td>
              <td>{{ product.salesCount || 0 }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../../api'

// 響應式數據
const loading = ref(true)
const recentOrders = ref([])
const popularProducts = ref([])
const dashboardStats = ref({
  totalProducts: 0,
  totalOrders: 0,
  totalUsers: 0,
  totalRevenue: 0
})

// 使用共享 axios 實例 api
const apiClient = api

// 計算統計數據
const stats = computed(() => [
  {
    key: 'products',
    icon: 'fas fa-box',
    iconClass: 'stat-icon-1',
    label: '總產品數',
    value: dashboardStats.value.totalProducts
  },
  {
    key: 'orders',
    icon: 'fas fa-shopping-cart',
    iconClass: 'stat-icon-2',
    label: '總訂單數',
    value: dashboardStats.value.totalOrders
  },
  {
    key: 'users',
    icon: 'fas fa-users',
    iconClass: 'stat-icon-3',
    label: '總用戶數',
    value: dashboardStats.value.totalUsers
  },
  {
    key: 'revenue',
    icon: 'fas fa-dollar-sign',
    iconClass: 'stat-icon-4',
    label: '總營收',
    value: `NT$ ${dashboardStats.value.totalRevenue?.toLocaleString() || '0'}`
  }
])

// 方法
const formatDate = (dateString) => {
  if (!dateString) return '未知'
  try {
    const date = new Date(dateString)
    return date.toLocaleString('zh-TW', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch (error) {
    return '無效日期'
  }
}

const getStatusClass = (status) => {
  const statusMap = {
    'PENDING': 'pending',
    'PROCESSING': 'processing',
    'COMPLETED': 'completed',
    'CANCELLED': 'cancelled'
  }
  return statusMap[status] || 'pending'
}

const getStatusText = (status) => {
  const statusMap = {
    'PENDING': '待處理',
    'PROCESSING': '處理中',
    'COMPLETED': '已完成',
    'CANCELLED': '已取消'
  }
  return statusMap[status] || status
}

// API 呼叫方法
const fetchApi = async (endpoint) => {
  try {
    const token = localStorage.getItem('token')
    const headers = token ? { Authorization: `Bearer ${token}` } : {}
    const response = await apiClient.get(endpoint, { headers })
    return response.data
  } catch (error) {
    console.error(`API call failed for ${endpoint}:`, error)
    throw error
  }
}

const loadDashboardData = async () => {
  try {
    loading.value = true
    
    // 載入統計數據
    try {
      const productsResponse = await fetchApi('/products')
      dashboardStats.value.totalProducts = productsResponse.data?.length || 0
    } catch (error) {
      console.error('Failed to load products:', error)
    }

    // 載入訂單統計
    try {
      const ordersResponse = await fetchApi('/orders')
      if (ordersResponse.data) {
        const orders = ordersResponse.data
        dashboardStats.value.totalOrders = orders.length
        dashboardStats.value.totalRevenue = orders.reduce((sum, order) => {
          return sum + (order.totalAmount || 0)
        }, 0)
        
        // 最近訂單（取前10筆）
        recentOrders.value = orders
          .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
          .slice(0, 10)
      }
    } catch (error) {
      console.error('Failed to load orders:', error)
    }

    // 載入用戶統計
    try {
      const usersResponse = await fetchApi('/users')
      dashboardStats.value.totalUsers = usersResponse.data?.length || 0
    } catch (error) {
      console.error('Failed to load users:', error)
    }

    // 載入熱門產品
    try {
      const productsResponse = await fetchApi('/products')
      if (productsResponse.data) {
        // 取前10個產品作為熱門產品
        popularProducts.value = productsResponse.data.slice(0, 10)
      }
    } catch (error) {
      console.error('Failed to load popular products:', error)
    }

  } catch (error) {
    console.error('Dashboard loading error:', error)
  } finally {
    loading.value = false
  }
}

// 生命週期
onMounted(() => {
  loadDashboardData()
})
</script>

<style scoped>
.dashboard {
  width: 100%;
}

/* 統計卡片 */
.dashboard-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  padding: 25px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  display: flex;
  align-items: center;
  gap: 15px;
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
}

.stat-icon-1 {
  background: linear-gradient(135deg, #667eea, #764ba2);
}

.stat-icon-2 {
  background: linear-gradient(135deg, #f093fb, #f5576c);
}

.stat-icon-3 {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
}

.stat-icon-4 {
  background: linear-gradient(135deg, #43e97b, #38f9d7);
}

.stat-info h3 {
  font-size: 2rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 5px;
}

.stat-info p {
  color: #64748b;
  font-size: 0.9rem;
}

/* 表格樣式 */
.data-table {
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  margin-bottom: 30px;
}

.table-header {
  padding: 20px 25px;
  border-bottom: 1px solid #e2e8f0;
}

.table-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
}

.table-content {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead th {
  background-color: #f8fafc;
  padding: 15px;
  text-align: left;
  font-weight: 600;
  color: #374151;
  border-bottom: 1px solid #e2e8f0;
}

tbody td {
  padding: 15px;
  border-bottom: 1px solid #f1f5f9;
}

tbody tr:hover {
  background-color: #f8fafc;
}

.loading, .no-data {
  text-align: center;
  padding: 40px;
  color: #64748b;
}

.loading {
  font-style: italic;
}

/* 狀態標籤 */
.status-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
  text-transform: uppercase;
}

.status-badge.pending {
  background-color: #fef3c7;
  color: #92400e;
}

.status-badge.processing {
  background-color: #dbeafe;
  color: #1e40af;
}

.status-badge.completed {
  background-color: #d1fae5;
  color: #065f46;
}

.status-badge.cancelled {
  background-color: #fee2e2;
  color: #991b1b;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .dashboard-stats {
    grid-template-columns: 1fr;
  }
}
</style>
