const express = require('express');
const cors = require('cors');
const path = require('path');
const multer = require('multer');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const sqlite3 = require('sqlite3').verbose();
const { v4: uuidv4 } = require('uuid');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3000;

// 中間件
app.use(cors());
app.use(express.json());
app.use(express.static('public'));
app.use('/uploads', express.static('uploads'));

// 數據庫初始化
const db = new sqlite3.Database('beverage_store.db');

// 創建數據表
db.serialize(() => {
    // 用戶表
    db.run(`CREATE TABLE IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        username TEXT UNIQUE NOT NULL,
        password TEXT NOT NULL,
        email TEXT UNIQUE NOT NULL,
        role TEXT DEFAULT 'user',
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    )`);

    // 產品表
    db.run(`CREATE TABLE IF NOT EXISTS products (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        description TEXT,
        price REAL NOT NULL,
        category TEXT,
        image_url TEXT,
        stock INTEGER DEFAULT 0,
        is_active BOOLEAN DEFAULT 1,
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    )`);

    // 訂單表
    db.run(`CREATE TABLE IF NOT EXISTS orders (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        user_id INTEGER,
        total_amount REAL NOT NULL,
        status TEXT DEFAULT 'pending',
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (user_id) REFERENCES users (id)
    )`);

    // 訂單詳情表
    db.run(`CREATE TABLE IF NOT EXISTS order_items (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        order_id INTEGER,
        product_id INTEGER,
        quantity INTEGER NOT NULL,
        price REAL NOT NULL,
        FOREIGN KEY (order_id) REFERENCES orders (id),
        FOREIGN KEY (product_id) REFERENCES products (id)
    )`);

    // 插入默認管理員帳戶
    const adminPassword = bcrypt.hashSync('admin123', 10);
    db.run(`INSERT OR IGNORE INTO users (username, password, email, role) VALUES (?, ?, ?, ?)`,
        ['admin', adminPassword, 'admin@beverage.com', 'admin']);

    // 插入示例產品
    const sampleProducts = [
        ['繽紛水果茶', '新鮮水果與優質茶葉完美結合，清爽解渴。', 80, '茶類', '/images/fruit-tea.jpg', 100],
        ['經典珍珠奶茶', '香濃奶茶搭配Q彈珍珠，台灣經典飲品。', 65, '奶茶類', '/images/bubble-tea.jpg', 150],
        ['抹茶拿鐵', '日本進口抹茶粉，搭配鮮奶製成，濃郁香醇。', 90, '咖啡類', '/images/matcha-latte.jpg', 80],
        ['清新檸檬冰茶', '新鮮檸檬與冰紅茶的完美結合，酸甜可口。', 70, '茶類', '/images/lemon-tea.jpg', 120],
        ['芒果冰沙', '使用台灣在地芒果，香甜濃郁，夏日必備。', 95, '冰沙類', '/images/mango-smoothie.jpg', 60],
        ['草莓奶昔', '新鮮草莓與優質鮮奶製成，香甜可口。', 85, '奶昔類', '/images/strawberry-milkshake.jpg', 90]
    ];

    sampleProducts.forEach(product => {
        db.run(`INSERT OR IGNORE INTO products (name, description, price, category, image_url, stock) VALUES (?, ?, ?, ?, ?, ?)`, product);
    });
});

// 文件上傳配置
const storage = multer.diskStorage({
    destination: (req, file, cb) => {
        cb(null, 'uploads/');
    },
    filename: (req, file, cb) => {
        const uniqueName = `${Date.now()}-${Math.round(Math.random() * 1E9)}${path.extname(file.originalname)}`;
        cb(null, uniqueName);
    }
});

const upload = multer({ storage: storage });

// JWT 驗證中間件
const authenticateToken = (req, res, next) => {
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    if (!token) {
        return res.sendStatus(401);
    }

    jwt.verify(token, process.env.JWT_SECRET || 'your-secret-key', (err, user) => {
        if (err) return res.sendStatus(403);
        req.user = user;
        next();
    });
};

