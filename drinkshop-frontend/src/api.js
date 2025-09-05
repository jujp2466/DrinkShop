import axios from 'axios';

// 優先使用 build 時注入的 VITE_API_BASE_URL（在 Vite / Azure 部署時可設定），否則依 hostname 判斷
const envBase = import.meta.env.VITE_API_BASE_URL;
const isLocal = window.location.hostname === 'localhost';

const baseURL = envBase
  ? envBase.replace(/\/+$/, '') // 移除尾端斜線
  : isLocal
  ? 'http://localhost:5249/api/v1'
  : 'https://drinkshop-c5ccheftavfvh0av.japaneast-01.azurewebsites.net/api/v1';

const api = axios.create({ baseURL });
export default api;
export { api };
