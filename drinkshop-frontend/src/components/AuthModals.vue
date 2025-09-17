<template>
  <!-- 登入模態框 -->
  <div v-if="showLogin" class="modal-overlay" @click="closeModals">
    <div class="modal" @click.stop>
      <div class="modal-header">
        <h2>用戶登入</h2>
        <button @click="closeModals" class="close-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>
      
      <form @submit.prevent="handleLogin" class="modal-body">
        <div v-if="authStore.error" class="error-message">
          {{ authStore.error }}
        </div>
        
        <div class="form-group">
          <label for="loginUsername">用戶名</label>
          <input
            id="loginUsername"
            v-model="loginForm.username"
            type="text"
            required
            class="form-input"
            placeholder="請輸入用戶名"
          >
        </div>
        
        <div class="form-group">
          <label for="loginPassword">密碼</label>
          <input
            id="loginPassword"
            v-model="loginForm.password"
            type="password"
            required
            class="form-input"
            placeholder="請輸入密碼"
          >
        </div>
        
        <div class="modal-footer">
          <button type="submit" :disabled="authStore.loading" class="btn-primary">
            {{ authStore.loading ? '登入中...' : '登入' }}
          </button>
          <button type="button" @click="switchToRegister" class="btn-link">
            還沒有帳號？註冊
          </button>
        </div>
      </form>
    </div>
  </div>

  <!-- 註冊模態框 -->
  <div v-if="showRegister" class="modal-overlay" @click="closeModals">
    <div class="modal" @click.stop>
      <div class="modal-header">
        <h2>用戶註冊</h2>
        <button @click="closeModals" class="close-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>
      
      <form @submit.prevent="handleRegister" class="modal-body">
        <div v-if="authStore.error" class="error-message">
          {{ authStore.error }}
        </div>
        
        <div class="form-group">
          <label for="registerUsername">用戶名</label>
          <input
            id="registerUsername"
            v-model="registerForm.username"
            type="text"
            required
            class="form-input"
            placeholder="請輸入用戶名"
                    @blur="debouncedCheckUserName"
                    @input="debouncedCheckUserName"
          >
                  <div v-if="userNameStatus === 'checking'" style="color:#888;font-size:0.9em;">檢查中...</div>
                  <div v-if="userNameStatus === 'exists'" style="color:#dc2626;font-size:0.9em;">此用戶名已被註冊</div>
                  <div v-if="userNameStatus === 'ok'" style="color:#16a34a;font-size:0.9em;">可使用</div>
        </div>
        
        <div class="form-group">
          <label for="registerEmail">電子郵件</label>
          <input
            id="registerEmail"
            v-model="registerForm.email"
            type="email"
            required
            class="form-input"
            placeholder="請輸入電子郵件"
                    @blur="debouncedCheckEmail"
                    @input="debouncedCheckEmail"
          >
                  <div v-if="emailStatus === 'checking'" style="color:#888;font-size:0.9em;">檢查中...</div>
                  <div v-if="emailStatus === 'exists'" style="color:#dc2626;font-size:0.9em;">此 Email 已被註冊</div>
                  <div v-if="emailStatus === 'ok'" style="color:#16a34a;font-size:0.9em;">可使用</div>
        </div>
        

        <div class="form-group">
          <label for="registerPassword">密碼</label>
          <input
            id="registerPassword"
            v-model="registerForm.password"
            type="password"
            required
            class="form-input"
            placeholder="請輸入密碼"
          >
        </div>

        <div class="form-group">
          <label for="confirmPassword">確認密碼</label>
          <input
            id="confirmPassword"
            v-model="registerForm.confirmPassword"
            type="password"
            required
            class="form-input"
            placeholder="請再次輸入密碼"
          >
          <div v-if="registerForm.password || registerForm.confirmPassword" style="margin-top:6px; font-size:0.9em;">
            <div v-if="!passwordsMatch" style="color:#dc2626;">密碼不一致</div>
            <div v-else style="color:#16a34a;">密碼一致</div>
          </div>
        </div>

        <div class="form-group">
          <label for="registerPhone">電話</label>
          <input
            id="registerPhone"
            v-model="registerForm.phone"
            type="tel"
            class="form-input"
            placeholder="請輸入電話號碼"
          >
        </div>

        <div class="form-group">
          <label for="registerAddress">地址</label>
          <input
            id="registerAddress"
            v-model="registerForm.address"
            type="text"
            class="form-input"
            placeholder="請輸入地址"
          >
        </div>

        <div class="form-group">
          <label for="registerIsActive">啟用狀態</label>
          <select id="registerIsActive" v-model="registerForm.isActive" class="form-input">
            <option :value="true">啟用</option>
            <option :value="false">停用</option>
          </select>
        </div>
        
        <div class="modal-footer">
          <button type="submit" :disabled="authStore.loading || !passwordsMatch" class="btn-primary">
            {{ authStore.loading ? '註冊中...' : '註冊' }}
          </button>
          <button type="button" @click="switchToLogin" class="btn-link">
            已有帳號？登入
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
// 檢查帳號/Email 是否存在（工具型函式，放在 script 最下方）
async function checkUnique({ userName, email }) {
  const params = {};
  if (userName) params.userName = userName;
  if (email) params.email = email;
  const res = await $api.get('/auth/check-unique', { params });
  return res.data;
}

const authStore = useAuthStore()
const router = useRouter()

