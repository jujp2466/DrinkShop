<template>
  <!-- 購物車遮罩 -->
  <div v-if="cartStore.isOpen" class="cart-overlay" @click="cartStore.closeCart">
    <!-- 購物車側邊欄 -->
    <div class="cart-sidebar" @click.stop>
      <div class="cart-header">
        <h3>購物車</h3>
        <button @click="cartStore.closeCart" class="close-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <div class="cart-content">
        <!-- 空購物車 -->
        <div v-if="cartStore.isCartEmpty" class="empty-cart">
          <i class="fas fa-shopping-cart empty-icon"></i>
          <p>購物車是空的</p>
          <button @click="cartStore.closeCart" class="btn-continue">繼續購物</button>
        </div>

        <!-- 購物車商品列表 -->
        <div v-else>
          <div v-for="item in cartStore.cartItems" :key="item.product.id" class="cart-item">
            <div class="item-image">
              <img :src="item.product.imageUrl || '/placeholder.jpg'" :alt="item.product.name">
            </div>
            
            <div class="item-details">
              <h4>{{ item.product.name }}</h4>
              <p class="item-price">NT$ {{ item.product.price }}</p>
              
              <div class="quantity-controls">
                <button @click="updateQuantity(item.product.id, item.quantity - 1)" class="qty-btn">-</button>
                <span class="quantity">{{ item.quantity }}</span>
                <button @click="updateQuantity(item.product.id, item.quantity + 1)" class="qty-btn">+</button>
              </div>
            </div>
            
            <div class="item-total">
              <p>NT$ {{ (item.product.price * item.quantity).toFixed(2) }}</p>
              <button @click="removeItem(item.product.id)" class="remove-btn">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- 購物車底部 -->
      <div v-if="!cartStore.isCartEmpty" class="cart-footer">
        <div class="cart-summary">
          <div class="summary-row">
            <span>商品總數：</span>
            <span>{{ cartStore.totalItems }} 件</span>
          </div>
          <div class="summary-row total">
            <span>總金額：</span>
            <span>NT$ {{ cartStore.totalAmount.toFixed(2) }}</span>
          </div>
        </div>
        
        <div class="cart-actions">
          <button @click="clearCart" class="btn-clear">清空購物車</button>
          <button @click="checkout" class="btn-checkout">結帳</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useCartStore } from '@/stores/cart'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const cartStore = useCartStore()
const authStore = useAuthStore()
const router = useRouter()

const updateQuantity = (productId, quantity) => {
  cartStore.updateQuantity(productId, quantity)
}

const removeItem = (productId) => {
  cartStore.removeItem(productId)
}

const clearCart = () => {
  if (confirm('確定要清空購物車嗎？')) {
    cartStore.clearCart()
  }
}

const checkout = () => {
  if (!authStore.isAuthenticated) {
    alert('請先登入後再進行結帳')
    return
  }
  
  cartStore.closeCart()
  router.push('/checkout')
}
</script>

<style scoped>
.cart-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  justify-content: flex-end;
}

.cart-sidebar {
  width: 400px;
  max-width: 90vw;
  background: white;
  height: 100vh;
  display: flex;
  flex-direction: column;
  box-shadow: -2px 0 10px rgba(0, 0, 0, 0.1);
}

.cart-header {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.cart-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
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
  background-color: #f3f4f6;
}

.cart-content {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
}

.empty-cart {
  text-align: center;
  padding: 2rem;
}

.empty-icon {
  font-size: 3rem;
  color: #d1d5db;
  margin-bottom: 1rem;
}

.btn-continue {
  background-color: #2563eb;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-continue:hover {
  background-color: #1d4ed8;
}

.cart-item {
  display: flex;
  gap: 1rem;
  padding: 1rem 0;
  border-bottom: 1px solid #f3f4f6;
}

.item-image {
  width: 60px;
  height: 60px;
  flex-shrink: 0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 0.375rem;
}

.item-details {
  flex: 1;
}

.item-details h4 {
  margin: 0 0 0.25rem 0;
  font-size: 0.875rem;
  font-weight: 600;
}

.item-price {
  color: #6b7280;
  font-size: 0.875rem;
  margin: 0 0 0.5rem 0;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.qty-btn {
  width: 30px;
  height: 30px;
  border: 1px solid #d1d5db;
  background: white;
  border-radius: 0.25rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.qty-btn:hover {
  background-color: #f9fafb;
}

.quantity {
  padding: 0 0.5rem;
  font-weight: 600;
}

.item-total {
  text-align: right;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: flex-end;
}

.item-total p {
  margin: 0;
  font-weight: 600;
  color: #2563eb;
}

.remove-btn {
  background: none;
  border: none;
  color: #ef4444;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 0.25rem;
  transition: background-color 0.2s;
}

.remove-btn:hover {
  background-color: #fef2f2;
}

.cart-footer {
  border-top: 1px solid #e5e7eb;
  padding: 1rem;
}

.cart-summary {
  margin-bottom: 1rem;
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

.cart-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-clear {
  flex: 1;
  background-color: #ef4444;
  color: white;
  border: none;
  padding: 0.75rem;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-clear:hover {
  background-color: #dc2626;
}

.btn-checkout {
  flex: 2;
  background-color: #10b981;
  color: white;
  border: none;
  padding: 0.75rem;
  border-radius: 0.375rem;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.2s;
}

.btn-checkout:hover {
  background-color: #059669;
}
</style>
