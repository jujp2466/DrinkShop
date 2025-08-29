<template>
  <div class="orders-admin">
    <!-- 頁面標題 -->
    <div class="page-header">
      <h2>訂單管理</h2>
    </div>

    <!-- 篩選區域 -->
    <div class="filters">
      <div class="search-box">
        <i class="fas fa-search"></i>
        <input 
          type="text" 
          placeholder="搜索訂單..." 
          v-model="searchQuery"
          @input="filterOrders"
        />
      </div>
      <div class="filter-buttons">
        <button 
          class="btn" 
          :class="{ 'btn-primary': activeFilter === 'all', 'btn-outline': activeFilter !== 'all' }"
          @click="setFilter('all')"
        >
          全部 ({{ orders.length }})
        </button>
        <button 
          class="btn" 
          :class="{ 'btn-primary': activeFilter === 'PENDING', 'btn-outline': activeFilter !== 'PENDING' }"
          @click="setFilter('PENDING')"
        >
          待處理 ({{ pendingCount }})
        </button>
        <button 
          class="btn" 
          :class="{ 'btn-primary': activeFilter === 'PROCESSING', 'btn-outline': activeFilter !== 'PROCESSING' }"
          @click="setFilter('PROCESSING')"
        >
          處理中 ({{ processingCount }})
        </button>
        <button 
          class="btn" 
          :class="{ 'btn-primary': activeFilter === 'COMPLETED', 'btn-outline': activeFilter !== 'COMPLETED' }"
          @click="setFilter('COMPLETED')"
        >
          已完成 ({{ completedCount }})
        </button>
      </div>
    </div>

    <!-- 訂單列表 -->
    <div class="data-table">
      <div v-if="loading" class="loading">載入中...</div>
      <div v-else-if="filteredOrders.length === 0" class="no-data">
        {{ searchQuery ? '沒有找到符合條件的訂單' : '暫無訂單資料' }}
      </div>
      <table v-else>
        <thead>
          <tr>
            <th>訂單編號</th>
            <th>用戶</th>
            <th>金額</th>
            <th>狀態</th>
            <th>付款方式</th>
            <th>訂單時間</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in paginatedOrders" :key="order.id">
            <td>
              <span class="order-id">#{{ order.id }}</span>
            </td>
            <td>{{ order.username || order.user?.username || '未知用戶' }}</td>
            <td class="order-amount">NT$ {{ order.totalAmount?.toLocaleString() || '0' }}</td>
            <td>
              <span class="status-badge" :class="getStatusClass(order.status)">
                {{ getStatusText(order.status) }}
              </span>
            </td>
            <td>{{ order.paymentMethod || '未知' }}</td>
            <td>{{ formatDate(order.createdAt) }}</td>
            <td class="actions">
              <button 
                class="btn-action btn-view" 
                @click="viewOrderDetail(order)"
                title="查看詳情"
              >
                <i class="fas fa-eye"></i>
              </button>
              <button 
                class="btn-action btn-status" 
                @click="updateOrderStatus(order)"
                title="更新狀態"
                v-if="order.status !== 'COMPLETED' && order.status !== 'CANCELLED'"
              >
                <i class="fas fa-check"></i>
              </button>
              <button 
                class="btn-action btn-cancel" 
                @click="cancelOrder(order)"
                title="取消訂單"
                v-if="order.status === 'PENDING'"
              >
                <i class="fas fa-times"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 分頁 -->
    <div class="pagination" v-if="totalPages > 1">
      <button 
        class="btn btn-outline" 
        @click="prevPage" 
        :disabled="currentPage === 1"
      >
        上一頁
      </button>
      <span class="page-info">
        第 {{ currentPage }} 頁，共 {{ totalPages }} 頁 ({{ filteredOrders.length }} 項)
      </span>
      <button 
        class="btn btn-outline" 
        @click="nextPage" 
        :disabled="currentPage === totalPages"
      >
        下一頁
      </button>
    </div>

    <!-- 訂單詳情 Modal -->
    <div class="modal" v-if="showDetailModal" @click.self="closeDetailModal">
      <div class="modal-content large">
        <div class="modal-header">
          <h3>訂單詳情 - #{{ currentOrder.id }}</h3>
          <button class="modal-close" @click="closeDetailModal">&times;</button>
        </div>
        
        <div class="modal-body">
          <!-- 訂單基本資訊 -->
          <div class="order-info">
            <div class="info-section">
              <h4>訂單資訊</h4>
              <div class="info-grid">
                <div class="info-item">
                  <label>訂單編號：</label>
                  <span>#{{ currentOrder.id }}</span>
                </div>
                <div class="info-item">
                  <label>用戶：</label>
                  <span>{{ currentOrder.username || currentOrder.user?.username || '未知用戶' }}</span>
                </div>
                <div class="info-item">
                  <label>狀態：</label>
                  <span class="status-badge" :class="getStatusClass(currentOrder.status)">
                    {{ getStatusText(currentOrder.status) }}
                  </span>
                </div>
                <div class="info-item">
                  <label>總金額：</label>
                  <span class="order-total">NT$ {{ currentOrder.totalAmount?.toLocaleString() || '0' }}</span>
                </div>
                <div class="info-item">
                  <label>付款方式：</label>
                  <span>{{ currentOrder.paymentMethod || '未知' }}</span>
                </div>
                <div class="info-item">
                  <label>訂單時間：</label>
                  <span>{{ formatDate(currentOrder.createdAt) }}</span>
                </div>
              </div>
            </div>

            <!-- 訂單商品 -->
            <div class="info-section" v-if="currentOrder.items && currentOrder.items.length > 0">
              <h4>訂購商品</h4>
              <div class="order-items">
                <div v-for="item in currentOrder.items" :key="item.id" class="order-item">
                  <div class="item-info">
                    <span class="item-name">{{ item.productName || item.product?.name }}</span>
                    <span class="item-quantity">數量: {{ item.quantity }}</span>
                  </div>
                  <div class="item-price">
                    <span>NT$ {{ (item.price || 0).toLocaleString() }}</span>
                    <span class="item-subtotal">小計: NT$ {{ ((item.price || 0) * (item.quantity || 1)).toLocaleString() }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- 備註 -->
            <div class="info-section" v-if="currentOrder.notes">
              <h4>備註</h4>
              <p class="order-notes">{{ currentOrder.notes }}</p>
            </div>
          </div>
        </div>

        <div class="modal-actions">
          <button class="btn btn-outline" @click="closeDetailModal">關閉</button>
          <button 
            class="btn btn-primary" 
            @click="updateOrderStatus(currentOrder)"
            v-if="currentOrder.status !== 'COMPLETED' && currentOrder.status !== 'CANCELLED'"
          >
            更新狀態
          </button>
        </div>
      </div>
    </div>

    <!-- 狀態更新 Modal -->
    <div class="modal" v-if="showStatusModal" @click.self="closeStatusModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>更新訂單狀態</h3>
          <button class="modal-close" @click="closeStatusModal">&times;</button>
        </div>
        
        <div class="modal-body">
          <div class="form-group">
            <label>選擇新狀態</label>
            <select v-model="newStatus" class="status-select">
              <option value="PENDING">待處理</option>
              <option value="PROCESSING">處理中</option>
              <option value="COMPLETED">已完成</option>
              <option value="CANCELLED">已取消</option>
            </select>
          </div>
        </div>

        <div class="modal-actions">
          <button class="btn btn-outline" @click="closeStatusModal">取消</button>
          <button class="btn btn-primary" @click="saveOrderStatus" :disabled="saving">
            {{ saving ? '更新中...' : '確認更新' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

// 響應式數據
const loading = ref(true)
const saving = ref(false)
const orders = ref([])
const filteredOrders = ref([])
const searchQuery = ref('')
const activeFilter = ref('all')
const currentPage = ref(1)
const itemsPerPage = 10

const showDetailModal = ref(false)
const showStatusModal = ref(false)
const currentOrder = ref({})
const newStatus = ref('')

// API 基礎 URL
const apiBase = window.location.hostname === 'localhost' ? 'http://localhost:5249/api/v1' : '/api/v1'

// 計算屬性
const pendingCount = computed(() => 
  orders.value.filter(o => o.status === 'PENDING').length
)

const processingCount = computed(() => 
  orders.value.filter(o => o.status === 'PROCESSING').length
)

const completedCount = computed(() => 
  orders.value.filter(o => o.status === 'COMPLETED').length
)

const totalPages = computed(() => 
  Math.ceil(filteredOrders.value.length / itemsPerPage)
)

const paginatedOrders = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredOrders.value.slice(start, end)
})

// API 呼叫方法
const fetchApi = async (endpoint, options = {}) => {
  try {
    const token = localStorage.getItem('token')
    const response = await fetch(`${apiBase}${endpoint}`, {
      headers: {
        'Authorization': token ? `Bearer ${token}` : '',
        'Content-Type': 'application/json',
        ...options.headers
      },
      ...options
    })
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    return await response.json()
  } catch (error) {
    console.error(`API call failed for ${endpoint}:`, error)
    throw error
  }
}

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

const loadOrders = async () => {
  try {
    loading.value = true
    const response = await fetchApi('/orders')
    orders.value = response.data || []
    filterOrders()
  } catch (error) {
    console.error('Failed to load orders:', error)
    orders.value = []
  } finally {
    loading.value = false
  }
}

const filterOrders = () => {
  let filtered = [...orders.value]
  
  // 搜索篩選
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(order => 
      order.id.toString().includes(query) ||
      order.username?.toLowerCase().includes(query) ||
      order.user?.username?.toLowerCase().includes(query)
    )
  }
  
  // 狀態篩選
  if (activeFilter.value !== 'all') {
    filtered = filtered.filter(order => order.status === activeFilter.value)
  }
  
  // 按創建時間排序（最新的在前）
  filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
  
  filteredOrders.value = filtered
  currentPage.value = 1
}

