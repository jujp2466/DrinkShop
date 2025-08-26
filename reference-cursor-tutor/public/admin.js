// 全局變量
let currentUser = null;
let products = [];
let orders = [];
let users = [];
let editingProductId = null;

// DOM 元素
const sidebarToggle = document.getElementById('sidebarToggle');
const sidebar = document.querySelector('.sidebar');
const navLinks = document.querySelectorAll('.nav-link');
const sections = document.querySelectorAll('.section');
const pageTitle = document.getElementById('pageTitle');
const currentTime = document.getElementById('currentTime');
const logoutBtn = document.getElementById('logoutBtn');
const adminUsername = document.getElementById('adminUsername');

// 儀表板元素
const totalProducts = document.getElementById('totalProducts');
const totalOrders = document.getElementById('totalOrders');
const totalUsers = document.getElementById('totalUsers');
const totalRevenue = document.getElementById('totalRevenue');
const recentOrders = document.getElementById('recentOrders');
const popularProducts = document.getElementById('popularProducts');

// 產品管理元素
const addProductBtn = document.getElementById('addProductBtn');
const productSearch = document.getElementById('productSearch');
const categoryFilter = document.getElementById('categoryFilter');
const productsTableBody = document.getElementById('productsTableBody');

// 訂單管理元素
const orderStatusFilter = document.getElementById('orderStatusFilter');
const orderDateFilter = document.getElementById('orderDateFilter');
const ordersTableBody = document.getElementById('ordersTableBody');

// 用戶管理元素
const userSearch = document.getElementById('userSearch');
const userRoleFilter = document.getElementById('userRoleFilter');
const usersTableBody = document.getElementById('usersTableBody');

// 模態框元素
const productModal = document.getElementById('productModal');
const orderModal = document.getElementById('orderModal');
const closeButtons = document.querySelectorAll('.close');

// 產品表單元素
const productForm = document.getElementById('productForm');
const productModalTitle = document.getElementById('productModalTitle');
const productName = document.getElementById('productName');
const productCategory = document.getElementById('productCategory');
const productPrice = document.getElementById('productPrice');
const productStock = document.getElementById('productStock');
const productDescription = document.getElementById('productDescription');
const productImage = document.getElementById('productImage');
const productActive = document.getElementById('productActive');
const imagePreview = document.getElementById('imagePreview');

// 初始化
document.addEventListener('DOMContentLoaded', function() {
    checkAuth();
    setupEventListeners();
    updateCurrentTime();
    setInterval(updateCurrentTime, 1000);
    loadDashboard();
});

// 檢查認證
function checkAuth() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/';
        return;
    }
    
    // 這裡可以驗證token的有效性
    const user = JSON.parse(localStorage.getItem('user'));
    if (user && user.role === 'admin') {
        currentUser = user;
        adminUsername.textContent = user.username;
    } else {
        window.location.href = '/';
    }
}

// 設置事件監聽器
function setupEventListeners() {
    // 側邊欄切換
    sidebarToggle.addEventListener('click', toggleSidebar);
    
    // 導航鏈接
    navLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const section = link.getAttribute('data-section');
            switchSection(section);
        });
    });
    
    // 登出
    logoutBtn.addEventListener('click', logout);
    
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
    
    // 產品管理
    addProductBtn.addEventListener('click', () => showProductModal());
    productForm.addEventListener('submit', handleProductSubmit);
    productSearch.addEventListener('input', filterProducts);
    categoryFilter.addEventListener('change', filterProducts);
    
    // 訂單管理
    orderStatusFilter.addEventListener('change', filterOrders);
    orderDateFilter.addEventListener('change', filterOrders);
    
    // 用戶管理
    userSearch.addEventListener('input', filterUsers);
    userRoleFilter.addEventListener('change', filterUsers);
    
    // 圖片預覽
    productImage.addEventListener('change', handleImagePreview);
}

// 切換側邊欄
function toggleSidebar() {
    sidebar.classList.toggle('open');
}

// 切換區段
function switchSection(sectionName) {
    // 更新導航鏈接
    navLinks.forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('data-section') === sectionName) {
            link.classList.add('active');
        }
    });
    
    // 更新區段顯示
    sections.forEach(section => {
        section.classList.remove('active');
        if (section.id === sectionName) {
            section.classList.add('active');
        }
    });
    
    // 更新頁面標題
    const titles = {
        dashboard: '儀表板',
        products: '產品管理',
        orders: '訂單管理',
        users: '用戶管理'
    };
    pageTitle.textContent = titles[sectionName];
    
    // 加載對應數據
    switch(sectionName) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'products':
            loadProducts();
            break;
        case 'orders':
            loadOrders();
            break;
        case 'users':
            loadUsers();
            break;
    }
}

