<template>
  <MainLayout>
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-content">
        <h1>清涼一夏，暢快每一天</h1>
        <p>我們提供最優質的飲品，使用天然食材，讓您享受健康與美味的完美結合。</p>
        <router-link to="/products" class="btn">立即選購</router-link>
      </div>
    </section>

    <!-- 精選產品 -->
    <section class="container">
      <h2 class="section-title">熱門飲品</h2>
      
      <div v-if="productStore.loading" class="loading">
        載入中...
      </div>
      
      <div v-else-if="productStore.error" class="error">
        {{ productStore.error }}
      </div>
      
      <div v-else class="products">
        <ProductCard
          v-for="product in featuredProducts"
          :key="product.id"
          :product="product"
          @add-to-cart="handleAddToCart"
        />
      </div>
      
      <!-- 查看更多 -->
      <div class="view-more">
        <router-link to="/products" class="btn btn-secondary">查看所有產品</router-link>
      </div>
    </section>

    <!-- 特色區塊 -->
    <section class="features">
      <div class="container">
        <h2 class="section-title">為什麼選擇我們</h2>
        <div class="feature-list">
          <div class="feature-item">
            <div class="feature-icon">🍃</div>
            <h3>天然食材</h3>
            <p>我們只使用最優質的天然食材，不添加人工色素和防腐劑。</p>
          </div>
          
          <div class="feature-item">
            <div class="feature-icon">🥤</div>
            <h3>獨特配方</h3>
            <p>每一款飲品都經過精心調配，口感獨特，令人回味無窮。</p>
          </div>
          
          <div class="feature-item">
            <div class="feature-icon">🚚</div>
            <h3>快速配送</h3>
            <p>線上訂購後30分鐘內送達，保證新鮮美味。</p>
          </div>
        </div>
      </div>
    </section>
  </MainLayout>
</template>

<script setup>
import { onMounted, computed } from 'vue'
import { useProductStore } from '@/stores/product'
import { useCartStore } from '@/stores/cart'
import MainLayout from '@/layouts/MainLayout.vue'
import ProductCard from '@/components/ProductCard.vue'

const productStore = useProductStore()
const cartStore = useCartStore()

// 精選產品（前6個）
const featuredProducts = computed(() => 
  productStore.availableProducts.slice(0, 6)
)

onMounted(async () => {
  await productStore.fetchProducts()
})

const handleAddToCart = (product, quantity = 1) => {
  cartStore.addItem(product, quantity)
  alert(`${product.name} 已添加到購物車！`)
}
</script>

<style scoped>
.view-more {
  text-align: center;
  margin-top: 40px;
}

.features {
  background-color: #e6f7ff;
  padding: 60px 0;
}

.feature-list {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 30px;
}

.feature-item {
  flex: 1;
  min-width: 250px;
  text-align: center;
  padding: 20px;
}

.feature-icon {
  font-size: 3rem;
  color: #006699;
  margin-bottom: 20px;
}

@media (max-width: 768px) {
  .feature-list {
    flex-direction: column;
    align-items: center;
  }
  
  .feature-item {
    min-width: auto;
    max-width: 400px;
  }
}
</style>
