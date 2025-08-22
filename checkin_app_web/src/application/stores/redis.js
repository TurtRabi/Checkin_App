import { defineStore } from "pinia";
import { ref } from "vue";
import { redisUseCase } from "@/dependencies"; // chỗ này anh đã khởi tạo trong dependencies.js

export const useRedisStore = defineStore("redis", () => {
  const value = ref(null);      // giá trị lấy từ Redis
  const isLoading = ref(false); // trạng thái loading
  const error = ref(null);      // lỗi

  async function getValue(key) {
    isLoading.value = true;
    error.value = null;
    try {
      const result = await redisUseCase.getValue(key);
      value.value = result;
      return result;
    } catch (err) {
      error.value = err.message || "Không thể lấy giá trị từ Redis";
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  // Set value vào Redis
  async function setValue(key, newValue, expireInSeconds = 3600) {
    isLoading.value = true;
    error.value = null;
    try {
      await redisUseCase.setValue(key, newValue, expireInSeconds);
      value.value = newValue; // cập nhật lại state
    } catch (err) {
      error.value = err.message || "Không thể lưu giá trị vào Redis";
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  async function deleteValue(key) {
    isLoading.value = true;
    error.value = null;
    try {
      await redisUseCase.deleteValue(key);
      if (value.value && key) {
        value.value = null;
      }
    } catch (err) {
      error.value = err.message || "Không thể xóa giá trị trong Redis";
      throw err;
    } finally {
      isLoading.value = false;
    }
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