const setFilter = (filter) => {
  activeFilter.value = filter
  filterOrders()
}

const prevPage = () => {
  if (currentPage.value > 1) currentPage.value--
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) currentPage.value++
}

const viewOrderDetail = (order) => {
  currentOrder.value = { ...order }
  showDetailModal.value = true
}

const closeDetailModal = () => {
  showDetailModal.value = false
  currentOrder.value = {}
}

const updateOrderStatus = (order) => {
  currentOrder.value = { ...order }
  newStatus.value = order.status
  showStatusModal.value = true
}

const closeStatusModal = () => {
  showStatusModal.value = false
  currentOrder.value = {}
  newStatus.value = ''
}

const saveOrderStatus = async () => {
  try {
    saving.value = true
    
    const updatedOrder = {
      ...currentOrder.value,
      status: newStatus.value
    }
    
    await fetchApi(`/orders/${currentOrder.value.id}`, {
      method: 'PUT',
      body: JSON.stringify(updatedOrder)
    })
    
    closeStatusModal()
    await loadOrders()
    
  } catch (error) {
    console.error('Failed to update order status:', error)
    alert('狀態更新失敗，請稍後再試')
  } finally {
    saving.value = false
  }
}

const cancelOrder = async (order) => {
  if (!confirm(`確定要取消訂單 #${order.id} 嗎？`)) return
  
  try {
    const updatedOrder = {
      ...order,
      status: 'CANCELLED'
    }
    
    await fetchApi(`/orders/${order.id}`, {
      method: 'PUT',
      body: JSON.stringify(updatedOrder)
    })
    
    await loadOrders()
    
  } catch (error) {
    console.error('Failed to cancel order:', error)
    alert('取消訂單失敗，請稍後再試')
  }
}

