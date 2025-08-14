import axios from 'axios';

export default axios.create({
  baseURL: 'http://localhost:5249/api/v1', // 請依實際後端 API 調整
});
