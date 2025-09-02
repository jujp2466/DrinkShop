<template>
  <div class="order-page">
    <header class="order-header">
      <div class="logo">淼淼飲品</div>
      <p>您的最佳飲料選擇</p>
    </header>
    <nav class="order-nav">
      <ul>
        <li><a href="#home">首頁</a></li>
        <li><a href="#about">關於我們</a></li>
        <li><a href="#contact">聯絡我們</a></li>
  <li><router-link to="/drinkcrud">管理菜單</router-link></li>
      </ul>
    </nav>
    <section id="home" class="hero">
      <div class="hero-content">
        <h1>清涼一夏，暢快每一天</h1>
        <p>我們提供最優質的飲品，使用天然食材，讓您享受健康與美味的完美結合。</p>
        <a href="#products" class="btn">立即選購</a>
      </div>
    </section>
    <section id="products" class="container">
      <h2 class="section-title">熱門飲品</h2>
      <div class="products">
        <div v-for="drink in drinks" :key="drink.id" class="product-card">
          <div class="product-image">
            <img :src="imageSrc(drink)" :alt="drink.name" />
          </div>
          <div class="product-info">
            <h3 class="product-title">{{ drink.name }}</h3>
            <p class="product-description">{{ drink.description || '新鮮美味，清涼解渴' }}</p>
            <p class="product-price">NT$ {{ drink.price }}</p>
            <p class="product-purchased">購買次數: {{ (drink.PurchaseCount ?? drink.purchaseCount) ?? 0 }}</p>
            <div class="quantity-selector">
              <label for="quantity">數量：</label>
              <input type="number" v-model.number="quantities[drink.id]" min="1" :id="'quantity-' + drink.id" />
            </div>
            <button class="btn" @click="addToCart(drink, quantities[drink.id])">加入購物車</button>
          </div>
        </div>
      </div>
    </section>
    <section class="cart-section" v-if="cart.length > 0">
      <h2 class="section-title">購物車</h2>
      <ul class="cart-list">
        <li v-for="item in cart" :key="item.id" class="cart-item">
          <span>{{ item.name }}</span>
          <span>數量: {{ item.quantity }}</span> <!-- 顯示數量 -->
          <span>購買次數: {{ (item.PurchaseCount ?? item.purchaseCount) ?? 0 }}</span>
          <span>NT$ {{ item.price * item.quantity }}</span> <!-- 更新金額計算 -->
          <button class="delete-btn" @click="removeFromCart(item.id)">移除</button>
        </li>
      </ul>
      <div class="cart-total">
        總金額：NT$ {{ totalPrice }}
      </div>
      <button class="checkout-btn" @click="checkout">結帳</button>
    </section>
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
    <footer>
      <div class="footer-content">
        <div class="footer-section">
          <h3 class="footer-title">關於我們</h3>
          <p>淼淼飲品成立於2025年，致力於提供最優質的飲品服務，讓每位顧客都能享受到健康美味的飲品體驗。</p>
        </div>
        <div class="footer-section">
          <h3 class="footer-title">聯絡資訊</h3>
          <p>地址：基隆中山區中山路123號</p>
          <p>電話：0912-345-678</p>
          <p>Email：info@cooldrinks.com</p>
        </div>
        <div class="footer-section">
          <h3 class="footer-title">營業時間</h3>
          <p>週一至週五：10:00 - 21:00</p>
          <p>週六至週日：11:00 - 22:00</p>
        </div>
        <div class="footer-section">
          <h3 class="footer-title">快速連結</h3>
          <div class="footer-links">
            <a href="#home">首頁</a>
            <a href="#about">關於我們</a>
            <a href="#contact">聯絡我們</a>
          </div>
        </div>
      </div>
      <div class="copyright">
        <p>&copy; 2025 淼淼飲品 版權所有</p>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import api from '../api';

const drinks = ref([]);
const cart = ref([]);
const quantities = ref({});

const fetchDrinks = async () => {
  try {
    const res = await api.get('/drink');
    drinks.value = res.data.data || [];
    drinks.value.forEach((drink) => {
      quantities.value[drink.id] = 1; // 初始化每個飲品的數量為 1
    });
  } catch {
    drinks.value = [];
  }
};

const addToCart = (drink, quantity = 1) => {
  const existingItem = cart.value.find((item) => item.id === drink.id);
  if (existingItem) {
    existingItem.quantity += quantity;
  } else {
    cart.value.push({ ...drink, quantity });
  }
};

const totalPrice = computed(() =>
  cart.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
);

const removeFromCart = (id) => {
  cart.value = cart.value.filter((item) => item.id !== id);
};

const checkout = async () => {
  try {
    const res = await api.post('/drink/checkout', cart.value);
    if (res.status === 200) {
      alert('結帳成功！感謝您的訂購！');
      cart.value = [];
    } else {
      alert('結帳失敗，請稍後再試！');
    }
  } catch (error) {
    console.error('結帳失敗:', error);
    alert('結帳失敗，請稍後再試！');
  }
};