// 管理員驗證中間件
const requireAdmin = (req, res, next) => {
    if (req.user.role !== 'admin') {
        return res.status(403).json({ error: '需要管理員權限' });
    }
    next();
};

// API 路由



// 用戶登錄
app.post('/api/auth/login', (req, res) => {
    const { username, password } = req.body;
    
    db.get('SELECT * FROM users WHERE username = ?', [username], async (err, user) => {
        if (err) return res.status(500).json({ error: '服務器錯誤' });
        if (!user) return res.status(401).json({ error: '用戶名或密碼錯誤' });
        
        const validPassword = await bcrypt.compare(password, user.password);
        if (!validPassword) return res.status(401).json({ error: '用戶名或密碼錯誤' });
        
        const token = jwt.sign(
            { id: user.id, username: user.username, role: user.role },
            process.env.JWT_SECRET || 'your-secret-key',
            { expiresIn: '24h' }
        );
        
        res.json({ token, user: { id: user.id, username: user.username, role: user.role } });
    });
});

// 獲取當前用戶信息
app.get('/api/auth/me', authenticateToken, (req, res) => {
    db.get('SELECT id, username, email, role, created_at FROM users WHERE id = ?', [req.user.id], (err, user) => {
        if (err) return res.status(500).json({ error: '獲取用戶信息失敗' });
        if (!user) return res.status(404).json({ error: '用戶不存在' });
        res.json(user);
    });
});

// 用戶註冊
app.post('/api/auth/register', async (req, res) => {
    const { username, email, password } = req.body;
    
    // 檢查用戶名是否已存在
    db.get('SELECT id FROM users WHERE username = ?', [username], async (err, existingUser) => {
        if (err) return res.status(500).json({ error: '服務器錯誤' });
        if (existingUser) return res.status(400).json({ error: '用戶名已存在' });
        
        // 檢查郵箱是否已存在
        db.get('SELECT id FROM users WHERE email = ?', [email], async (err, existingEmail) => {
            if (err) return res.status(500).json({ error: '服務器錯誤' });
            if (existingEmail) return res.status(400).json({ error: '郵箱已存在' });
            
            // 加密密碼
            const hashedPassword = await bcrypt.hash(password, 10);
            
            // 創建新用戶
            db.run('INSERT INTO users (username, email, password, role) VALUES (?, ?, ?, ?)',
                [username, email, hashedPassword, 'user'],
                function(err) {
                    if (err) return res.status(500).json({ error: '註冊失敗' });
                    
                    // 生成 JWT token
                    const token = jwt.sign(
                        { id: this.lastID, username, role: 'user' },
                        process.env.JWT_SECRET || 'your-secret-key',
                        { expiresIn: '24h' }
                    );
                    
                    res.json({ 
                        token, 
                        user: { id: this.lastID, username, email, role: 'user' } 
                    });
                });
        });
    });
});

// 獲取所有產品
app.get('/api/products', (req, res) => {
    db.all('SELECT * FROM products WHERE is_active = 1', (err, products) => {
        if (err) return res.status(500).json({ error: '獲取產品失敗' });
        res.json(products);
    });
});

// 獲取單個產品
app.get('/api/products/:id', (req, res) => {
    db.get('SELECT * FROM products WHERE id = ? AND is_active = 1', [req.params.id], (err, product) => {
        if (err) return res.status(500).json({ error: '獲取產品失敗' });
        if (!product) return res.status(404).json({ error: '產品不存在' });
        res.json(product);
    });
});

