<template>
  <div class="product-card">
    <div class="product-image">
      <img :src="imageSrc" :alt="product.name" />
      <div class="product-badge" v-if="product.isNew">NEW</div>
    </div>
    
    <div class="product-info">
      <h3 class="product-name">{{ product.name }}</h3>
      <p class="product-description">{{ product.description || '美味飲品，值得品嚐' }}</p>
      
      <div class="product-details">
        <div class="product-price">
          <span class="currency">NT$</span>
          <span class="amount">{{ product.price }}</span>
        </div>
        
        <div class="product-actions">
          <div class="quantity-selector">
            <button @click="decreaseQuantity" class="qty-btn" :disabled="quantity <= 1">-</button>
            <span class="quantity">{{ quantity }}</span>
            <button @click="increaseQuantity" class="qty-btn">+</button>
          </div>
          
          <button @click="addToCart" class="add-to-cart-btn" :disabled="!product.isActive">
            <i class="fas fa-cart-plus"></i>
            {{ product.isActive ? '加入購物車' : '缺貨' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import api from '../api'

const props = defineProps({
  product: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['add-to-cart'])

const quantity = ref(1)

// 產出最終圖片 URL：
// 1) 優先使用 product.imageUrl，其次 product.image
// 2) 若是相對路徑（以 / 開頭），嘗試直接使用（Vite 會從 /public 提供）；
// 3) 否則回退到 Unsplash 圖片。
// 產出最終圖片 URL：
// 1) 優先使用 product.imageUrl，其次 product.image
// 2) 若是相對路徑（以 / 開頭），嘗試直接使用（Vite 會從 /public 提供）；
// 3) 否則回退到 Unsplash 圖片。茶類使用專屬茶圖。
const imageSrc = computed(() => {
  const candidate = props.product.imageUrl || props.product.image || ''
  const name = (props.product.name || '').toLowerCase()
  const category = (props.product.category || '').toLowerCase()
  const isTea = /茶|tea/.test(name) || /茶|tea/.test(category)
  if (!candidate) {
    return isTea
      ? 'https://images.unsplash.com/photo-1544145945-f90425340c7e?w=400&h=300&fit=crop' // tea fallback
      : 'https://images.unsplash.com/photo-1504674900247-0877df9cc836'
  }
  if (candidate.startsWith('http://') || candidate.startsWith('https://')) {
    return candidate
  }
  if (candidate.startsWith('/')) {
    return candidate
  }
  return candidate
})

const increaseQuantity = () => {
  quantity.value++
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

const addToCart = () => {
  if (props.product.isActive) {
    emit('add-to-cart', props.product, quantity.value)
    // 重置數量
    quantity.value = 1
  }
}
</script>

<style scoped>
.product-card {
  background: white;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: transform 0.3s, box-shadow 0.3s;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.product-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.product-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.product-card:hover .product-image img {
  transform: scale(1.05);
}

.product-badge {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: linear-gradient(45deg, #f59e0b, #d97706);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.product-info {
  padding: 1.5rem;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.product-name {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #1f2937;
}

.product-description {
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.5;
  margin-bottom: 1rem;
  flex: 1;
}

.product-details {
  margin-top: auto;
}

.product-price {
  margin-bottom: 1rem;
  text-align: center;
}

.currency {
  font-size: 1rem;
  color: #6b7280;
}

.amount {
  font-size: 1.75rem;
  font-weight: 700;
  color: #2563eb;
  margin-left: 0.25rem;
}

.product-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.quantity-selector {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.qty-btn {
  width: 36px;
  height: 36px;
  border: 1px solid #d1d5db;
  background: white;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  transition: all 0.2s;
}

.qty-btn:hover:not(:disabled) {
  background-color: #f3f4f6;
  border-color: #9ca3af;
}

.qty-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity {
  min-width: 2rem;
  text-align: center;
  font-weight: 600;
  font-size: 1.125rem;
}

.add-to-cart-btn {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: all 0.3s;
}

.add-to-cart-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #059669, #047857);
  transform: translateY(-1px);
}

.add-to-cart-btn:disabled {
  background: #9ca3af;
  cursor: not-allowed;
  transform: none;
}

.add-to-cart-btn i {
  font-size: 1rem;
}

/* 響應式設計 */
@media (max-width: 480px) {
  .product-info {
    padding: 1rem;
  }

  .product-name {
    font-size: 1.125rem;
  }

  .amount {
    font-size: 1.5rem;
  }

  .add-to-cart-btn {
    padding: 0.625rem 1.25rem;
    font-size: 0.875rem;
  }
}
</style>
