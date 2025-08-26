// 全局變量
let currentUser = null;
let cart = [];
let products = [];

// DOM 元素
const loginBtn = document.getElementById('loginBtn');
const registerBtn = document.getElementById('registerBtn');
const logoutBtn = document.getElementById('logoutBtn');
const userInfo = document.getElementById('userInfo');
const loginSection = document.getElementById('loginSection');
const username = document.getElementById('username');
const cartBtn = document.getElementById('cartBtn');
const cartCount = document.getElementById('cartCount');
const productsContainer = document.getElementById('productsContainer');

// 模態框元素
const loginModal = document.getElementById('loginModal');
const registerModal = document.getElementById('registerModal');
const cartModal = document.getElementById('cartModal');
const closeButtons = document.querySelectorAll('.close');

// 表單元素
const loginForm = document.getElementById('loginForm');
const registerForm = document.getElementById('registerForm');

// 購物車元素
const cartItems = document.getElementById('cartItems');
const cartTotal = document.getElementById('cartTotal');
const clearCartBtn = document.getElementById('clearCart');
const checkoutBtn = document.getElementById('checkoutBtn');

// 初始化
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
    setupEventListeners();
    loadProducts();
    loadCart();
    checkAuthStatus();
});

// 初始化應用
function initializeApp() {
    // 從localStorage加載購物車
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
        cart = JSON.parse(savedCart);
        updateCartCount();
    }
    
    // 檢查用戶登錄狀態
    const token = localStorage.getItem('token');
    if (token) {
        // 這裡可以驗證token的有效性
        const user = JSON.parse(localStorage.getItem('user'));
        if (user) {
            currentUser = user;
            updateUserInterface();
        }
    }
}

// 設置事件監聽器
function setupEventListeners() {
    // 登入/註冊按鈕
    loginBtn.addEventListener('click', () => showModal(loginModal));
    registerBtn.addEventListener('click', () => showModal(registerModal));
    logoutBtn.addEventListener('click', logout);
    
    // 購物車按鈕
    cartBtn.addEventListener('click', () => showModal(cartModal));
    
    // 關閉模態框
    closeButtons.forEach(button => {
        button.addEventListener('click', () => {
            hideAllModals();
        });
    });
    
    // 點擊模態框外部關閉
    window.addEventListener('click', (event) => {
        if (event.target.classList.contains('modal')) {
            hideAllModals();
        }
    });
    
    // 表單提交
    loginForm.addEventListener('submit', handleLogin);
    registerForm.addEventListener('submit', handleRegister);
    
    // 購物車操作
    clearCartBtn.addEventListener('click', clearCart);
    checkoutBtn.addEventListener('click', handleCheckout);
}

// 顯示模態框
function showModal(modal) {
    hideAllModals();
    modal.style.display = 'block';
}

// 隱藏所有模態框
function hideAllModals() {
    loginModal.style.display = 'none';
    registerModal.style.display = 'none';
    cartModal.style.display = 'none';
}

