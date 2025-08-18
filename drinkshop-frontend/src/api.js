import axios from 'axios';

const isLocal = window.location.hostname === 'localhost';

export default axios.create({
  baseURL: isLocal
    ? 'http://localhost:5249/api/v1' // 本機測試用
    : 'https://drinkshop-c5ccheftavfvh0av.japaneast-01.azurewebsites.net/api/v1', // Azure 用
});