onMounted(() => {
  fetchDrinks();
});

// 統一圖片來源處理：支援 imageUrl / image / 相對路徑 與預設圖；茶類優先用茶圖
const imageSrc = (item) => {
  return item?.imageUrl || item?.image || ''
}
</script>

<style>
/* 參考 cursor.html 設計，優化美觀 */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: 'Arial', '微軟正黑體', sans-serif;
}
body, .order-page {
  background-color: #f5f5f5;
  color: #333;
}
.order-header {
  background: linear-gradient(135deg, #0099cc, #006699);
  color: white;
  padding: 20px 0;
  text-align: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}
.logo {
  font-size: 2.5rem;
  font-weight: bold;
  margin-bottom: 10px;
}
.order-nav {
  background-color: #006699;
  padding: 15px 0;
}
.order-nav ul {
  display: flex;
  justify-content: center;
  list-style: none;
}
.order-nav ul li {
  margin: 0 15px;
}
.order-nav ul li a {
  color: white;
  text-decoration: none;
  font-weight: bold;
  transition: color 0.3s;
}
.order-nav ul li a:hover {
  color: #99ccff;
}
.hero {
  background-image: url('https://images.unsplash.com/photo-1657759558425-a0f43e577432?fm=jpg&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&ixlib=rb-4.1.0&q=60&w=3000');
  background-size: cover;
  background-position: center;
  height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  color: white;
  position: relative;
}
.hero::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
}
.hero-content {
  position: relative;
  z-index: 1;
  max-width: 800px;
  padding: 20px;
}
.hero h1 {
  font-size: 2.5rem;
  margin-bottom: 20px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}
.hero p {
  font-size: 1.2rem;
  margin-bottom: 30px;
}
.btn {
  display: inline-block;
  background-color: #ff9900;
  color: white;
  padding: 12px 30px;
  border-radius: 30px;
  text-decoration: none;
  font-weight: bold;
  transition: background-color 0.3s;
}
.btn:hover {
  background-color: #ff6600;
}
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 40px 20px;
}
.section-title {
  text-align: center;
  margin-bottom: 40px;
  font-size: 2rem;
  color: #006699;
}
.products {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 30px;
}
.product-card {
  background-color: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s;
}
.product-card:hover {
  transform: translateY(-10px);
}
.product-image {
  height: 250px;
  overflow: hidden;
}
.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s;
}
.product-card:hover .product-image img {
  transform: scale(1.1);
}
.product-info {
  padding: 20px;
}
.product-title {
  font-size: 1.5rem;
  margin-bottom: 10px;
  color: #006699;
}
.product-description {
  color: #666;
  margin-bottom: 15px;
}
.product-price {
  font-size: 1.2rem;
  font-weight: bold;
  color: #ff6600;
  margin-bottom: 15px;
}
.quantity-selector {
  margin-bottom: 10px;
}
.quantity-selector label {
  margin-right: 5px;
  font-weight: bold;
}
.quantity-selector input {
  width: 50px;
  padding: 5px;
  text-align: center;
}
.cart-section {
  background: #e6f7ff;
  padding: 40px 20px;
  border-radius: 10px;
  margin: 40px auto;
  max-width: 600px;
  box-shadow: 0 2px 10px #99ccff44;
}
.cart-list {
  list-style: none;
  padding: 0;
}
.cart-item {
  display: flex;
  justify-content: space-between;
  background: white;
  padding: 10px;
  border-radius: 5px;
  margin-bottom: 10px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}
.cart-total {
  text-align: right;
  font-size: 1.2rem;
  font-weight: bold;
  margin-top: 10px;
}
.checkout-btn {
  display: block;
  margin: 20px auto;
  background-color: #0099cc;
  color: white;
  padding: 10px 30px;
  border-radius: 20px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s;
}
.checkout-btn:hover {
  background-color: #006699;
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
footer {
  background-color: #333;
  color: white;
  padding: 40px 0;
  text-align: center;
}
.footer-content {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}
.footer-section {
  flex: 1;
  min-width: 250px;
  margin-bottom: 30px;
}
.footer-title {
  font-size: 1.2rem;
  margin-bottom: 20px;
  color: #99ccff;
}
.footer-links a {
  color: #ccc;
  text-decoration: none;
  display: block;
  margin-bottom: 10px;
  transition: color 0.3s;
}
.footer-links a:hover {
  color: white;
}
.copyright {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #555;
}
@media (max-width: 768px) {
  .hero h1 {
    font-size: 2rem;
  }
  .order-nav ul {
    flex-direction: column;
    align-items: center;
  }
  .order-nav ul li {
    margin: 5px 0;
  }
  .products {
    grid-template-columns: 1fr;
  }
  .container {
    padding: 20px 10px;
  }
}
</style>
