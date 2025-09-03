<template>
  <MainLayout>
    <div class="checkout-page">
      <!-- 頁面標題 -->
      <div class="page-header">
        <h1>結帳</h1>
        <p>確認您的訂單並完成付款</p>
      </div>

      <div class="container">
        <div class="checkout-content">
          <!-- 訂單摘要 -->
          <div class="order-summary">
            <h2>訂單摘要</h2>
            
            <div v-if="cartStore.isCartEmpty" class="empty-cart">
              <i class="fas fa-shopping-cart empty-icon"></i>
              <p>購物車是空的</p>
              <router-link to="/products" class="btn btn-primary">前往購物</router-link>
            </div>

            <div v-else>
              <!-- 商品列表 -->
              <div class="order-items">
                <div v-for="item in cartStore.cartItems" :key="item.product.id" class="order-item">
                  <div class="item-image">
                    <img :src="item.product.imageUrl || '/placeholder.jpg'" :alt="item.product.name">
                  </div>
                  <div class="item-details">
                    <h4>{{ item.product.name }}</h4>
                    <p class="item-price">NT$ {{ item.product.price }} x {{ item.quantity }}</p>
                  </div>
                  <div class="item-total">
                    NT$ {{ (item.product.price * item.quantity).toFixed(2) }}
                  </div>
                </div>
              </div>

              <!-- 總計 -->
              <div class="order-total">
                <div class="total-row">
                  <span>小計：</span>
                  <span>NT$ {{ cartStore.totalAmount.toFixed(2) }}</span>
                </div>
                <div class="total-row">
                  <span>運費：</span>
                  <span>NT$ {{ shippingFee.toFixed(2) }}</span>
                </div>
                <div class="total-row grand-total">
                  <span>總計：</span>
                  <span>NT$ {{ (cartStore.totalAmount + shippingFee).toFixed(2) }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- 結帳表單 -->
          <div v-if="!cartStore.isCartEmpty" class="checkout-form">
            <h2>配送資訊</h2>
            
            <form @submit.prevent="submitOrder" class="form">
              <!-- 收件人資訊 -->
              <div class="form-section">
                <h3>收件人資訊</h3>
                
                <div class="form-row">
                  <div class="form-group">
                    <label for="customerName">姓名 *</label>
                    <input
                      id="customerName"
                      v-model="orderForm.customerName"
                      type="text"
                      required
                      class="form-input"
                      placeholder="請輸入收件人姓名"
                    >
                  </div>
                  
                  <div class="form-group">
                    <label for="customerPhone">電話 *</label>
                    <input
                      id="customerPhone"
                      v-model="orderForm.customerPhone"
                      type="tel"
                      required
                      class="form-input"
                      placeholder="請輸入聯絡電話"
                    >
                  </div>
                </div>

                <div class="form-group">
                  <label for="customerEmail">電子郵件</label>
                  <input
                    id="customerEmail"
                    v-model="orderForm.customerEmail"
                    type="email"
                    class="form-input"
                    placeholder="請輸入電子郵件"
                  >
                </div>
              </div>

              <!-- 配送地址 -->
              <div class="form-section">
                <h3>配送地址</h3>
                
                <div class="form-group">
                  <label for="shippingAddress">地址</label>
                  <textarea
                    id="shippingAddress"
                    v-model="orderForm.shippingAddress"
                    rows="3"
                    class="form-input"
                    placeholder="請輸入詳細地址"
                  ></textarea>
                </div>
              </div>

              <!-- 付款方式 -->
              <div class="form-section">
                <h3>付款方式</h3>
                
                <div class="payment-options">
                  <label class="payment-option">
                    <input
                      v-model="orderForm.paymentMethod"
                      type="radio"
                      value="credit-card"
                      name="payment"
                    >
                    <span class="radio-custom"></span>
                    <i class="fas fa-credit-card"></i>
                    信用卡
                  </label>
                  
                  <label class="payment-option">
                    <input
                      v-model="orderForm.paymentMethod"
                      type="radio"
                      value="bank-transfer"
                      name="payment"
                    >
                    <span class="radio-custom"></span>
                    <i class="fas fa-university"></i>
                    銀行轉帳
                  </label>
                  
                  <label class="payment-option">
                    <input
                      v-model="orderForm.paymentMethod"
                      type="radio"
                      value="cash-on-delivery"
                      name="payment"
                    >
                    <span class="radio-custom"></span>
                    <i class="fas fa-money-bill-wave"></i>
                    貨到付款
                  </label>
                </div>
              </div>

              <!-- 備註 -->
              <div class="form-section">
                <h3>訂單備註</h3>
                <div class="form-group">
                  <textarea
                    v-model="orderForm.notes"
                    rows="3"
                    class="form-input"
                    placeholder="有任何特殊需求請在此註明"
                  ></textarea>
                </div>
              </div>

              <!-- 提交按鈕 -->
              <div class="form-actions">
                <button type="button" @click="$router.push('/products')" class="btn btn-secondary">
                  繼續購物
                </button>
                <button type="submit" :disabled="isSubmitting" class="btn btn-primary">
                  {{ isSubmitting ? '處理中...' : '確認訂單' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { useAuthStore } from '@/stores/auth'
import MainLayout from '@/layouts/MainLayout.vue'
import api from '@/api'

const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()

const isSubmitting = ref(false)
const shippingFee = ref(60) // 固定運費

// 訂單表單
const orderForm = reactive({
  customerName: authStore.currentUser?.username || '',
  customerPhone: '',
  customerEmail: authStore.currentUser?.email || '',
  shippingAddress: '',
  paymentMethod: 'credit-card',
  notes: ''
})

// 提交訂單
const submitOrder = async () => {
  isSubmitting.value = true
  
  try {
    // 如果用戶已登入，先更新用戶資訊
    if (authStore.currentUser?.Id) {
      const userUpdateData = {
        name: orderForm.customerName,
        phone: orderForm.customerPhone,
        email: orderForm.customerEmail,
        address: orderForm.shippingAddress
      }
      
      try {
        await api.put(`/users/${authStore.currentUser.Id}`, userUpdateData)
      } catch (userUpdateError) {
        console.warn('更新用戶資訊失敗：', userUpdateError)
        // 繼續提交訂單，不因為用戶資訊更新失敗而中斷
      }
    }
    
    // 構建訂單數據（配合第三正規化結構）
    const orderData = {
      userId: authStore.currentUser?.id ? Number(authStore.currentUser.id) : 1, // 如果沒有登入，使用訪客用戶ID
      totalAmount: cartStore.totalAmount + shippingFee.value,
      shippingFee: shippingFee.value,
      shippingAddress: orderForm.shippingAddress,
      paymentMethod: orderForm.paymentMethod,
      notes: orderForm.notes,
      items: cartStore.cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.quantity,
        price: item.product.price
      }))
    }

    // 呼叫後端 API 創建訂單
    const response = await api.post('/orders', orderData)
    
    if (response.data?.code === 200) {
      alert('訂單已成功提交！我們會盡快為您處理。')
      
      // 清空購物車
      cartStore.clearCart()
      
      // 導向首頁
      router.push('/')
    } else {
      throw new Error('訂單創建失敗')
    }
    
  } catch (error) {
    console.error('訂單提交失敗:', error)
    alert('訂單提交失敗，請稍後再試。')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.checkout-page {
  padding: 2rem 0;
  min-height: 100vh;
  background-color: #f9fafb;
}

.page-header {
  text-align: center;
  margin-bottom: 3rem;
  padding: 2rem 0;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
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

.checkout-content {
  display: grid;
  grid-template-columns: 1fr 1.5fr;
  gap: 3rem;
  max-width: 1200px;
  margin: 0 auto;
}

.order-summary, .checkout-form {
  background: white;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  height: fit-content;
}

.order-summary h2, .checkout-form h2 {
  font-size: 1.5rem;
  color: #1f2937;
  margin-bottom: 1.5rem;
  font-weight: 600;
  border-bottom: 2px solid #e5e7eb;
  padding-bottom: 0.5rem;
}

.empty-cart {
  text-align: center;
  padding: 3rem 1rem;
}

.empty-icon {
  font-size: 3rem;
  color: #d1d5db;
  margin-bottom: 1rem;
  display: block;
}

.order-items {
  margin-bottom: 2rem;
}

.order-item {
  display: flex;
  align-items: center;
  padding: 1rem 0;
  border-bottom: 1px solid #f3f4f6;
}

.order-item:last-child {
  border-bottom: none;
}

.item-image {
  width: 60px;
  height: 60px;
  margin-right: 1rem;
  flex-shrink: 0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 0.5rem;
}

.item-details {
  flex: 1;
}

.item-details h4 {
  font-size: 1rem;
  color: #1f2937;
  margin-bottom: 0.25rem;
  font-weight: 600;
}

.item-price {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0;
}

.item-total {
  font-weight: 600;
  color: #2563eb;
}

.order-total {
  border-top: 2px solid #e5e7eb;
  padding-top: 1rem;
}

.total-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.grand-total {
  font-size: 1.25rem;
  font-weight: 700;
  color: #2563eb;
  border-top: 1px solid #e5e7eb;
  padding-top: 0.5rem;
  margin-top: 1rem;
}

.form-section {
  margin-bottom: 2rem;
  padding-bottom: 2rem;
  border-bottom: 1px solid #f3f4f6;
}

.form-section:last-child {
  border-bottom: none;
}

.form-section h3 {
  font-size: 1.25rem;
  color: #374151;
  margin-bottom: 1rem;
  font-weight: 600;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
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
  transition: border-color 0.2s, box-shadow 0.2s;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.payment-options {
  display: grid;
  gap: 1rem;
}

.payment-option {
  display: flex;
  align-items: center;
  padding: 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.payment-option:hover {
  border-color: #2563eb;
  background-color: #f8fafc;
}

.payment-option input[type="radio"] {
  position: absolute;
  opacity: 0;
}

.payment-option input[type="radio"]:checked + .radio-custom {
  background-color: #2563eb;
  border-color: #2563eb;
}

.payment-option input[type="radio"]:checked + .radio-custom::after {
  opacity: 1;
}

.payment-option input[type="radio"]:checked ~ * {
  color: #2563eb;
}

.radio-custom {
  width: 20px;
  height: 20px;
  border: 2px solid #d1d5db;
  border-radius: 50%;
  margin-right: 0.75rem;
  position: relative;
  transition: all 0.2s;
}

.radio-custom::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 8px;
  height: 8px;
  background-color: white;
  border-radius: 50%;
  opacity: 0;
  transition: opacity 0.2s;
}

.payment-option i {
  margin-right: 0.5rem;
  font-size: 1.125rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 2px solid #e5e7eb;
}

.btn {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn-primary {
  background: linear-gradient(135deg, #2563eb, #1d4ed8);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: linear-gradient(135deg, #1d4ed8, #1e40af);
  transform: translateY(-1px);
}

.btn-primary:disabled {
  background: #9ca3af;
  cursor: not-allowed;
  transform: none;
}

.btn-secondary {
  background: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background: #4b5563;
  transform: translateY(-1px);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .checkout-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .order-summary, .checkout-form {
    padding: 1.5rem;
  }
}

@media (max-width: 480px) {
  .checkout-content {
    gap: 1rem;
  }

  .order-summary, .checkout-form {
    padding: 1rem;
  }

  .order-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .item-image {
    margin-right: 0;
  }
}
</style>