// 更新當前時間
function updateCurrentTime() {
    const now = new Date();
    currentTime.textContent = now.toLocaleString('zh-TW');
}

// 登出
function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/';
}

// 隱藏所有模態框
function hideAllModals() {
    productModal.style.display = 'none';
    orderModal.style.display = 'none';
    resetProductForm();
}

// 顯示消息
function showMessage(message, type) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type}`;
    messageDiv.textContent = message;
    
    document.body.appendChild(messageDiv);
    
    setTimeout(() => {
        if (messageDiv.parentNode) {
            messageDiv.remove();
        }
    }, 3000);
}

// 加載儀表板
async function loadDashboard() {
    try {
        const [productsRes, ordersRes, usersRes] = await Promise.all([
            fetch('/api/products'),
            fetch('/api/orders', {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            }),
            fetch('/api/users', {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            })
        ]);
        
        const productsData = await productsRes.json();
        const ordersData = await ordersRes.json();
        const usersData = await usersRes.json();
        
        if (productsRes.ok && ordersRes.ok && usersRes.ok) {
            updateDashboardStats(productsData, ordersData, usersData);
            updateRecentOrders(ordersData.slice(0, 5));
            updatePopularProducts(productsData);
        }
    } catch (error) {
        showMessage('加載儀表板數據失敗', 'error');
    }
}

// 更新儀表板統計
function updateDashboardStats(products, orders, users) {
    totalProducts.textContent = products.length;
    totalOrders.textContent = orders.length;
    totalUsers.textContent = users.length;
    
    const revenue = orders.reduce((sum, order) => sum + order.total_amount, 0);
    totalRevenue.textContent = `NT$ ${revenue.toLocaleString()}`;
}

// 更新最近訂單
function updateRecentOrders(orders) {
    recentOrders.innerHTML = '';
    
    if (orders.length === 0) {
        recentOrders.innerHTML = '<div class="empty-state"><p>暫無訂單</p></div>';
        return;
    }
    
    orders.forEach(order => {
        const orderItem = document.createElement('div');
        orderItem.className = 'order-item';
        orderItem.innerHTML = `
            <div class="order-info">
                <div class="order-title">訂單 #${order.id}</div>
                <div class="order-meta">${order.username || '未知用戶'} • ${new Date(order.created_at).toLocaleDateString()}</div>
            </div>
            <div class="order-amount">NT$ ${order.total_amount}</div>
        `;
        recentOrders.appendChild(orderItem);
    });
}

// 更新熱門產品
function updatePopularProducts(products) {
    popularProducts.innerHTML = '';
    
    if (products.length === 0) {
        popularProducts.innerHTML = '<div class="empty-state"><p>暫無產品</p></div>';
        return;
    }
    
    // 這裡可以根據實際銷售數據來排序，目前只是顯示前5個產品
    const topProducts = products.slice(0, 5);
    
    topProducts.forEach(product => {
        const productItem = document.createElement('div');
        productItem.className = 'product-item';
        productItem.innerHTML = `
            <div class="product-info">
                <div class="product-title">${product.name}</div>
                <div class="product-meta">${product.category} • 庫存: ${product.stock}</div>
            </div>
            <div class="product-sales">NT$ ${product.price}</div>
        `;
        popularProducts.appendChild(productItem);
    });
}

// 加載產品
async function loadProducts() {
    try {
        const response = await fetch('/api/products');
        const data = await response.json();
        
        if (response.ok) {
            products = data;
            renderProductsTable();
        } else {
            showMessage('加載產品失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤', 'error');
    }
}

// 渲染產品表格
function renderProductsTable(filteredProducts = null) {
    const productsToRender = filteredProducts || products;
    
    productsTableBody.innerHTML = '';
    
    if (productsToRender.length === 0) {
        productsTableBody.innerHTML = `
            <tr>
                <td colspan="7" class="empty-state">
                    <i class="fas fa-box-open"></i>
                    <h3>暫無產品</h3>
                    <p>點擊「新增產品」來添加第一個產品</p>
                </td>
            </tr>
        `;
        return;
    }
    
    productsToRender.forEach(product => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>
                <img src="${product.image_url || 'https://images.unsplash.com/photo-1544145945-f90425340c7e'}" 
                     alt="${product.name}" 
                     onerror="this.src='https://images.unsplash.com/photo-1544145945-f90425340c7e'">
            </td>
            <td>${product.name}</td>
            <td>${product.category}</td>
            <td>NT$ ${product.price}</td>
            <td>${product.stock}</td>
            <td>
                <span class="status-badge ${product.is_active ? 'status-active' : 'status-inactive'}">
                    ${product.is_active ? '啟用' : '停用'}
                </span>
            </td>
            <td>
                <button class="btn-success" onclick="editProduct(${product.id})">編輯</button>
                <button class="btn-danger" onclick="deleteProduct(${product.id})">刪除</button>
            </td>
        `;
        productsTableBody.appendChild(row);
    });
}

