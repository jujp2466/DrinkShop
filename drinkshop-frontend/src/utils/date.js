// 解析 ISO 字串為 Date 的小工具
// 注意：後端可能回傳像 "2025-09-03T14:50:16.758984" 這種沒有時區標記的字串。
// 在不同瀏覽器上，沒有時區的 ISO 字串解析行為可能不一致（有些會視為本地時區），
// 若後端實際上是以 UTC 儲存/回傳時間，前端需要把它當作 UTC 來解析，才能在使用者瀏覽器
// 上正確顯示對應的在地時間（例如 UTC+8 的使用者會看到加 8 小時的時間）。

function normalizeIsoToUtc(isoString) {
  if (!isoString) return ''

  // 常見格式：
  // 1) 2025-09-03T14:50:16.758984  (no timezone) -> treat as UTC -> append 'Z'
  // 2) 2025-09-03 14:50:16           (space instead of T) -> convert to T and append 'Z'
  // 3) 2025-09-03T14:50:16Z         (already UTC) -> leave as-is
  // 4) 2025-09-03T14:50:16+08:00    (has timezone offset) -> leave as-is

  let s = isoString.trim()

  // 如果是類似 "YYYY-MM-DD HH:mm:ss"（有空格）就把空格換成 T
  if (/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/.test(s)) {
    s = s.replace(' ', 'T')
  }

  // 如果已經包含時區標記（Z 或 ±hh:mm），就直接回傳
  if (/[zZ]$/.test(s) || /[+-]\d{2}:?\d{2}$/.test(s)) {
    return s
  }

  // 如果只有 "YYYY-MM-DDTHH:mm:ss"（沒有時區），補上 Z 當作 UTC
  if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?$/.test(s)) {
    return s + 'Z'
  }

  // 其他情況就回傳原字串，讓 Date 去解析（降級處理）
  return s
}

export function formatDate(isoString) {
  if (!isoString) return ''
  const normalized = normalizeIsoToUtc(isoString)
  const d = new Date(normalized)
  return new Intl.DateTimeFormat(undefined, { year: 'numeric', month: '2-digit', day: '2-digit' }).format(d)
}

export function formatDateTime(isoString) {
  if (!isoString) return ''
  const normalized = normalizeIsoToUtc(isoString)
  const d = new Date(normalized)
  return new Intl.DateTimeFormat(undefined, {
    year: 'numeric', month: '2-digit', day: '2-digit',
    hour: '2-digit', minute: '2-digit', second: '2-digit'
  }).format(d)
}

export default { formatDate, formatDateTime }
