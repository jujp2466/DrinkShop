import { defineStore } from 'pinia'

export const useCartStore = defineStore('cart', {
  state: () => ({
    items: [],
    isOpen: false
  }),

  getters: {
    totalItems: (state) => state.items.reduce((total, item) => total + item.quantity, 0),
    
    totalAmount: (state) => state.items.reduce((total, item) => {
      return total + (item.product.price * item.quantity)
    }, 0),
    
    cartItems: (state) => state.items,
    
    isCartEmpty: (state) => state.items.length === 0
  },

  actions: {
    // 初始化購物車
    initCart() {
      const savedCart = localStorage.getItem('cart')
      if (savedCart) {
        this.items = JSON.parse(savedCart)
      }
    },

    // 添加商品到購物車
    addItem(product, quantity = 1) {
      const existingItem = this.items.find(item => item.product.id === product.id)
      
      if (existingItem) {
        existingItem.quantity += quantity
      } else {
        this.items.push({
          product,
          quantity
        })
      }
      
      this.saveToLocalStorage()
    },

    // 更新商品數量
    updateQuantity(productId, quantity) {
      const item = this.items.find(item => item.product.id === productId)
      if (item) {
        if (quantity <= 0) {
          this.removeItem(productId)
        } else {
          item.quantity = quantity
          this.saveToLocalStorage()
        }
      }
    },

    // 移除商品
    removeItem(productId) {
      this.items = this.items.filter(item => item.product.id !== productId)
      this.saveToLocalStorage()
    },

    // 清空購物車
    clearCart() {
      this.items = []
      this.saveToLocalStorage()
    },

    // 開啟/關閉購物車
    toggleCart() {
      this.isOpen = !this.isOpen
    },

    // 關閉購物車
    closeCart() {
      this.isOpen = false
    },

    // 保存到 localStorage
    saveToLocalStorage() {
      localStorage.setItem('cart', JSON.stringify(this.items))
    }
  }
})
