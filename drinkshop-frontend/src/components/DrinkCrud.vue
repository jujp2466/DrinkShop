<template>
  <div class="drinkshop-container">
    <h1 class="title">Drink Shop 菜單管理</h1>
    <form class="drink-form" @submit.prevent="addDrink">
      <div class="form-group">
        <label>飲品名稱</label>
        <input v-model="newDrink.name" placeholder="請輸入飲品名稱" required />
      </div>
      <div class="form-group">
        <label>價格</label>
        <input v-model.number="newDrink.price" placeholder="請輸入價格" type="number" min="0" required />
      </div>
      <div class="form-group">
        <label>數量</label>
        <input v-model.number="newDrink.quantity" placeholder="請輸入數量" type="number" min="0" required />
      </div>
      <button class="add-btn" type="submit">新增飲品</button>
    </form>
    <div v-if="successMsg" class="success-msg">{{ successMsg }}</div>
    <ul class="drink-list">
      <li v-for="d in drinks" :key="d.id" class="drink-item">
        <span class="drink-name">{{ d.name }}</span>
        <span class="drink-price">${{ d.price }}</span>
        <span class="drink-quantity">數量: {{ d.quantity }}</span>
        <button class="delete-btn" @click="removeDrink(d.id)">刪除</button>
      </li>
    </ul>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import api from '../api';
const drinks = ref([]);
const newDrink = ref({ name: '', price: 0, quantity: 0 });
const successMsg = ref('');
const fetchDrinks = async () => {
  try {
    const res = await api.get('/drink');
    drinks.value = res.data.data || [];
  } catch {
    drinks.value = [];
  }
};
onMounted(fetchDrinks);
const addDrink = async () => {
  if (!newDrink.value.name || newDrink.value.price < 0 || newDrink.value.quantity < 0) return;
  try {
    await api.post('/drink', newDrink.value);
    successMsg.value = '新增成功！';
    setTimeout(() => (successMsg.value = ''), 1500);
    newDrink.value = { name: '', price: 0, quantity: 0 };
    fetchDrinks();
  } catch {
    successMsg.value = '新增失敗，請檢查後端 API';
    setTimeout(() => (successMsg.value = ''), 2000);
  }
};
const removeDrink = async (id) => {
  await api.delete(`/drink/${id}`);
  fetchDrinks();
};
</script>
<style scoped>
.drinkshop-container {
  max-width: 480px;
  margin: 40px auto;
  background: #fffbe6;
  border-radius: 16px;
  box-shadow: 0 2px 16px #e0c97f44;
  padding: 32px 24px 24px 24px;
  font-family: 'Segoe UI', '微軟正黑體', Arial, sans-serif;
}
.title {
  text-align: center;
  color: #b8860b;
  margin-bottom: 24px;
  letter-spacing: 2px;
}
.drink-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 16px;
}
.form-group {
  display: flex;
  flex-direction: column;
}
.form-group label {
  font-weight: bold;
  color: #8b5c00;
  margin-bottom: 4px;
}
.add-btn {
  background: linear-gradient(90deg, #ffe066, #ffd700);
  color: #8b5c00;
  border: none;
  border-radius: 6px;
  padding: 8px 0;
  font-size: 1.1em;
  font-weight: bold;
  cursor: pointer;
  margin-top: 8px;
  transition: background 0.2s;
}
.add-btn:hover {
  background: linear-gradient(90deg, #ffd700, #ffe066);
}
.success-msg {
  color: #388e3c;
  background: #e6ffe6;
  border: 1px solid #b2ffb2;
  border-radius: 4px;
  padding: 6px 0;
  text-align: center;
  margin-bottom: 10px;
}
.drink-list {
  list-style: none;
  padding: 0;
}
.drink-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #fff8dc;
  border-radius: 6px;
  margin-bottom: 8px;
  padding: 10px 14px;
  box-shadow: 0 1px 4px #e0c97f22;
}
.drink-name {
  font-weight: bold;
  color: #b8860b;
}
.drink-price {
  color: #8b5c00;
  margin-left: 12px;
  margin-right: 12px;
}
.drink-quantity {
  color: #8b5c00;
  margin-left: 12px;
}
.delete-btn {
  background: #ffb300;
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 4px 10px;
  cursor: pointer;
  font-size: 0.95em;
  transition: background 0.2s;
}
.delete-btn:hover {
  background: #ff7043;
}
</style>