// 生命週期
onMounted(() => {
  loadOrders()
})
</script>

<style scoped>
/* 基本樣式 */
.orders-admin {
  width: 100%;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.page-header h2 {
  color: #1e293b;
  font-size: 1.75rem;
  font-weight: 600;
}

/* 篩選區域 */
.filters {
  background: white;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
  flex-wrap: wrap;
}

.search-box {
  position: relative;
  flex: 1;
  max-width: 300px;
}

.search-box i {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #64748b;
}

.search-box input {
  width: 100%;
  padding: 10px 12px 10px 35px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 0.9rem;
}

.filter-buttons {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

/* 訂單樣式 */
.order-id {
  font-family: 'Courier New', monospace;
  font-weight: 600;
  color: #1e293b;
}

.order-amount {
  font-weight: 600;
  color: #059669;
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

/* 操作按鈕 */
.actions {
  display: flex;
  gap: 5px;
}

.btn-action {
  width: 32px;
  height: 32px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  transition: all 0.2s ease;
}

.btn-view {
  background-color: #3b82f6;
  color: white;
}

.btn-view:hover {
  background-color: #2563eb;
}

.btn-status {
  background-color: #10b981;
  color: white;
}

.btn-status:hover {
  background-color: #059669;
}

.btn-cancel {
  background-color: #ef4444;
  color: white;
}

.btn-cancel:hover {
  background-color: #dc2626;
}

/* 分頁 */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 30px;
}

.page-info {
  color: #64748b;
  font-size: 0.9rem;
}

/* Modal 樣式 */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 10px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-content.large {
  max-width: 800px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #e2e8f0;
}

.modal-header h3 {
  color: #1e293b;
  font-size: 1.25rem;
  font-weight: 600;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #64748b;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
}

.modal-close:hover {
  background-color: #f1f5f9;
}

.modal-body {
  padding: 20px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid #e2e8f0;
}

/* 訂單詳情樣式 */
.order-info .info-section {
  margin-bottom: 30px;
}

.order-info h4 {
  color: #1e293b;
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 15px;
  border-bottom: 2px solid #e2e8f0;
  padding-bottom: 8px;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
}

.info-item label {
  font-weight: 500;
  color: #374151;
}

.order-total {
  font-weight: 600;
  color: #059669;
  font-size: 1.1rem;
}

.order-items {
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  overflow: hidden;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #f1f5f9;
}

.order-item:last-child {
  border-bottom: none;
}

.item-info {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.item-name {
  font-weight: 500;
  color: #1e293b;
}

.item-quantity {
  font-size: 0.9rem;
  color: #64748b;
}

.item-price {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 5px;
}

.item-subtotal {
  font-size: 0.9rem;
  color: #059669;
  font-weight: 500;
}

.order-notes {
  background-color: #f8fafc;
  padding: 15px;
  border-radius: 8px;
  border-left: 4px solid #3b82f6;
  color: #374151;
  line-height: 1.5;
}

/* 表單樣式 */
.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
  color: #374151;
}

.status-select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.9rem;
  background-color: white;
}

.status-select:focus {
  outline: none;
  border-color: #3b82f6;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .filters {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-box {
    max-width: none;
  }
  
  .filter-buttons {
    justify-content: center;
  }
  
  .info-grid {
    grid-template-columns: 1fr;
  }
  
  .order-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
  
  .item-price {
    align-items: flex-start;
    width: 100%;
  }
}

/* 通用按鈕樣式 */
.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: linear-gradient(135deg, #006699, #0099cc);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: linear-gradient(135deg, #005580, #0088bb);
}

.btn-outline {
  background: white;
  color: #64748b;
  border: 1px solid #d1d5db;
}

.btn-outline:hover:not(:disabled) {
  background-color: #f8fafc;
  border-color: #9ca3af;
}

/* 表格樣式 */
.data-table {
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  margin-bottom: 30px;
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
</style>
