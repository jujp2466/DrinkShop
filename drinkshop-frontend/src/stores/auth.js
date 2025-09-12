import { defineStore } from 'pinia';
import { jwtDecode } from 'jwt-decode';
import api from '../api';
import { useCartStore } from './cart';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: null,
    isAuthenticated: false,
    loading: false,
    error: null
  }),

  getters: {
    currentUser: (state) => state.user,
    isLoggedIn: (state) => state.isAuthenticated,
  isAdmin: (state) => {
    const role = state.user?.role ?? state.user?.roles ?? '';
    if (Array.isArray(role)) {
      return role.map(r => r.toLowerCase()).includes('admin');
    }
    return (role || '').toLowerCase() === 'admin';
  }
  },

  actions: {
    // 初始化認證狀態
    initAuth() {
      const token = localStorage.getItem('token');
      const user = localStorage.getItem('user');
      
      if (token && user && user !== 'undefined' && user !== 'null') {
        try {
          this.token = token;
          this.user = JSON.parse(user);
          this.isAuthenticated = true;
          
          // 設置 api 實例的默認 header
          api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        } catch (error) {
          console.error('解析用戶數據時出錯:', error);
          // 清除無效數據
          this.logout();
        }
      }
    },

    // 登入
    async login(credentials) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await api.post(`/auth/login`, credentials);
        
        const { token } = response.data.data;

        if (!token) {
          throw new Error("未收到 Token");
        }
        
        this.token = token;
        
        // 解碼 JWT 以獲取用戶資訊
        const decodedToken = jwtDecode(token);
        
        // 檢查所有可能的角色欄位
        const roleField = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || 
                         decodedToken.role ||
                         '';
        
        // 標準化使用者物件
        this.user = {
          id: decodedToken.sub,
          userName: decodedToken.name,
          role: roleField,
        };
        
        this.isAuthenticated = true;
        
        // 保存到 localStorage
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(this.user));
        
        // 設置 api 實例的默認 header
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        
        return { 
          success: true, 
          user: this.user,
          redirectTo: this.user.role === 'admin' ? '/admin' : '/'
        };
      } catch (error) {
        this.error = error.response?.data?.message || '登入失敗';
        return { success: false, error: this.error };
      } finally {
        this.loading = false;
      }
    },

    // 註冊
    async register(userData) {
      this.loading = true
      this.error = null
      
      try {
        // payload 轉換，後端需要 userName
        const payload = {
          userName: userData.userName ?? userData.username,
          password: userData.password,
          email: userData.email,
          address: userData.address,
          phone: userData.phone,
          isActive: userData.isActive
        }
        const response = await api.post('/auth/register', payload)

        // 註冊成功後自動登入（使用 camelCase 欄位）
        return await this.login({
          userName: payload.userName,
          password: payload.password
        })
      } catch (error) {
        console.error('Register API call failed:', error);
        this.error = error.response?.data?.message || '註冊失敗'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    // 登出
    logout() {
      this.user = null;
      this.token = null;
      this.isAuthenticated = false;
      this.error = null;
      
      // 清除 localStorage
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      
      // 移除 api 實例的默認 header
      delete api.defaults.headers.common['Authorization'];
      // 清空購物車
      try {
        const cartStore = useCartStore();
        cartStore.clearCart();
      } catch (e) {
        localStorage.removeItem('cart');
      }
    },

    // 清除錯誤
    clearError() {
      this.error = null
    }
  }
})
