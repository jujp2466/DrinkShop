import { createRouter, createWebHistory } from 'vue-router';
import OrderPage from '../components/OrderPage.vue';
import DrinkCrud from '../components/DrinkCrud.vue';

const routes = [
  { path: '/', component: OrderPage },
  { path: '/drinkcrud', component: DrinkCrud },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
