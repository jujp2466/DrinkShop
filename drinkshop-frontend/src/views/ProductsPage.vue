<template>
  <MainLayout>
    <div class="products-page">
      <!-- 頁面標題 -->
      <div class="page-header">
        <h1>所有產品</h1>
        <p>探索我們精選的美味飲品系列</p>
      </div>

      <!-- 搜尋和篩選區 -->
      <div class="filters-section">
        <div class="container">
          <div class="search-filters">
            <!-- 搜尋框 -->
            <div class="search-box">
              <input
                v-model="searchQuery"
                type="text"
                placeholder="搜尋飲品..."
                class="search-input"
              >
              <i class="fas fa-search search-icon"></i>
            </div>

            <!-- 價格篩選 -->
            <div class="filter-group">
              <label>價格範圍：</label>
              <select v-model="priceFilter" class="filter-select">
                <option value="">全部價格</option>
                <option value="0-50">NT$0 - NT$50</option>
                <option value="50-100">NT$50 - NT$100</option>
                <option value="100-200">NT$100 - NT$200</option>
                <option value="200+">NT$200+</option>
              </select>
            </div>

            <!-- 排序 -->
            <div class="filter-group">
              <label>排序方式：</label>
              <select v-model="sortBy" class="filter-select">
                <option value="name">按名稱</option>
                <option value="price-low">價格：低到高</option>
                <option value="price-high">價格：高到低</option>
                <option value="newest">最新商品</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <!-- 產品網格 -->
      <div class="container">
        <!-- 載入中 -->
        <div v-if="productStore.loading" class="loading">
          <i class="fas fa-spinner fa-spin"></i>
          載入產品中...
        </div>

        <!-- 錯誤訊息 -->
        <div v-else-if="productStore.error" class="error-message">
          <i class="fas fa-exclamation-triangle"></i>
          {{ productStore.error }}
          <button @click="retryLoad" class="btn btn-primary">重新載入</button>
        </div>

        <!-- 無產品 -->
        <div v-else-if="filteredProducts.length === 0" class="no-products">
          <i class="fas fa-search no-products-icon"></i>
          <h3>找不到符合條件的產品</h3>
          <p>請嘗試調整搜尋條件或篩選設定</p>
        </div>

        <!-- 產品列表 -->
        <div v-else class="products-grid">
          <ProductCard
            v-for="product in paginatedProducts"
            :key="product.id"
            :product="product"
            @add-to-cart="handleAddToCart"
          />
        </div>

        <!-- 分頁 -->
        <div v-if="totalPages > 1" class="pagination">
          <button
            @click="previousPage"
            :disabled="currentPage === 1"
            class="pagination-btn"
          >
            <i class="fas fa-chevron-left"></i>
            上一頁
          </button>

          <div class="page-numbers">
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="goToPage(page)"
              :class="['page-btn', { active: page === currentPage }]"
            >
              {{ page }}
            </button>
          </div>

          <button
            @click="nextPage"
            :disabled="currentPage === totalPages"
            class="pagination-btn"
          >
            下一頁
            <i class="fas fa-chevron-right"></i>
          </button>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useProductStore } from '@/stores/product'
import { useCartStore } from '@/stores/cart'
import MainLayout from '@/layouts/MainLayout.vue'
import ProductCard from '@/components/ProductCard.vue'

const productStore = useProductStore()
const cartStore = useCartStore()

// 搜尋和篩選狀態
const searchQuery = ref('')
const priceFilter = ref('')
const sortBy = ref('name')

// 分頁狀態
const currentPage = ref(1)
const itemsPerPage = 12