// 篩選產品
function filterProducts() {
    const searchTerm = productSearch.value.toLowerCase();
    const categoryFilter = document.getElementById('categoryFilter').value;
    
    const filtered = products.filter(product => {
        const matchesSearch = product.name.toLowerCase().includes(searchTerm) ||
                            product.description.toLowerCase().includes(searchTerm);
        const matchesCategory = !categoryFilter || product.category === categoryFilter;
        
        return matchesSearch && matchesCategory;
    });
    
    renderProductsTable(filtered);
}

// 顯示產品模態框
function showProductModal(productId = null) {
    editingProductId = productId;
    
    if (productId) {
        const product = products.find(p => p.id === productId);
        if (product) {
            productModalTitle.textContent = '編輯產品';
            productName.value = product.name;
            productCategory.value = product.category;
            productPrice.value = product.price;
            productStock.value = product.stock;
            productDescription.value = product.description;
            productActive.checked = product.is_active;
            
            if (product.image_url) {
                imagePreview.innerHTML = `<img src="${product.image_url}" alt="預覽">`;
            }
        }
    } else {
        productModalTitle.textContent = '新增產品';
        resetProductForm();
    }
    
    productModal.style.display = 'block';
}

// 重置產品表單
function resetProductForm() {
    productForm.reset();
    imagePreview.innerHTML = '';
    editingProductId = null;
}

// 處理圖片預覽
function handleImagePreview(event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            imagePreview.innerHTML = `<img src="${e.target.result}" alt="預覽">`;
        };
        reader.readAsDataURL(file);
    }
}

// 處理產品表單提交
async function handleProductSubmit(event) {
    event.preventDefault();
    
    const formData = new FormData();
    formData.append('name', productName.value);
    formData.append('category', productCategory.value);
    formData.append('price', productPrice.value);
    formData.append('stock', productStock.value);
    formData.append('description', productDescription.value);
    formData.append('is_active', productActive.checked ? '1' : '0');
    
    const imageFile = productImage.files[0];
    if (imageFile) {
        formData.append('image', imageFile);
    }
    
    try {
        const url = editingProductId 
            ? `/api/admin/products/${editingProductId}`
            : '/api/admin/products';
        
        const method = editingProductId ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method: method,
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: formData
        });
        
        const data = await response.json();
        
        if (response.ok) {
            showMessage(editingProductId ? '產品更新成功' : '產品新增成功', 'success');
            hideAllModals();
            loadProducts();
        } else {
            showMessage(data.error || '操作失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤', 'error');
    }
}

// 編輯產品
function editProduct(productId) {
    showProductModal(productId);
}

// 刪除產品
async function deleteProduct(productId) {
    if (!confirm('確定要刪除此產品嗎？')) {
        return;
    }
    
    try {
        const response = await fetch(`/api/admin/products/${productId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });
        
        const data = await response.json();
        
        if (response.ok) {
            showMessage('產品刪除成功', 'success');
            loadProducts();
        } else {
            showMessage(data.error || '刪除失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤', 'error');
    }
}

// 加載訂單
async function loadOrders() {
    try {
        const response = await fetch('/api/orders', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });
        const data = await response.json();
        
        if (response.ok) {
            orders = data;
            renderOrdersTable();
        } else {
            showMessage('加載訂單失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤', 'error');
    }
}

// 渲染訂單表格
function renderOrdersTable(filteredOrders = null) {
    const ordersToRender = filteredOrders || orders;
    
    ordersTableBody.innerHTML = '';
    
    if (ordersToRender.length === 0) {
        ordersTableBody.innerHTML = `
            <tr>
                <td colspan="6" class="empty-state">
                    <i class="fas fa-shopping-cart"></i>
                    <h3>暫無訂單</h3>
                    <p>還沒有任何訂單</p>
                </td>
            </tr>
        `;
        return;
    }
    
    ordersToRender.forEach(order => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>#${order.id}</td>
            <td>${order.username || '未知用戶'}</td>
            <td>NT$ ${order.total_amount}</td>
            <td>
                <span class="status-badge status-${order.status}">
                    ${getStatusText(order.status)}
                </span>
            </td>
            <td>${new Date(order.created_at).toLocaleString('zh-TW')}</td>
            <td>
                <button class="btn-success" onclick="viewOrderDetails(${order.id})">查看詳情</button>
                <select onchange="updateOrderStatus(${order.id}, this.value)" class="filter-select">
                    <option value="pending" ${order.status === 'pending' ? 'selected' : ''}>待處理</option>
                    <option value="processing" ${order.status === 'processing' ? 'selected' : ''}>處理中</option>
                    <option value="completed" ${order.status === 'completed' ? 'selected' : ''}>已完成</option>
                    <option value="cancelled" ${order.status === 'cancelled' ? 'selected' : ''}>已取消</option>
                </select>
            </td>
        `;
        ordersTableBody.appendChild(row);
    });
}

