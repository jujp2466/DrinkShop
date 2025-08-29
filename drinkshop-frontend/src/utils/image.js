// 共用圖片來源處理：支援 imageUrl / image / 相對路徑與預設圖；茶類優先用茶圖
export function getImageSrc(item) {
  const candidate = item?.imageUrl || item?.image || ''
  const name = (item?.name || '').toLowerCase()
  const category = (item?.category || '').toLowerCase()
  const isTea = /茶|tea/.test(name) || /茶|tea/.test(category)
  if (!candidate) {
    return isTea
      ? 'https://images.unsplash.com/photo-1488900128323-21503983a07e'
      : 'https://images.unsplash.com/photo-1504674900247-0877df9cc836'
  }
  if (candidate.startsWith('http://') || candidate.startsWith('https://')) return candidate
  if (candidate.startsWith('/')) return candidate
  return candidate
}