// 篩選後的產品
const filteredProducts = computed(() => {
  let products = productStore.availableProducts

  // 搜尋篩選
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    products = products.filter(product =>
      product.name.toLowerCase().includes(query) ||
      (product.description && product.description.toLowerCase().includes(query))
    )
  }

  // 價格篩選
  if (priceFilter.value) {
    const [min, max] = priceFilter.value.includes('+')
      ? [parseInt(priceFilter.value.replace('+', '')), Infinity]
      : priceFilter.value.split('-').map(Number)

    products = products.filter(product =>
      product.price >= min && product.price <= max
    )
  }

  // 排序
  products = [...products].sort((a, b) => {
    switch (sortBy.value) {
      case 'name':
        return a.name.localeCompare(b.name)
      case 'price-low':
        return a.price - b.price
      case 'price-high':
        return b.price - a.price
      case 'newest':
        return new Date(b.createdAt || 0) - new Date(a.createdAt || 0)
      default:
        return 0
    }
  })

  return products
})

// 分頁計算
const totalPages = computed(() =>
  Math.ceil(filteredProducts.value.length / itemsPerPage)
)

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredProducts.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages = []
  const total = totalPages.value
  const current = currentPage.value

  if (total <= 7) {
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    pages.push(1)
    
    if (current > 4) pages.push('...')
    
    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)
    
    for (let i = start; i <= end; i++) {
      pages.push(i)
    }
    
    if (current < total - 3) pages.push('...')
    
    pages.push(total)
  }
  
  return pages
})

// 分頁方法
const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const goToPage = (page) => {
  if (typeof page === 'number') {
    currentPage.value = page
  }
}

// 重設分頁當篩選條件改變時
watch([searchQuery, priceFilter, sortBy], () => {
  currentPage.value = 1
})

// 其他方法
const handleAddToCart = (product, quantity = 1) => {
  cartStore.addItem(product, quantity)
  alert(`${product.name} 已添加到購物車！`)
}

const retryLoad = async () => {
  await productStore.fetchProducts()
}

onMounted(async () => {
  if (productStore.products.length === 0) {
    await productStore.fetchProducts()
  }
})
</script>

<style scoped>
.products-page {
  min-height: 100vh;
}

.page-header {
  text-align: center;
  padding: 2rem 0 3rem 0;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.page-header h1 {
  font-size: 2.5rem;
  color: #1f2937;
  margin-bottom: 0.5rem;
  font-weight: 700;
}

.page-header p {
  font-size: 1.125rem;
  color: #6b7280;
}

.filters-section {
  background: white;
  border-bottom: 1px solid #e5e7eb;
  padding: 1.5rem 0;
  margin-bottom: 2rem;
}

.search-filters {
  display: flex;
  gap: 1.5rem;
  align-items: center;
  flex-wrap: wrap;
}

.search-box {
  position: relative;
  flex: 1;
  min-width: 250px;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem 0.75rem 3rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.search-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.search-icon {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
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

.filter-select {
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  background: white;
  cursor: pointer;
  transition: border-color 0.2s;
}

.filter-select:focus {
  outline: none;
  border-color: #2563eb;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.loading, .error-message, .no-products {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.loading i, .no-products-icon {
  font-size: 2rem;
  margin-bottom: 1rem;
  display: block;
}

.error-message {
  background-color: #fef2f2;
  color: #dc2626;
  border-radius: 0.5rem;
  border: 1px solid #fecaca;
}

.error-message i {
  margin-right: 0.5rem;
}

.error-message .btn {
  margin-top: 1rem;
}

.no-products h3 {
  margin-bottom: 0.5rem;
  color: #374151;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  padding: 2rem 0;
}

.pagination-btn, .page-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #d1d5db;
  background: white;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.pagination-btn:hover:not(:disabled), .page-btn:hover {
  background-color: #f3f4f6;
  border-color: #9ca3af;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-numbers {
  display: flex;
  gap: 0.25rem;
}

.page-btn.active {
  background-color: #2563eb;
  color: white;
  border-color: #2563eb;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .search-filters {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }

  .search-box {
    min-width: auto;
  }

  .filter-group {
    justify-content: space-between;
  }

  .products-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .pagination {
    flex-wrap: wrap;
  }

  .page-numbers {
    order: 1;
    width: 100%;
    justify-content: center;
    margin: 0.5rem 0;
  }
}

@media (max-width: 480px) {
  .search-input {
    padding: 0.625rem 0.875rem 0.625rem 2.5rem;
  }

  .search-icon {
    left: 0.75rem;
  }
}
</style>
