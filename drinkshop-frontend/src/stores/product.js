import { defineStore } from 'pinia'
import axios from 'axios'
import api from '../api'

export const useProductStore = defineStore('product', {
  state: () => ({
    products: [],
    currentProduct: null,
    loading: false,
    error: null
  }),

  getters: {
    availableProducts: (state) => {
      // 確保 products 是陣列
      if (!Array.isArray(state.products)) {
        return []
      }
      return state.products.filter(product => product.isActive)
    },
    productById: (state) => (id) => {
      if (!Array.isArray(state.products)) {
        return null
      }
      return state.products.find(product => product.id === id)
    }
  },

  actions: {
    // 獲取所有產品
    async fetchProducts() {
      this.loading = true
      this.error = null
      
      try {
  const response = await api.get(`/products`)
        // API 返回的格式是 {code: 200, message: "Success", data: [...]}
        this.products = response.data.data || []
      } catch (error) {
        this.error = error.response?.data?.message || '獲取產品失敗'
        console.error('Fetch products error:', error)
      } finally {
        this.loading = false
      }
    },

    // 獲取單個產品
    async fetchProduct(id) {
      this.loading = true
      this.error = null
      
      try {
  const response = await api.get(`/products/${id}`)
        // API 返回的格式是 {code: 200, message: "Success", data: {...}}
        this.currentProduct = response.data.data || null
        return response.data.data
      } catch (error) {
        this.error = error.response?.data?.message || '獲取產品失敗'
        console.error('Fetch product error:', error)
      } finally {
        this.loading = false
      }
    },

    // 創建產品（管理員）
    async createProduct(productData) {
      this.loading = true
      this.error = null
      
      try {
  const response = await api.post(`/products`, productData)
        const newProduct = response.data.data || response.data
        // 確保 products 是陣列
        if (Array.isArray(this.products)) {
          this.products.push(newProduct)
        }
        return { success: true, data: newProduct }
      } catch (error) {
        this.error = error.response?.data?.message || '創建產品失敗'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    // 更新產品（管理員）
    async updateProduct(id, productData) {
      this.loading = true
      this.error = null
      
      try {
  const response = await api.put(`/products/${id}`, productData)
        const updatedProduct = response.data.data || response.data
        // 確保 products 是陣列
        if (Array.isArray(this.products)) {
          const index = this.products.findIndex(product => product.id === id)
          if (index !== -1) {
            this.products[index] = updatedProduct
          }
        }
        return { success: true, data: updatedProduct }
      } catch (error) {
        this.error = error.response?.data?.message || '更新產品失敗'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    // 刪除產品（管理員）
    async deleteProduct(id) {
      this.loading = true
      this.error = null
      
      try {
  await api.delete(`/products/${id}`)
        // 確保 products 是陣列
        if (Array.isArray(this.products)) {
          this.products = this.products.filter(product => product.id !== id)
        }
        return { success: true }
      } catch (error) {
        this.error = error.response?.data?.message || '刪除產品失敗'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    // 清除錯誤
    clearError() {
      this.error = null
    }
  }
})
