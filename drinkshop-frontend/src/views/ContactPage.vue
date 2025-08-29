<template>
  <MainLayout>
    <div class="contact-page">
      <!-- 頁面標題 -->
      <div class="page-header">
        <h1>聯絡我們</h1>
        <p>有任何問題或建議，歡迎與我們聯絡</p>
      </div>

      <div class="container">
        <div class="contact-content">
          <!-- 聯絡表單 -->
          <div class="contact-form-section">
            <h2>發送訊息</h2>
            <form @submit.prevent="submitForm" class="contact-form">
              <div class="form-row">
                <div class="form-group">
                  <label for="name">姓名 *</label>
                  <input
                    id="name"
                    v-model="form.name"
                    type="text"
                    required
                    class="form-input"
                    placeholder="請輸入您的姓名"
                  >
                </div>
                
                <div class="form-group">
                  <label for="email">電子郵件 *</label>
                  <input
                    id="email"
                    v-model="form.email"
                    type="email"
                    required
                    class="form-input"
                    placeholder="請輸入您的電子郵件"
                  >
                </div>
              </div>

              <div class="form-group">
                <label for="phone">電話號碼</label>
                <input
                  id="phone"
                  v-model="form.phone"
                  type="tel"
                  class="form-input"
                  placeholder="請輸入您的電話號碼"
                >
              </div>

              <div class="form-group">
                <label for="subject">主旨 *</label>
                <select
                  id="subject"
                  v-model="form.subject"
                  required
                  class="form-input"
                >
                  <option value="">請選擇主旨</option>
                  <option value="product-inquiry">產品諮詢</option>
                  <option value="order-issue">訂單問題</option>
                  <option value="suggestion">意見建議</option>
                  <option value="complaint">客訴處理</option>
                  <option value="cooperation">合作洽談</option>
                  <option value="other">其他</option>
                </select>
              </div>

              <div class="form-group">
                <label for="message">訊息內容 *</label>
                <textarea
                  id="message"
                  v-model="form.message"
                  required
                  rows="6"
                  class="form-input"
                  placeholder="請詳細描述您的問題或建議..."
                ></textarea>
              </div>

              <button type="submit" :disabled="isSubmitting" class="submit-btn">
                {{ isSubmitting ? '發送中...' : '發送訊息' }}
              </button>
            </form>
          </div>

          <!-- 聯絡資訊 -->
          <div class="contact-info-section">
            <h2>聯絡資訊</h2>
            
            <div class="contact-info">
              <div class="info-item">
                <div class="info-icon">
                  <i class="fas fa-map-marker-alt"></i>
                </div>
                <div class="info-content">
                  <h3>地址</h3>
                  <p>台北市信義區信義路五段7號<br>101購物中心1樓</p>
                </div>
              </div>

              <div class="info-item">
                <div class="info-icon">
                  <i class="fas fa-phone"></i>
                </div>
                <div class="info-content">
                  <h3>客服電話</h3>
                  <p>(02) 2345-6789</p>
                  <p class="note">服務時間：週一至週日 09:00-21:00</p>
                </div>
              </div>

              <div class="info-item">
                <div class="info-icon">
                  <i class="fas fa-envelope"></i>
                </div>
                <div class="info-content">
                  <h3>電子郵件</h3>
                  <p>service@cooldrinks.com</p>
                  <p class="note">我們會在24小時內回覆您的郵件</p>
                </div>
              </div>

              <div class="info-item">
                <div class="info-icon">
                  <i class="fas fa-clock"></i>
                </div>
                <div class="info-content">
                  <h3>營業時間</h3>
                  <p>週一至週五：09:00-22:00<br>
                     週六至週日：10:00-23:00</p>
                </div>
              </div>
            </div>

            <!-- 社群媒體 -->
            <div class="social-media">
              <h3>追蹤我們</h3>
              <div class="social-links">
                <a href="#" class="social-link facebook">
                  <i class="fab fa-facebook-f"></i>
                  <span>Facebook</span>
                </a>
                <a href="#" class="social-link instagram">
                  <i class="fab fa-instagram"></i>
                  <span>Instagram</span>
                </a>
                <a href="#" class="social-link line">
                  <i class="fab fa-line"></i>
                  <span>LINE</span>
                </a>
              </div>
            </div>
          </div>
        </div>

        <!-- 地圖區域 -->
        <div class="map-section">
          <h2>店面位置</h2>
          <div class="map-container">
            <iframe
              src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3614.7837094841646!2d121.56168531500419!3d25.0338!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3442abb6da80a7ad%3A0xabc93d1b5bec5ac!2s101%20Tower!5e0!3m2!1sen!2stw!4v1234567890123"
              width="100%"
              height="400"
              style="border:0;"
              allowfullscreen=""
              loading="lazy"
              referrerpolicy="no-referrer-when-downgrade"
            ></iframe>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script setup>