// 管理員：添加產品
app.post('/api/products', authenticateToken, requireAdmin, upload.single('image'), (req, res) => {
    const { name, description, price, category, stock } = req.body;
    const imageUrl = req.file ? `/uploads/${req.file.filename}` : null;
    
    db.run('INSERT INTO products (name, description, price, category, image_url, stock) VALUES (?, ?, ?, ?, ?, ?)',
        [name, description, price, category, imageUrl, stock],
        function(err) {
            if (err) return res.status(500).json({ error: '添加產品失敗' });
            
            // 返回新創建的產品信息
            db.get('SELECT * FROM products WHERE id = ?', [this.lastID], (err, product) => {
                if (err) return res.status(500).json({ error: '獲取產品信息失敗' });
                res.json(product);
            });
        });
});

// 管理員：更新產品
app.put('/api/products/:id', authenticateToken, requireAdmin, upload.single('image'), (req, res) => {
    const { name, description, price, category, stock, is_active } = req.body;
    const imageUrl = req.file ? `/uploads/${req.file.filename}` : undefined;
    
    let query = 'UPDATE products SET name = ?, description = ?, price = ?, category = ?, stock = ?, is_active = ?';
    let params = [name, description, price, category, stock, is_active];
    
    if (imageUrl) {
        query += ', image_url = ?';
        params.push(imageUrl);
    }
    
    query += ' WHERE id = ?';
    params.push(req.params.id);
    
    db.run(query, params, function(err) {
        if (err) return res.status(500).json({ error: '更新產品失敗' });
        
        // 返回更新後的產品信息
        db.get('SELECT * FROM products WHERE id = ?', [req.params.id], (err, product) => {
            if (err) return res.status(500).json({ error: '獲取產品信息失敗' });
            res.json(product);
        });
    });
});

// 管理員：刪除產品
app.delete('/api/products/:id', authenticateToken, requireAdmin, (req, res) => {
    db.run('DELETE FROM products WHERE id = ?', [req.params.id], function(err) {
        if (err) return res.status(500).json({ error: '刪除產品失敗' });
        res.json({ message: '產品刪除成功' });
    });
});

// 創建訂單
app.post('/api/orders', authenticateToken, (req, res) => {
    const { items, totalAmount } = req.body;
    
    db.run('INSERT INTO orders (user_id, total_amount) VALUES (?, ?)',
        [req.user.id, totalAmount],
        function(err) {
            if (err) return res.status(500).json({ error: '創建訂單失敗' });
            
            const orderId = this.lastID;
            let completed = 0;
            
            items.forEach(item => {
                db.run('INSERT INTO order_items (order_id, product_id, quantity, price) VALUES (?, ?, ?, ?)',
                    [orderId, item.productId, item.quantity, item.price],
                    (err) => {
                        if (err) return res.status(500).json({ error: '添加訂單項目失敗' });
                        completed++;
                        if (completed === items.length) {
                            res.json({ message: '訂單創建成功', orderId });
                        }
                    });
            });
        });
});

// 獲取用戶訂單
app.get('/api/orders', authenticateToken, (req, res) => {
    const query = req.user.role === 'admin' 
        ? 'SELECT o.*, u.username FROM orders o LEFT JOIN users u ON o.user_id = u.id ORDER BY o.created_at DESC'
        : 'SELECT * FROM orders WHERE user_id = ? ORDER BY created_at DESC';
    
    const params = req.user.role === 'admin' ? [] : [req.user.id];
    
    db.all(query, params, (err, orders) => {
        if (err) return res.status(500).json({ error: '獲取訂單失敗' });
        res.json(orders);
    });
});

// 更新訂單狀態
app.put('/api/admin/orders/:id/status', authenticateToken, requireAdmin, (req, res) => {
    const { status } = req.body;
    db.run('UPDATE orders SET status = ? WHERE id = ?', [status, req.params.id], function(err) {
        if (err) return res.status(500).json({ error: '更新訂單狀態失敗' });
        res.json({ message: '訂單狀態更新成功' });
    });
});

// 前端路由 - 支援 Vue Router history 模式
app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname, 'index.html'));
});

app.listen(PORT, () => {
    console.log(`服務器運行在 http://localhost:${PORT}`);
});