// 獲取狀態文字
function getStatusText(status) {
    const statusMap = {
        pending: '待處理',
        processing: '處理中',
        completed: '已完成',
        cancelled: '已取消'
    };
    return statusMap[status] || status;
}

// 篩選訂單
function filterOrders() {
    const statusFilter = orderStatusFilter.value;
    const dateFilter = orderDateFilter.value;
    
    const filtered = orders.filter(order => {
        const matchesStatus = !statusFilter || order.status === statusFilter;
        const matchesDate = !dateFilter || order.created_at.startsWith(dateFilter);
        
        return matchesStatus && matchesDate;
    });
    
    renderOrdersTable(filtered);
}

// 更新訂單狀態
async function updateOrderStatus(orderId, status) {
    try {
        const response = await fetch(`/api/admin/orders/${orderId}/status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify({ status })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            showMessage('訂單狀態更新成功', 'success');
            loadOrders();
        } else {
            showMessage(data.error || '更新失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤', 'error');
    }
}

// 查看訂單詳情
function viewOrderDetails(orderId) {
    // 這裡可以實現查看訂單詳情的功能
    showMessage('訂單詳情功能開發中', 'error');
}

// 加載用戶
async function loadUsers() {
    try {
        const response = await fetch('/api/users', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });
        const data = await response.json();
        
        if (response.ok) {
            users = data;
            renderUsersTable();
        } else {
            showMessage('加載用戶失敗', 'error');
        }
    } catch (error) {
        showMessage('網絡錯誤', 'error');
    }
}

// 渲染用戶表格
function renderUsersTable(filteredUsers = null) {
    const usersToRender = filteredUsers || users;
    
    usersTableBody.innerHTML = '';
    
    if (usersToRender.length === 0) {
        usersTableBody.innerHTML = `
            <tr>
                <td colspan="5" class="empty-state">
                    <i class="fas fa-users"></i>
                    <h3>暫無用戶</h3>
                    <p>還沒有任何用戶註冊</p>
                </td>
            </tr>
        `;
        return;
    }
    
    usersToRender.forEach(user => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${user.username}</td>
            <td>${user.email}</td>
            <td>
                <span class="status-badge ${user.role === 'admin' ? 'status-active' : 'status-inactive'}">
                    ${user.role === 'admin' ? '管理員' : '一般用戶'}
                </span>
            </td>
            <td>${new Date(user.created_at).toLocaleString('zh-TW')}</td>
            <td>
                <button class="btn-danger" onclick="deleteUser(${user.id})">刪除</button>
            </td>
        `;
        usersTableBody.appendChild(row);
    });
}

// 篩選用戶
function filterUsers() {
    const searchTerm = userSearch.value.toLowerCase();
    const roleFilter = userRoleFilter.value;
    
    const filtered = users.filter(user => {
        const matchesSearch = user.username.toLowerCase().includes(searchTerm) ||
                            user.email.toLowerCase().includes(searchTerm);
        const matchesRole = !roleFilter || user.role === roleFilter;
        
        return matchesSearch && matchesRole;
    });
    
    renderUsersTable(filtered);
}

// 刪除用戶
async function deleteUser(userId) {
    if (!confirm('確定要刪除此用戶嗎？')) {
        return;
    }
    
    // 這裡可以實現刪除用戶的API調用
    showMessage('刪除用戶功能開發中', 'error');
}

// 關閉產品模態框
function closeProductModal() {
    hideAllModals();
}