import { ref, reactive } from 'vue'
import MainLayout from '@/layouts/MainLayout.vue'

const isSubmitting = ref(false)

const form = reactive({
  name: '',
  email: '',
  phone: '',
  subject: '',
  message: ''
})

const submitForm = async () => {
  isSubmitting.value = true
  
  try {
    // 模擬API調用
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // 顯示成功訊息
    alert('感謝您的訊息！我們會盡快回覆您。')
    
    // 清空表單
    Object.keys(form).forEach(key => {
      form[key] = ''
    })
  } catch (error) {
    alert('發送失敗，請稍後再試。')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.contact-page {
  padding: 2rem 0;
}

.page-header {
  text-align: center;
  margin-bottom: 3rem;
  padding: 2rem 0;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.page-header h1 {
  font-size: 2.5rem;
  color: #1f2937;
  margin-bottom: 0.5rem;
  font-weight: 700;
}

.page-header p {
  font-size: 1.125rem;
  color: #6b7280;
}

.contact-content {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 3rem;
  margin-bottom: 3rem;
}

.contact-form-section h2,
.contact-info-section h2 {
  font-size: 1.75rem;
  color: #1f2937;
  margin-bottom: 1.5rem;
  font-weight: 600;
}

.contact-form {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #374151;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

textarea.form-input {
  resize: vertical;
  min-height: 120px;
}

.submit-btn {
  background: linear-gradient(135deg, #2563eb, #1d4ed8);
  color: white;
  border: none;
  padding: 0.75rem 2rem;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  width: 100%;
}

.submit-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #1d4ed8, #1e40af);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.4);
}

.submit-btn:disabled {
  background: #9ca3af;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.contact-info {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.info-item {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.info-item:last-child {
  margin-bottom: 0;
}

.info-icon {
  width: 50px;
  height: 50px;
  background: linear-gradient(135deg, #10b981, #059669);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.info-icon i {
  color: white;
  font-size: 1.25rem;
}

.info-content h3 {
  color: #1f2937;
  font-size: 1.125rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.info-content p {
  color: #6b7280;
  margin-bottom: 0.25rem;
  line-height: 1.5;
}

.note {
  font-size: 0.875rem;
  color: #9ca3af;
}

.social-media {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.social-media h3 {
  color: #1f2937;
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.social-links {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.social-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 0.5rem;
  text-decoration: none;
  transition: all 0.2s;
}

.social-link.facebook {
  color: #1877f2;
  background-color: rgba(24, 119, 242, 0.1);
}

.social-link.instagram {
  color: #e1306c;
  background-color: rgba(225, 48, 108, 0.1);
}

.social-link.line {
  color: #00c300;
  background-color: rgba(0, 195, 0, 0.1);
}

.social-link:hover {
  transform: translateX(5px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.map-section {
  margin-top: 3rem;
}

.map-section h2 {
  font-size: 1.75rem;
  color: #1f2937;
  margin-bottom: 1.5rem;
  font-weight: 600;
  text-align: center;
}

.map-container {
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .contact-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .contact-form,
  .contact-info,
  .social-media {
    padding: 1.5rem;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .social-links {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  }
}

@media (max-width: 480px) {
  .contact-form,
  .contact-info,
  .social-media {
    padding: 1rem;
  }

  .info-item {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  .info-icon {
    margin-bottom: 0.5rem;
  }
}
</style>
