import { defineStore } from 'pinia'
import axios from 'axios'

const API_BASE_URL = 'http://localhost:5249/api/v1'

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
    isAdmin: (state) => state.user?.Role === 'admin' || state.user?.role === 'admin'
  },

  actions: {
    // 初始化認證狀態
    initAuth() {
      const token = localStorage.getItem('token')
      const user = localStorage.getItem('user')
      
      if (token && user && user !== 'undefined' && user !== 'null') {
        try {
          this.token = token
          this.user = JSON.parse(user)
          this.isAuthenticated = true
          
          // 設置 axios 默認 header
          axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
        } catch (error) {
          console.error('解析用戶數據時出錯:', error)
          // 清除無效數據
          this.logout()
        }
      }
    },

    // 登入
    async login(credentials) {
      this.loading = true
      this.error = null
      
      try {
        const response = await axios.post(`${API_BASE_URL}/auth/login`, credentials)
        
        // 根據後端實際的響應格式處理數據
        const { data } = response.data
        
        // 生成一個簡單的 token（實際項目中應該由後端生成JWT）
        const token = `token_${data.id}_${Date.now()}`
        
        this.token = token
        this.user = data
        this.isAuthenticated = true
        
        // 保存到 localStorage
        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify(data))
        
        // 設置 axios 默認 header
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
        
        return { 
          success: true, 
          user: data,
          redirectTo: data.role === 'admin' ? '/admin' : '/'
        }
      } catch (error) {
        this.error = error.response?.data?.message || '登入失敗'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    // 註冊
    async register(userData) {
      this.loading = true
      this.error = null
      
      try {
        // payload 轉換，後端需要 userName
        const payload = {
          UserName: userData.userName ?? userData.username,
          Password: userData.password,
          Email: userData.email,
          Address: userData.address,
          Phone: userData.phone,
          IsActive: userData.isActive
        }
        await axios.post(`${API_BASE_URL}/auth/register`, payload)
        // 註冊成功後自動登入
        return await this.login({
          username: payload.UserName,
          password: payload.Password
        })
      } catch (error) {
        this.error = error.response?.data?.message || '註冊失敗'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    // 登出
    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      this.error = null
      
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      
      delete axios.defaults.headers.common['Authorization']
    },

    // 清除錯誤
    clearError() {
      this.error = null
    }
  }
})
