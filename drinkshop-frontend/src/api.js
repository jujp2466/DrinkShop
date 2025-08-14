import axios from 'axios';

export default axios.create({
  baseURL: 'https://drinkshop-c5ccheftavfvh0av.japaneast-01.azurewebsites.net/api/v1', // 已改為 Azure App Service 網域
});
