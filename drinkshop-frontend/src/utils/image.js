// 共用圖片來源處理：支援 imageUrl / image / 相對路徑與預設圖；茶類優先用茶圖
export function getImageSrc(item) {
  return item?.imageUrl || item?.image || ''
}
