using System;
using System.Collections.Generic;

namespace DrinkShop.Domain.Entities
{
    /// <summary>
    /// 用戶資料表 Entity
    /// </summary>
    public class User
    {
        /// <summary>
        /// 主鍵 Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 電子郵件
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密碼雜湊
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// 電話
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 狀態（active/inactive）
        /// </summary>
        public string Status { get; set; } = "active";

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 訂單集合
        /// </summary>
        public List<Order> Orders { get; set; } = new();
    }
}
