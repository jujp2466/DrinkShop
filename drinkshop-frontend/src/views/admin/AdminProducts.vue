<template>
  <div class="admin-products">
    <div class="page-header">
      <h1>產品管理</h1>
      <button @click="openCreateModal" class="btn btn-primary">
        <i class="fas fa-plus"></i>
        新增產品
      </button>
    </div>

    <!-- 產品列表 -->
    <div class="products-table-container">
      <div v-if="productStore.loading" class="loading">
        <i class="fas fa-spinner fa-spin"></i>
        載入中...
      </div>

      <div v-else-if="productStore.error" class="error-message">
        <i class="fas fa-exclamation-triangle"></i>
        {{ productStore.error }}
        <button @click="loadProducts" class="btn btn-primary">重新載入</button>
      </div>

      <div v-else class="products-table">
        <table>
          <thead>
            <tr>
              <th>產品圖片</th>
              <th>產品名稱</th>
              <th>價格</th>
              <th>狀態</th>
              <th>創建時間</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in productStore.products" :key="product.id">
              <td>
                <div class="product-image">
                  <img :src="imageSrc(product)" :alt="product.name">
                </div>
              </td>
              <td>
                <div class="product-name">
                  <h4>{{ product.name }}</h4>
                  <p v-if="product.description">{{ product.description.substring(0, 50) }}...</p>
                </div>
              </td>
              <td>
                <span class="price">NT$ {{ product.price }}</span>
              </td>
              <td>
                <span class="status" :class="{ active: product.isActive, inactive: !product.isActive }">
                  {{ product.isActive ? '上架' : '下架' }}
                </span>
              </td>
              <td>
                <span class="date">{{ formatDate(product.createdAt) }}</span>
              </td>
              <td>
                <div class="actions">
                  <button @click="editProduct(product)" class="btn-action edit">
                    <i class="fas fa-edit"></i>
                  </button>
                  <button @click="deleteProduct(product)" class="btn-action delete">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- 產品編輯模態框 (簡化版本) -->
    <div v-if="showModal" class="modal-overlay" @click="closeModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>{{ editingProduct ? '編輯產品' : '新增產品' }}</h2>
          <button @click="closeModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <form @submit.prevent="saveProduct" class="modal-body">
          <div class="form-group">
            <label for="productName">產品名稱 *</label>
            <input
              id="productName"
              v-model="productForm.name"
              type="text"
              required
              class="form-input"
              placeholder="請輸入產品名稱"
            >
          </div>
          
          <div class="form-group">
            <label for="productPrice">價格 *</label>
            <input
              id="productPrice"
              v-model="productForm.price"
              type="number"
              min="0"
              step="0.01"
              required
              class="form-input"
              placeholder="請輸入價格"
            >
          </div>
          
          <div class="form-group">
            <label for="productDescription">產品描述</label>
            <textarea
              id="productDescription"
              v-model="productForm.description"
              rows="4"
              class="form-input"
              placeholder="請輸入產品描述"
            ></textarea>
          </div>
          
          <div class="form-group">
            <label>
              <input
                v-model="productForm.isActive"
                type="checkbox"
              >
              立即上架
            </label>
          </div>
          
          <div class="modal-footer">
            <button type="button" @click="closeModal" class="btn btn-secondary">
              取消
            </button>
            <button type="submit" :disabled="saving" class="btn btn-primary">
              {{ saving ? '儲存中...' : '儲存' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useProductStore } from '@/stores/product'

const productStore = useProductStore()

const showModal = ref(false)
const editingProduct = ref(null)
const saving = ref(false)

const productForm = reactive({
  name: '',
  price: 0,
  description: '',
  isActive: true
})

const formatDate = (dateString) => {
  if (!dateString) return '-'
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW')
}

// 統一圖片來源處理：支援 imageUrl / image / 相對路徑 與預設圖；茶類優先用茶圖
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

const loadProducts = async () => {
  await productStore.fetchProducts()
}

const openCreateModal = () => {
  editingProduct.value = null
  resetForm()
  showModal.value = true
}

const editProduct = (product) => {
  editingProduct.value = product
  productForm.name = product.name
  productForm.price = product.price
  productForm.description = product.description || ''
  productForm.isActive = product.isActive
  showModal.value = true
}

const deleteProduct = async (product) => {
  if (confirm(`確定要刪除產品「${product.name}」嗎？`)) {
    const result = await productStore.deleteProduct(product.id)
    if (result.success) {
      alert('產品已成功刪除')
    } else {
      alert(`刪除失敗：${result.error}`)
    }
  }
}

const saveProduct = async () => {
  saving.value = true
  
  try {
    let result
    if (editingProduct.value) {
      result = await productStore.updateProduct(editingProduct.value.id, productForm)
    } else {
      result = await productStore.createProduct(productForm)
    }
    
    if (result.success) {
      alert(editingProduct.value ? '產品已更新' : '產品已新增')
      closeModal()
    } else {
      alert(`操作失敗：${result.error}`)
    }
  } finally {
    saving.value = false
  }
}

const closeModal = () => {
  showModal.value = false
  editingProduct.value = null
  resetForm()
}

const resetForm = () => {
  productForm.name = ''
  productForm.price = 0
  productForm.description = ''
  productForm.isActive = true
}

onMounted(() => {
  loadProducts()
})
</script>

<style scoped>
.admin-products {
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

.products-table-container {
  background: white;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.loading, .error-message {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.error-message {
  color: #dc2626;
}

.products-table {
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

.product-image {
  width: 60px;
  height: 60px;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 0.5rem;
}

.product-name h4 {
  margin: 0 0 0.25rem 0;
  color: #1f2937;
}

.product-name p {
  margin: 0;
  color: #6b7280;
  font-size: 0.875rem;
}

.price {
  font-weight: 600;
  color: #2563eb;
}

.status {
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.status.active {
  background: #dcfce7;
  color: #166534;
}

.status.inactive {
  background: #fee2e2;
  color: #991b1b;
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

.btn-action.edit {
  background: #dbeafe;
  color: #2563eb;
}

.btn-action.edit:hover {
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

.btn-primary:hover:not(:disabled) {
  background: #1d4ed8;
}

.btn-primary:disabled {
  background: #9ca3af;
  cursor: not-allowed;
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
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #374151;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.modal-footer {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1.5rem;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .page-header h1 {
    font-size: 1.5rem;
  }

  table {
    font-size: 0.875rem;
  }

  th, td {
    padding: 0.75rem 0.5rem;
  }

  .modal {
    margin: 1rem;
  }

  .modal-footer {
    flex-direction: column;
  }
}
</style>