// 處理登入
async function handleLogin(event) {
    event.preventDefault();
    
    const username = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;
    
    try {
        const response = await fetch('/api/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data.user));
            currentUser = data.user;
            updateUserInterface();
            hideAllModals();
            showMessage('登入成功！', 'success');
            loginForm.reset();
        } else {
            showMessage(data.error || '登入失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤，請稍後再試', 'error');
    }
}

// 處理註冊
async function handleRegister(event) {
    event.preventDefault();
    
    const username = document.getElementById('registerUsername').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    
    try {
        const response = await fetch('/api/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, email, password })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            showMessage('註冊成功！請登入', 'success');
            hideAllModals();
            showModal(loginModal);
            registerForm.reset();
        } else {
            showMessage(data.error || '註冊失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤，請稍後再試', 'error');
    }
}

// 登出
function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    currentUser = null;
    updateUserInterface();
    showMessage('已登出', 'success');
}

// 更新用戶界面
function updateUserInterface() {
    if (currentUser) {
        loginSection.style.display = 'none';
        userInfo.style.display = 'block';
        username.textContent = currentUser.username;
    } else {
        loginSection.style.display = 'block';
        userInfo.style.display = 'none';
    }
}

// 檢查認證狀態
function checkAuthStatus() {
    const token = localStorage.getItem('token');
    if (token) {
        const user = JSON.parse(localStorage.getItem('user'));
        if (user) {
            currentUser = user;
            updateUserInterface();
        }
    }
}

// 加載產品
async function loadProducts() {
    try {
        const response = await fetch('/api/products');
        const data = await response.json();
        
        if (response.ok) {
            products = data;
            renderProducts();
        } else {
            showMessage('加載產品失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤，請稍後再試', 'error');
    }
}

// 渲染產品
function renderProducts() {
    productsContainer.innerHTML = '';
    
    products.forEach(product => {
        const productCard = createProductCard(product);
        productsContainer.appendChild(productCard);
    });
}

// 創建產品卡片
function createProductCard(product) {
    const card = document.createElement('div');
    card.className = 'product-card';
    
    const imageUrl = product.image_url || 'https://images.unsplash.com/photo-1544145945-f90425340c7e';
    
    card.innerHTML = `
        <div class="product-image">
            <img src="${imageUrl}" alt="${product.name}" onerror="this.src='https://images.unsplash.com/photo-1544145945-f90425340c7e'">
        </div>
        <div class="product-info">
            <h3 class="product-title">${product.name}</h3>
            <p class="product-description">${product.description}</p>
            <p class="product-price">NT$ ${product.price}</p>
            <div class="quantity-controls">
                <button class="quantity-btn" onclick="changeQuantity(${product.id}, -1)">-</button>
                <input type="number" class="quantity-input" id="qty-${product.id}" value="1" min="1" max="99">
                <button class="quantity-btn" onclick="changeQuantity(${product.id}, 1)">+</button>
            </div>
            <button class="btn" onclick="addToCart(${product.id})">加入購物車</button>
        </div>
    `;
    
    return card;
}

// 改變數量
function changeQuantity(productId, change) {
    const input = document.getElementById(`qty-${productId}`);
    let value = parseInt(input.value) + change;
    value = Math.max(1, Math.min(99, value));
    input.value = value;
}

// 添加到購物車
function addToCart(productId) {
    const product = products.find(p => p.id === productId);
    if (!product) return;
    
    const quantityInput = document.getElementById(`qty-${productId}`);
    const quantity = parseInt(quantityInput.value);
    
    const existingItem = cart.find(item => item.productId === productId);
    
    if (existingItem) {
        existingItem.quantity += quantity;
    } else {
        cart.push({
            productId: productId,
            name: product.name,
            price: product.price,
            image: product.image_url,
            quantity: quantity
        });
    }
    
    saveCart();
    updateCartCount();
    showMessage(`${product.name} 已加入購物車`, 'success');
    quantityInput.value = 1;
}

// 保存購物車
function saveCart() {
    localStorage.setItem('cart', JSON.stringify(cart));
}

// 加載購物車
function loadCart() {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
        cart = JSON.parse(savedCart);
        updateCartCount();
    }
}

// 更新購物車數量
function updateCartCount() {
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    cartCount.textContent = totalItems;
}

// 渲染購物車
function renderCart() {
    if (cart.length === 0) {
        cartItems.innerHTML = `
            <div class="empty-cart">
                <i class="fas fa-shopping-cart"></i>
                <p>購物車是空的</p>
            </div>
        `;
        cartTotal.textContent = '0';
        return;
    }
    
    cartItems.innerHTML = '';
    let total = 0;
    
    cart.forEach((item, index) => {
        const cartItem = document.createElement('div');
        cartItem.className = 'cart-item';
        
        const imageUrl = item.image || 'https://images.unsplash.com/photo-1544145945-f90425340c7e';
        const itemTotal = item.price * item.quantity;
        total += itemTotal;
        
        cartItem.innerHTML = `
            <img src="${imageUrl}" alt="${item.name}" class="cart-item-image" onerror="this.src='https://images.unsplash.com/photo-1544145945-f90425340c7e'">
            <div class="cart-item-info">
                <div class="cart-item-name">${item.name}</div>
                <div class="cart-item-price">NT$ ${item.price}</div>
            </div>
            <div class="cart-item-quantity">
                <button class="quantity-btn" onclick="updateCartItemQuantity(${index}, -1)">-</button>
                <span>${item.quantity}</span>
                <button class="quantity-btn" onclick="updateCartItemQuantity(${index}, 1)">+</button>
            </div>
            <div class="cart-item-total">NT$ ${itemTotal}</div>
            <button class="btn-small" onclick="removeFromCart(${index})">刪除</button>
        `;
        
        cartItems.appendChild(cartItem);
    });
    
    cartTotal.textContent = total;
}

// 更新購物車項目數量
function updateCartItemQuantity(index, change) {
    const item = cart[index];
    const newQuantity = item.quantity + change;
    
    if (newQuantity <= 0) {
        removeFromCart(index);
    } else {
        item.quantity = newQuantity;
        saveCart();
        updateCartCount();
        renderCart();
    }
}

// 從購物車移除
function removeFromCart(index) {
    cart.splice(index, 1);
    saveCart();
    updateCartCount();
    renderCart();
}

// 清空購物車
function clearCart() {
    cart = [];
    saveCart();
    updateCartCount();
    renderCart();
    showMessage('購物車已清空', 'success');
}

// 處理結帳
async function handleCheckout() {
    if (!currentUser) {
        showMessage('請先登入', 'error');
        hideAllModals();
        showModal(loginModal);
        return;
    }
    
    if (cart.length === 0) {
        showMessage('購物車是空的', 'error');
        return;
    }
    
    const totalAmount = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    
    try {
        const response = await fetch('/api/orders', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify({
                items: cart.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity,
                    price: item.price
                })),
                totalAmount: totalAmount
            })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            showMessage('訂單創建成功！', 'success');
            clearCart();
            hideAllModals();
        } else {
            showMessage(data.error || '結帳失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤，請稍後再試', 'error');
    }
}

// 顯示消息
function showMessage(message, type) {
    // 移除現有的消息
    const existingMessage = document.querySelector('.message');
    if (existingMessage) {
        existingMessage.remove();
    }
    
    // 創建新消息
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type}`;
    messageDiv.textContent = message;
    
    // 插入到頁面頂部
    document.body.insertBefore(messageDiv, document.body.firstChild);
    
    // 3秒後自動移除
    setTimeout(() => {
        if (messageDiv.parentNode) {
            messageDiv.remove();
        }
    }, 3000);
}

// 購物車模態框顯示時更新購物車
cartBtn.addEventListener('click', () => {
    renderCart();
});