const showLogin = ref(false)
const showRegister = ref(false)

const loginForm = reactive({
  username: '',
  password: ''
})

const registerForm = reactive({
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
  phone: '',
  address: '',
  isActive: true
})

// 唯一性檢查狀態
const userNameStatus = ref(''); // '', 'checking', 'exists', 'ok'
const emailStatus = ref('');

let userNameTimer = null;
let emailTimer = null;

import api from '@/api'
const $api = api;
const debouncedCheckUserName = () => {
  if (userNameTimer) clearTimeout(userNameTimer);
  userNameStatus.value = registerForm.username ? 'checking' : '';
  if (!registerForm.username) return;
  userNameTimer = setTimeout(async () => {
    try {
      const res = await checkUnique({ userName: registerForm.username });
      userNameStatus.value = res.exists ? 'exists' : 'ok';
    } catch {
      userNameStatus.value = '';
    }
  }, 400);
};

const debouncedCheckEmail = () => {
  if (emailTimer) clearTimeout(emailTimer);
  emailStatus.value = registerForm.email ? 'checking' : '';
  if (!registerForm.email) return;
  emailTimer = setTimeout(async () => {
    try {
      const res = await checkUnique({ email: registerForm.email });
      emailStatus.value = res.exists ? 'exists' : 'ok';
    } catch {
      emailStatus.value = '';
    }
  }, 400);
};

// 新增一個計算屬性來檢查密碼是否一致
const passwordsMatch = computed(() => {
  return registerForm.password === registerForm.confirmPassword;
});

// 公開的方法，供父組件調用
const openLogin = () => {
  closeModals()
  showLogin.value = true
  authStore.clearError()
}

const openRegister = () => {
  closeModals()
  showRegister.value = true
  authStore.clearError()
}

const closeModals = () => {
  showLogin.value = false
  showRegister.value = false
  authStore.clearError()
}

const switchToRegister = () => {
  showLogin.value = false
  showRegister.value = true
  authStore.clearError()
}

const switchToLogin = () => {
  showRegister.value = false
  showLogin.value = true
  authStore.clearError()
}

const isValidEmail = (val) => /.+@.+\..+/.test(String(val || '').trim())

const handleLogin = async () => {
  const result = await authStore.login(loginForm)
  if (result.success) {
    closeModals()
    // 清空表單
    loginForm.username = ''
    loginForm.password = ''
    // 根據用戶角色跳轉到不同頁面
    if (result.redirectTo) {
      await router.push(result.redirectTo)
    } else if (result.user?.role === 'admin') {
      await router.push('/admin')
    } else {
      await router.push('/')
    }
  }
}

const handleRegister = async () => {
  // 簡單前端驗證：email 必填且格式基本正確
  if (!registerForm.email || !isValidEmail(registerForm.email)) {
    authStore.error = '請輸入有效的電子郵件'
    return
  }
  // 確認密碼需與密碼相同
  if (!registerForm.password || registerForm.password !== registerForm.confirmPassword) {
    authStore.error = '兩次輸入的密碼不相同'
    return
  }

  // 唯一性檢查
  if (userNameStatus.value === 'exists') {
    authStore.error = '用戶名已被註冊';
    return;
  }
  if (emailStatus.value === 'exists') {
    authStore.error = 'Email 已被註冊';
    return;
  }

  const payload = {
    userName: registerForm.username,
    email: registerForm.email,
    password: registerForm.password,
    address: registerForm.address,
    phone: registerForm.phone,
    isActive: registerForm.isActive
  }
  const result = await authStore.register(payload)
  if (result.success) {
    closeModals()
    // 清空表單
    registerForm.username = ''
    registerForm.email = ''
    registerForm.password = ''
    registerForm.confirmPassword = ''
    registerForm.phone = ''
    registerForm.address = ''
    registerForm.isActive = true
    // 根據用戶角色跳轉到不同頁面
    if (result.redirectTo) {
      await router.push(result.redirectTo)
    } else if (result.user?.role === 'admin') {
      await router.push('/admin')
    } else {
      await router.push('/')
    }
  }
}

// 暴露方法供父組件使用
defineExpose({
  openLogin,
  openRegister,
  closeModals
})
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.modal {
  background: white;
  border-radius: 0.5rem;
  width: 100%;
  max-width: 400px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.modal-header {
  padding: 1.5rem 1.5rem 0 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #e5e7eb;
  margin-bottom: 1.5rem;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: #6b7280;
  padding: 0.5rem;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.close-btn:hover {
  background-color: #f3f4f6;
}

.modal-body {
  padding: 0 1.5rem 1.5rem 1.5rem;
}

.error-message {
  background-color: #fef2f2;
  color: #dc2626;
  padding: 0.75rem;
  border-radius: 0.375rem;
  border: 1px solid #fecaca;
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.form-group {
  margin-bottom: 1rem;
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
  border-radius: 0.375rem;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.modal-footer {
  margin-top: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.btn-primary {
  width: 100%;
  background-color: #2563eb;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.375rem;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1d4ed8;
}

.btn-primary:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.btn-link {
  background: none;
  border: none;
  color: #2563eb;
  cursor: pointer;
  font-size: 0.875rem;
  text-decoration: underline;
  padding: 0.5rem;
  transition: color 0.2s;
}

.btn-link:hover {
  color: #1d4ed8;
}
</style>
