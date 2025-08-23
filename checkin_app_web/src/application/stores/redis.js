import { defineStore } from "pinia";
import { ref } from "vue";
import { redisUseCase } from "@/dependencies";

export const useRedisStore = defineStore("redis", () => {
  const value = ref(null);
  const isLoading = ref(false);
  const error = ref(null);

  async function getValue(key) {
    isLoading.value = true;
    error.value = null;
    const result = await redisUseCase.getValue(key);
    if (result.isSuccess) {
      value.value = result;
    } else {
      error.value = result.message;
      console.error(`Lỗi khi lấy giá trị từ Redis: ${result.message}`);
    }
    isLoading.value = false;
    return result; // Trả về kết quả gốc để component có thể xử lý thêm nếu cần
  }

  async function setValue(key, newValue, expireInSeconds = 3600) {
    isLoading.value = true;
    error.value = null;
    const result = await redisUseCase.setValue(key, newValue, expireInSeconds);
    if (result.isSuccess) {
      value.value = newValue; // Cập nhật state nội bộ
    } else {
      error.value = result.message;
      console.error(`Lỗi khi lưu giá trị vào Redis: ${result.message}`);
    }
    isLoading.value = false;
    return result;
  }

  async function deleteValue(key) {
    isLoading.value = true;
    error.value = null;
    // Giả định use case có phương thức deleteValue, và nó cũng trả về cấu trúc chuẩn
    const result = await redisUseCase.deleteValue(key);
    if (result.isSuccess) {
      value.value = null; // Xóa giá trị trong state
    } else {
      error.value = result.message;
      console.error(`Lỗi khi xóa giá trị trong Redis: ${result.message}`);
    }
    isLoading.value = false;
    return result;
  }

  return {
    value,
    isLoading,
    error,
    getValue,
    setValue,
    deleteValue,
  };
});
