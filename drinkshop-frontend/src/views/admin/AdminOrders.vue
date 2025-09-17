<template>
  <div class="admin-orders">
    <div class="page-header">
      <h1>訂單管理</h1>
      <button @click="refreshOrders" class="btn btn-primary">
        <i class="fas fa-refresh"></i>
        刷新訂單
      </button>
    </div>

    <!-- 訂單統計 -->
    <div class="order-stats">
      <div class="stat-card">
        <div class="stat-icon pending">
          <i class="fas fa-clock"></i>
        </div>
        <div class="stat-info">
          <h3>{{ orderStats.pending }}</h3>
          <p>待處理</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon processing">
          <i class="fas fa-spinner"></i>
        </div>
        <div class="stat-info">
          <h3>{{ orderStats.processing }}</h3>
          <p>處理中</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon completed">
          <i class="fas fa-check"></i>
        </div>
        <div class="stat-info">
          <h3>{{ orderStats.completed }}</h3>
          <p>已完成</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon total">
          <i class="fas fa-list"></i>
        </div>
        <div class="stat-info">
          <h3>{{ orders.length }}</h3>
          <p>總訂單</p>
        </div>
      </div>
    </div>

    <!-- 訂單篩選 -->
    <div class="order-filters">
      <div class="filter-group">
        <label>狀態篩選：</label>
        <select v-model="statusFilter" class="filter-select">
          <option value="">全部狀態</option>
          <option value="pending">待處理</option>
          <option value="processing">處理中</option>
          <option value="shipped">已出貨</option>
          <option value="delivered">已送達</option>
          <option value="cancelled">已取消</option>
        </select>
      </div>
      
      <div class="filter-group">
        <label>搜尋訂單：</label>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="訂單編號或客戶姓名"
          class="search-input"
        >
      </div>
    </div>

    <!-- 訂單列表 -->
    <div class="orders-table-container">
      <div v-if="loading" class="loading">
        <i class="fas fa-spinner fa-spin"></i>
        載入中...
      </div>

      <div v-else-if="filteredOrders.length === 0" class="no-orders">
        <i class="fas fa-inbox"></i>
        <p>沒有找到符合條件的訂單</p>
      </div>

      <div v-else class="orders-table">
        <table>
          <thead>
            <tr>
              <th>訂單編號</th>
              <th>客戶資訊</th>
              <th>訂單金額</th>
              <th>訂單狀態</th>
              <th>付款方式</th>
              <th>訂單時間</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in filteredOrders" :key="order.id">
              <td>
                <span class="order-id">#{{ String(order.id).slice(-8) }}</span>
              </td>
              <td>
                <div class="customer-info">
                  <h4>{{ order.displayName || order.customerName }}</h4>
                  <p>{{ order.customerPhone }}</p>
                  <p>{{ order.customerEmail }}</p>
                </div>
              </td>
              <td>
                <div class="order-amount">
                  <span class="amount">NT$ {{ order.totalAmount.toLocaleString() }}</span>
                  <span class="item-count">{{ order.itemCount }} 件商品</span>
                </div>
              </td>
              <td>
                <select 
                  v-model="order.status" 
                  @change="updateOrderStatus(order)"
                  class="status-select"
                  :class="order.status"
                >
                  <option value="pending">待處理</option>
                  <option value="processing">處理中</option>
                  <option value="shipped">已出貨</option>
                  <option value="delivered">已送達</option>
                  <option value="cancelled">已取消</option>
                </select>
              </td>
              <td>
                <span class="payment-method">{{ getPaymentMethodText(order.paymentMethod) }}</span>
              </td>
              <td>
                <span class="order-date">{{ formatDateTime(order.createdAt) }}</span>
              </td>
              <td>
                <div class="actions">
                  <button @click="viewOrder(order)" class="btn-action view">
                    <i class="fas fa-eye"></i>
                  </button>
                  <button @click="deleteOrder(order)" class="btn-action delete">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- 訂單詳情模態框 -->
    <div v-if="showOrderModal" class="modal-overlay" @click="closeOrderModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>訂單詳情 #{{ String(selectedOrder?.id).slice(-8) }}</h2>
          <button @click="closeOrderModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div class="modal-body" v-if="selectedOrder">
          <!-- 客戶資訊 -->
          <div class="info-section">
            <h3>客戶資訊</h3>
            <div class="info-grid">
              <div class="info-item">
                <label>姓名：</label>
                <span>{{ selectedOrder.displayName || selectedOrder.customerName }}</span>
              </div>
              <div class="info-item">
                <label>電話：</label>
                <span>{{ selectedOrder.customerPhone }}</span>
              </div>
              <div class="info-item">
                <label>郵件：</label>
                <span>{{ selectedOrder.customerEmail }}</span>
              </div>
              <div class="info-item">
                <label>地址：</label>
                <span>{{ selectedOrder.shippingAddress }}</span>
              </div>
            </div>
          </div>

          <!-- 訂單商品 -->
          <div class="info-section">
            <h3>訂單商品</h3>
            <div class="order-items">
              <div v-for="item in selectedOrder.items" :key="item.id" class="order-item">
                <div class="item-name">{{ item.productName }}</div>
                <div class="item-quantity">x{{ item.quantity }}</div>
                <div class="item-price">NT$ {{ item.price.toLocaleString() }}</div>
              </div>
            </div>
            
            <div class="order-summary">
              <div class="summary-row">
                <span>小計：</span>
                <span>NT$ {{ (selectedOrder.totalAmount - selectedOrder.shippingFee).toLocaleString() }}</span>
              </div>
              <div class="summary-row">
                <span>運費：</span>
                <span>{{ selectedOrder.shippingFee === 0 ? '免運' : ('NT$ ' + selectedOrder.shippingFee.toLocaleString()) }}</span>
              </div>
              <div class="summary-row total">
                <span>總計：</span>
                <span>NT$ {{ selectedOrder.totalAmount.toLocaleString() }}</span>
              </div>
            </div>
          </div>

          <!-- 訂單備註 -->
          <div v-if="selectedOrder.notes" class="info-section">
            <h3>訂單備註</h3>
            <p class="order-notes">{{ selectedOrder.notes }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import emitter from '@/eventBus'
import api from '@/api'
import { formatDateTime } from '@/utils/date'

const loading = ref(false)
const orders = ref([])
const statusFilter = ref('')
const searchQuery = ref('')
const showOrderModal = ref(false)
const selectedOrder = ref(null)

// 訂單統計
const orderStats = computed(() => {
  const stats = {
    pending: 0,
    processing: 0,
    completed: 0
  }
  
  orders.value.forEach(order => {
    if (order.status === 'pending') stats.pending++
    else if (order.status === 'processing') stats.processing++
    else if (['delivered', 'shipped'].includes(order.status)) stats.completed++
  })
  
  return stats
})

// 篩選後的訂單
const filteredOrders = computed(() => {
  let filtered = orders.value

  if (statusFilter.value) {
    filtered = filtered.filter(order => order.status === statusFilter.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(order =>
      String(order.id).toLowerCase().includes(query) ||
      (order.displayName || order.customerName || '').toLowerCase().includes(query) ||
      (order.customerEmail || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
})

const getPaymentMethodText = (method) => {
  // 如果沒有提供付款方式，顯示為「未知」
  if (!method) return '未知'
  const methods = {
    'credit-card': '信用卡',
    'bank-transfer': '銀行轉帳',
    'cash-on-delivery': '貨到付款'
  }
  return methods[method] || method
}

// 使用共用日期工具，顯示使用者本機時區
// formatDateTime 已由 utils 提供


const mapOrder = (o) => ({
  id: o.id ?? o.Id ?? '',
  userId: o.userId ?? o.UserId ?? '',
  user: o.user ?? {},
  // 支援不同命名（UserDto.UserName -> JSON 可能為 userName）
  // prefer backend-provided displayName, fallback to various fields for compatibility
  customerName: o.displayName ?? o.user?.userName ?? o.user?.username ?? o.user?.name ?? o.customerName ?? o.CustomerName ?? '未知客戶',
  customerPhone: o.user?.phone ?? o.customerPhone ?? o.CustomerPhone ?? '',
  customerEmail: o.user?.email ?? o.customerEmail ?? o.CustomerEmail ?? '',
  shippingAddress: o.shippingAddress ?? o.ShippingAddress ?? '',
  totalAmount: o.totalAmount ?? o.TotalAmount ?? 0,
  shippingFee: o.shippingFee ?? o.ShippingFee ?? 0,
  itemCount: (o.items && o.items.length) || o.itemCount || 0,
  status: o.status ?? o.Status ?? 'pending',
  // 不在這裡預設為信用卡，若無值在顯示時會呈現「未知」
  paymentMethod: o.paymentMethod ?? o.PaymentMethod ?? null,
  createdAt: o.createdAt ?? o.CreatedAt,
  items: o.items ?? [],
  notes: o.notes ?? ''
})

const refreshOrders = async () => {
  loading.value = true
  try {
    const res = await api.get('/orders', { params: { role: 'admin' } })
    const list = res?.data?.data || []
    orders.value = list.map(mapOrder)
  } catch (error) {
    console.error('載入訂單失敗:', error)
    alert('載入訂單失敗，請檢查後端')
    orders.value = []
  } finally {
    loading.value = false
  }
}

const updateOrderStatus = async (order) => {
  try {
    const payload = { Status: order.status }
    await api.put(`/orders/${order.id}`, payload)
    emitter.emit('order-changed')
  } catch (error) {
    console.error('更新訂單狀態失敗:', error)
    alert('更新訂單狀態失敗')
  }
}

const viewOrder = (order) => {
  selectedOrder.value = order
  showOrderModal.value = true
}

const deleteOrder = async (order) => {
  if (confirm(`確定要刪除訂單 #${String(order.id).slice(-8)} 嗎？`)) {
    try {
      await api.delete(`/orders/${order.id}`)
      const index = orders.value.findIndex(o => o.id === order.id)
      if (index !== -1) orders.value.splice(index, 1)
      alert('訂單已刪除')
      emitter.emit('order-changed')
    } catch (error) {
      console.error('刪除訂單失敗:', error)
      alert('刪除訂單失敗')
    }
  }
}

const closeOrderModal = () => {
  showOrderModal.value = false
  selectedOrder.value = null
}

onMounted(() => {
  refreshOrders()
})
</script>

<style scoped>
.admin-orders {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  color: #1f2937;
  margin: 0;
}

.order-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
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
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.stat-icon.pending {
  background: linear-gradient(135deg, #f59e0b, #d97706);
}

.stat-icon.processing {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.stat-icon.completed {
  background: linear-gradient(135deg, #10b981, #059669);
}

.stat-icon.total {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
}

.stat-info h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0 0 0.25rem 0;
}

.stat-info p {
  color: #6b7280;
  margin: 0;
  font-size: 0.875rem;
}

.order-filters {
  background: white;
  padding: 1.5rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-group label {
  font-weight: 500;
  color: #374151;
  white-space: nowrap;
}

.filter-select, .search-input {
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.search-input {
  min-width: 200px;
}

.orders-table-container {
  background: white;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.loading, .no-orders {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.no-orders i {
  font-size: 3rem;
  margin-bottom: 1rem;
  display: block;
}

.orders-table {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #f3f4f6;
}

th {
  background: #f8fafc;
  font-weight: 600;
  color: #374151;
}

.order-id {
  font-family: 'Courier New', monospace;
  font-weight: 600;
  color: #2563eb;
}

.customer-info h4 {
  margin: 0 0 0.25rem 0;
  color: #1f2937;
}

.customer-info p {
  margin: 0;
  color: #6b7280;
  font-size: 0.875rem;
}

.order-amount .amount {
  font-weight: 600;
  color: #2563eb;
  display: block;
}

.order-amount .item-count {
  font-size: 0.875rem;
  color: #6b7280;
}

.status-select {
  padding: 0.25rem 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.status-select.pending {
  background: #fef3c7;
  color: #92400e;
}

.status-select.processing {
  background: #dbeafe;
  color: #1e40af;
}

.status-select.shipped {
  background: #d1fae5;
  color: #065f46;
}

.status-select.delivered {
  background: #dcfce7;
  color: #166534;
}

.status-select.cancelled {
  background: #fee2e2;
  color: #991b1b;
}

.payment-method {
  background: #f3f4f6;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.order-date {
  color: #6b7280;
  font-size: 0.875rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.btn-action {
  padding: 0.5rem;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-action.view {
  background: #dbeafe;
  color: #2563eb;
}

.btn-action.view:hover {
  background: #bfdbfe;
}

.btn-action.delete {
  background: #fee2e2;
  color: #dc2626;
}

.btn-action.delete:hover {
  background: #fecaca;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-primary {
  background: #2563eb;
  color: white;
}

.btn-primary:hover {
  background: #1d4ed8;
}

/* 模態框樣式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.modal {
  background: white;
  border-radius: 1rem;
  max-width: 600px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
  color: #1f2937;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: #6b7280;
  padding: 0.5rem;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.close-btn:hover {
  background: #f3f4f6;
}

.modal-body {
  padding: 1.5rem;
}

.info-section {
  margin-bottom: 2rem;
}

.info-section:last-child {
  margin-bottom: 0;
}

.info-section h3 {
  color: #1f2937;
  margin-bottom: 1rem;
  font-size: 1.125rem;
  font-weight: 600;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.info-item {
  display: flex;
  gap: 0.5rem;
}

.info-item label {
  font-weight: 500;
  color: #6b7280;
  min-width: 60px;
}

.order-items {
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  overflow: hidden;
  margin-bottom: 1rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.order-item:last-child {
  border-bottom: none;
}

.item-name {
  flex: 1;
  font-weight: 500;
}

.item-quantity {
  color: #6b7280;
  margin: 0 1rem;
}

.item-price {
  font-weight: 600;
  color: #2563eb;
}

.order-summary {
  background: #f9fafb;
  padding: 1rem;
  border-radius: 0.5rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.summary-row.total {
  font-weight: 600;
  font-size: 1.125rem;
  color: #2563eb;
  border-top: 1px solid #e5e7eb;
  padding-top: 0.5rem;
  margin-top: 1rem;
}

.order-notes {
  background: #f9fafb;
  padding: 1rem;
  border-radius: 0.5rem;
  border-left: 4px solid #2563eb;
  color: #374151;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .order-filters {
    flex-direction: column;
    gap: 1rem;
  }

  .filter-group {
    flex-direction: column;
    align-items: flex-start;
  }

  .search-input {
    min-width: auto;
    width: 100%;
  }

  table {
    font-size: 0.875rem;
  }

  th, td {
    padding: 0.75rem 0.5rem;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }

  .modal {
    margin: 1rem;
  }
}
</style>
