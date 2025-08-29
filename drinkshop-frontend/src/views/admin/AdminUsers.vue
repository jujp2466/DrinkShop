<template>
  <div class="admin-users">
    <div class="page-header">
      <h1>用戶管理</h1>
      <button @click="showCreateUserModal = true" class="btn btn-primary">
        <i class="fas fa-plus"></i>
        新增用戶
      </button>
    </div>

    <!-- 用戶統計 -->
    <div class="user-stats">
      <div class="stat-card">
        <div class="stat-icon customers">
          <i class="fas fa-users"></i>
        </div>
        <div class="stat-info">
          <h3>{{ userStats.customers }}</h3>
          <p>一般用戶</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon admins">
          <i class="fas fa-user-shield"></i>
        </div>
        <div class="stat-info">
          <h3>{{ userStats.admins }}</h3>
          <p>管理員</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon active">
          <i class="fas fa-user-check"></i>
        </div>
        <div class="stat-info">
          <h3>{{ userStats.active }}</h3>
          <p>活躍用戶</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon total">
          <i class="fas fa-users-cog"></i>
        </div>
        <div class="stat-info">
          <h3>{{ users.length }}</h3>
          <p>總用戶數</p>
        </div>
      </div>
    </div>

    <!-- 用戶篩選 -->
    <div class="user-filters">
      <div class="filter-group">
        <label>角色篩選：</label>
        <select v-model="roleFilter" class="filter-select">
          <option value="">全部角色</option>
          <option value="customer">一般用戶</option>
          <option value="admin">管理員</option>
        </select>
      </div>
      
      <div class="filter-group">
        <label>狀態篩選：</label>
        <select v-model="statusFilter" class="filter-select">
          <option value="">全部狀態</option>
          <option value="active">已啟用</option>
          <option value="inactive">已停用</option>
        </select>
      </div>
      
      <div class="filter-group">
        <label>搜尋用戶：</label>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="姓名、郵件或電話"
          class="search-input"
        >
      </div>
    </div>

    <!-- 用戶列表 -->
    <div class="users-table-container">
      <div v-if="loading" class="loading">
        <i class="fas fa-spinner fa-spin"></i>
        載入中...
      </div>

      <div v-else-if="filteredUsers.length === 0" class="no-users">
        <i class="fas fa-user-slash"></i>
        <p>沒有找到符合條件的用戶</p>
      </div>

      <div v-else class="users-table">
        <table>
          <thead>
            <tr>
              <th>用戶資訊</th>
              <th>聯絡方式</th>
              <th>角色</th>
              <th>狀態</th>
              <th>註冊時間</th>
              <th>最後登入</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in filteredUsers" :key="user.id">
              <td>
                <div class="user-info">
                  <div class="user-avatar">
                    <i class="fas fa-user"></i>
                  </div>
                  <div class="user-details">
                    <h4>{{ user.name }}</h4>
                    <p>ID: {{ user.id.slice(-8) }}</p>
                  </div>
                </div>
              </td>
              <td>
                <div class="contact-info">
                  <p><i class="fas fa-envelope"></i> {{ user.email }}</p>
                  <p><i class="fas fa-phone"></i> {{ user.phone || '-' }}</p>
                </div>
              </td>
              <td>
                <span class="role-badge" :class="user.role">
                  {{ getRoleText(user.role) }}
                </span>
              </td>
              <td>
                <label class="status-toggle">
                  <input 
                    type="checkbox" 
                    :checked="user.status === 'active'"
                    @change="toggleUserStatus(user)"
                  >
                  <span class="toggle-slider"></span>
                  <span class="status-text">{{ getStatusText(user.status) }}</span>
                </label>
              </td>
              <td>
                <span class="date">{{ formatDateTime(user.createdAt) }}</span>
              </td>
              <td>
                <span class="date">{{ formatDateTime(user.lastLoginAt) }}</span>
              </td>
              <td>
                <div class="actions">
                  <button @click="editUser(user)" class="btn-action edit">
                    <i class="fas fa-edit"></i>
                  </button>
                  <button @click="viewUserOrders(user)" class="btn-action orders">
                    <i class="fas fa-shopping-cart"></i>
                  </button>
                  <button 
                    @click="deleteUser(user)" 
                    class="btn-action delete"
                    :disabled="user.role === 'admin' && user.id === currentUserId"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- 創建/編輯用戶模態框 -->
    <div v-if="showCreateUserModal || showEditUserModal" class="modal-overlay" @click="closeUserModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>{{ showCreateUserModal ? '新增用戶' : '編輯用戶' }}</h2>
          <button @click="closeUserModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div class="modal-body">
          <form @submit.prevent="submitUserForm">
            <div class="form-group">
              <label>姓名 <span class="required">*</span></label>
              <input 
                v-model="userForm.name" 
                type="text" 
                required 
                placeholder="請輸入用戶姓名"
              >
            </div>
            
            <div class="form-group">
              <label>郵箱 <span class="required">*</span></label>
              <input 
                v-model="userForm.email" 
                type="email" 
                required 
                placeholder="請輸入郵箱地址"
                :disabled="showEditUserModal"
              >
            </div>
            
            <div class="form-group">
              <label>電話</label>
              <input 
                v-model="userForm.phone" 
                type="tel" 
                placeholder="請輸入電話號碼"
              >
            </div>
            
            <div v-if="showCreateUserModal" class="form-group">
              <label>密碼 <span class="required">*</span></label>
              <input 
                v-model="userForm.password" 
                type="password" 
                required 
                placeholder="請輸入密碼"
                minlength="6"
              >
            </div>
            
            <div class="form-group">
              <label>角色</label>
              <select v-model="userForm.role">
                <option value="customer">一般用戶</option>
                <option value="admin">管理員</option>
              </select>
            </div>
            
            <div class="form-group">
              <label>地址</label>
              <textarea 
                v-model="userForm.address" 
                placeholder="請輸入地址"
                rows="3"
              ></textarea>
            </div>
            
            <div class="form-actions">
              <button type="button" @click="closeUserModal" class="btn btn-secondary">
                取消
              </button>
              <button type="submit" class="btn btn-primary">
                {{ showCreateUserModal ? '新增' : '更新' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- 用戶訂單模態框 -->
    <div v-if="showOrdersModal" class="modal-overlay" @click="closeOrdersModal">
      <div class="modal modal-lg" @click.stop>
        <div class="modal-header">
          <h2>{{ selectedUser?.name }} 的訂單記錄</h2>
          <button @click="closeOrdersModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div class="modal-body">
          <div v-if="userOrders.length === 0" class="no-orders">
            <i class="fas fa-shopping-cart"></i>
            <p>此用戶還沒有任何訂單</p>
          </div>
          
          <div v-else class="orders-list">
            <div v-for="order in userOrders" :key="order.id" class="order-card">
              <div class="order-header">
                <span class="order-id">#{{ order.id.slice(-8) }}</span>
                <span class="order-status" :class="order.status">
                  {{ getOrderStatusText(order.status) }}
                </span>
              </div>
              <div class="order-details">
                <p><i class="fas fa-calendar"></i> {{ formatDateTime(order.createdAt) }}</p>
                <p><i class="fas fa-dollar-sign"></i> NT$ {{ order.totalAmount.toLocaleString() }}</p>
                <p><i class="fas fa-box"></i> {{ order.itemCount }} 件商品</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const loading = ref(false)
const users = ref([])
const roleFilter = ref('')
const statusFilter = ref('')
const searchQuery = ref('')

const showCreateUserModal = ref(false)
const showEditUserModal = ref(false)
const showOrdersModal = ref(false)
const selectedUser = ref(null)
const userOrders = ref([])
const currentUserId = ref('current-admin-id') // 假設當前管理員ID

const userForm = ref({
  name: '',
  email: '',
  phone: '',
  password: '',
  role: 'customer',
  address: ''
})

// 用戶統計
const userStats = computed(() => {
  const stats = {
    customers: 0,
    admins: 0,
    active: 0
  }
  
  users.value.forEach(user => {
    if (user.role === 'customer') stats.customers++
    if (user.role === 'admin') stats.admins++
    if (user.status === 'active') stats.active++
  })
  
  return stats
})

// 篩選後的用戶
const filteredUsers = computed(() => {
  let filtered = users.value

  if (roleFilter.value) {
    filtered = filtered.filter(user => user.role === roleFilter.value)
  }

  if (statusFilter.value) {
    filtered = filtered.filter(user => user.status === statusFilter.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(user =>
      user.name.toLowerCase().includes(query) ||
      user.email.toLowerCase().includes(query) ||
      (user.phone && user.phone.includes(query))
    )
  }

  return filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
})

const getRoleText = (role) => {
  return role === 'admin' ? '管理員' : '一般用戶'
}

const getStatusText = (status) => {
  return status === 'active' ? '已啟用' : '已停用'
}

const getOrderStatusText = (status) => {
  const statusTexts = {
    'pending': '待處理',
    'processing': '處理中',
    'shipped': '已出貨',
    'delivered': '已送達',
    'cancelled': '已取消'
  }
  return statusTexts[status] || status
}

const formatDateTime = (dateString) => {
  if (!dateString) return '-'
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW') + ' ' + date.toLocaleTimeString('zh-TW', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

const generateMockUsers = () => {
  const users = []
  const names = ['王小明', '李美玲', '張志偉', '陳雅芬', '林俊德', '黃淑芬', '劉建國', '吳佩蓉', '蔡正義', '許雅婷']
  const domains = ['gmail.com', 'yahoo.com', 'hotmail.com', 'example.com']
  
  // 添加當前管理員
  users.push({
    id: 'current-admin-id',
    name: '系統管理員',
    email: 'admin@drinkshop.com',
    phone: '0912345678',
    role: 'admin',
    status: 'active',
    address: '台北市信義區信義路五段1號',
    createdAt: new Date('2023-01-01'),
    lastLoginAt: new Date()
  })
  
  for (let i = 0; i < 15; i++) {
    const name = names[i % names.length]
    const domain = domains[Math.floor(Math.random() * domains.length)]
    const email = `${name.replace(/[^\w]/g, '').toLowerCase()}${i}@${domain}`
    
    users.push({
      id: `user-${Date.now()}-${i}`,
      name: name,
      email: email,
      phone: Math.random() > 0.3 ? `09${Math.floor(Math.random() * 100000000).toString().padStart(8, '0')}` : null,
      role: Math.random() > 0.85 ? 'admin' : 'customer',
      status: Math.random() > 0.1 ? 'active' : 'inactive',
      address: `台北市${['中正區', '大同區', '中山區', '松山區', '大安區', '萬華區', '信義區'][Math.floor(Math.random() * 7)]}${Math.floor(Math.random() * 99) + 1}號`,
      createdAt: new Date(Date.now() - Math.random() * 365 * 24 * 60 * 60 * 1000),
      lastLoginAt: Math.random() > 0.2 ? new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000) : null
    })
  }
  
  return users
}

const generateUserOrders = (userId) => {
  const orderCount = Math.floor(Math.random() * 8) + 1
  const orders = []
  const statuses = ['pending', 'processing', 'shipped', 'delivered', 'cancelled']
  
  for (let i = 0; i < orderCount; i++) {
    orders.push({
      id: `order-${userId}-${i}`,
      status: statuses[Math.floor(Math.random() * statuses.length)],
      totalAmount: Math.floor(Math.random() * 2000) + 300,
      itemCount: Math.floor(Math.random() * 5) + 1,
      createdAt: new Date(Date.now() - Math.random() * 180 * 24 * 60 * 60 * 1000)
    })
  }
  
  return orders.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
}

const loadUsers = async () => {
  loading.value = true
  try {
    // 模擬 API 調用
    await new Promise(resolve => setTimeout(resolve, 800))
    users.value = generateMockUsers()
  } finally {
    loading.value = false
  }
}

const toggleUserStatus = async (user) => {
  const newStatus = user.status === 'active' ? 'inactive' : 'active'
  
  try {
    // 模擬 API 調用
    await new Promise(resolve => setTimeout(resolve, 300))
    user.status = newStatus
    console.log(`用戶 ${user.name} 狀態已更新為 ${newStatus}`)
  } catch (error) {
    console.error('更新用戶狀態失敗:', error)
    alert('更新用戶狀態失敗')
  }
}

const editUser = (user) => {
  selectedUser.value = user
  userForm.value = {
    name: user.name,
    email: user.email,
    phone: user.phone || '',
    password: '',
    role: user.role,
    address: user.address || ''
  }
  showEditUserModal.value = true
}

const viewUserOrders = async (user) => {
  selectedUser.value = user
  showOrdersModal.value = true
  
  // 載入用戶訂單
  try {
    await new Promise(resolve => setTimeout(resolve, 500))
    userOrders.value = generateUserOrders(user.id)
  } catch (error) {
    console.error('載入用戶訂單失敗:', error)
    userOrders.value = []
  }
}

const deleteUser = async (user) => {
  if (user.role === 'admin' && user.id === currentUserId.value) {
    alert('不能刪除自己的帳戶')
    return
  }

  if (confirm(`確定要刪除用戶 ${user.name} 嗎？`)) {
    try {
      // 模擬 API 調用
      await new Promise(resolve => setTimeout(resolve, 500))
      const index = users.value.findIndex(u => u.id === user.id)
      if (index !== -1) {
        users.value.splice(index, 1)
      }
      alert('用戶已刪除')
    } catch (error) {
      console.error('刪除用戶失敗:', error)
      alert('刪除用戶失敗')
    }
  }
}

const submitUserForm = async () => {
  try {
    // 模擬 API 調用
    await new Promise(resolve => setTimeout(resolve, 800))
    
    if (showCreateUserModal.value) {
      // 新增用戶
      const newUser = {
        id: `user-${Date.now()}`,
        name: userForm.value.name,
        email: userForm.value.email,
        phone: userForm.value.phone,
        role: userForm.value.role,
        status: 'active',
        address: userForm.value.address,
        createdAt: new Date(),
        lastLoginAt: null
      }
      users.value.unshift(newUser)
      alert('用戶創建成功')
    } else {
      // 更新用戶
      Object.assign(selectedUser.value, {
        name: userForm.value.name,
        phone: userForm.value.phone,
        role: userForm.value.role,
        address: userForm.value.address
      })
      alert('用戶資料更新成功')
    }
    
    closeUserModal()
  } catch (error) {
    console.error('提交失敗:', error)
    alert('操作失敗，請重試')
  }
}

const closeUserModal = () => {
  showCreateUserModal.value = false
  showEditUserModal.value = false
  selectedUser.value = null
  userForm.value = {
    name: '',
    email: '',
    phone: '',
    password: '',
    role: 'customer',
    address: ''
  }
}

const closeOrdersModal = () => {
  showOrdersModal.value = false
  selectedUser.value = null
  userOrders.value = []
}

onMounted(() => {
  loadUsers()
})
</script>

<style scoped>
.admin-users {
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

.user-stats {
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

.stat-icon.customers {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.stat-icon.admins {
  background: linear-gradient(135deg, #ef4444, #dc2626);
}

.stat-icon.active {
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

.user-filters {
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

.users-table-container {
  background: white;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.loading, .no-users {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.no-users i {
  font-size: 3rem;
  margin-bottom: 1rem;
  display: block;
}

.users-table {
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

.user-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-avatar {
  width: 40px;
  height: 40px;
  background: #e5e7eb;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6b7280;
}

.user-details h4 {
  margin: 0 0 0.25rem 0;
  color: #1f2937;
}

.user-details p {
  margin: 0;
  color: #6b7280;
  font-size: 0.75rem;
  font-family: 'Courier New', monospace;
}

.contact-info p {
  margin: 0.25rem 0;
  color: #6b7280;
  font-size: 0.875rem;
}

.contact-info i {
  width: 16px;
  margin-right: 0.5rem;
}

.role-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.role-badge.customer {
  background: #dbeafe;
  color: #1e40af;
}

.role-badge.admin {
  background: #fee2e2;
  color: #991b1b;
}

.status-toggle {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.status-toggle input {
  display: none;
}

.toggle-slider {
  width: 40px;
  height: 20px;
  background: #d1d5db;
  border-radius: 10px;
  position: relative;
  transition: background-color 0.3s;
}

.toggle-slider::before {
  content: '';
  width: 16px;
  height: 16px;
  background: white;
  border-radius: 50%;
  position: absolute;
  top: 2px;
  left: 2px;
  transition: transform 0.3s;
}

.status-toggle input:checked + .toggle-slider {
  background: #10b981;
}

.status-toggle input:checked + .toggle-slider::before {
  transform: translateX(20px);
}

.status-text {
  font-size: 0.875rem;
  font-weight: 500;
}

.date {
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

.btn-action:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-action.edit {
  background: #fef3c7;
  color: #92400e;
}

.btn-action.edit:hover:not(:disabled) {
  background: #fde68a;
}

.btn-action.orders {
  background: #dbeafe;
  color: #2563eb;
}

.btn-action.orders:hover:not(:disabled) {
  background: #bfdbfe;
}

.btn-action.delete {
  background: #fee2e2;
  color: #dc2626;
}

.btn-action.delete:hover:not(:disabled) {
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

.btn-secondary {
  background: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background: #4b5563;
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
  max-width: 500px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-lg {
  max-width: 800px;
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

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #374151;
}

.required {
  color: #ef4444;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #2563eb;
}

.form-group input:disabled {
  background: #f9fafb;
  color: #6b7280;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
}

.no-orders {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
}

.no-orders i {
  font-size: 2rem;
  margin-bottom: 1rem;
  display: block;
}

.orders-list {
  display: grid;
  gap: 1rem;
  max-height: 400px;
  overflow-y: auto;
}

.order-card {
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  padding: 1rem;
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.order-id {
  font-family: 'Courier New', monospace;
  font-weight: 600;
  color: #2563eb;
}

.order-status {
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.75rem;
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

.order-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 0.5rem;
}

.order-details p {
  margin: 0;
  font-size: 0.875rem;
  color: #6b7280;
}

.order-details i {
  width: 16px;
  margin-right: 0.5rem;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .user-filters {
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

  .user-info {
    gap: 0.5rem;
  }

  .user-avatar {
    width: 32px;
    height: 32px;
  }

  .modal {
    margin: 1rem;
  }

  .form-actions {
    flex-direction: column;
  }

  .order-details {
    grid-template-columns: 1fr;
  }
}
</style>
