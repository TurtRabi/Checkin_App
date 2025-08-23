import { userUseCase } from "@/dependencies";
import { defineStore } from "pinia";   
import { ref } from "vue";
export const useUserStore = defineStore('user', () => {
    const isLoading = ref(false);
    const error = ref(null);
    // Hàm trợ giúp set dữ liệu
    function clearData() {
        error.value = null;
    }
    // Change Password
    async function handleChangePassword(email, username, newPassword) {
        isLoading.value = true;
        error.value = null;
        try {
            const result = await userUseCase.changePassword(email, username, newPassword);
            
            return result;
        } catch (err) {
            error.value = err.message || 'Change password failed';
            return null;
        } finally {
            isLoading.value = false;
        }
    }
    return {
        isLoading,
        error,
        clearData,
        handleChangePassword
    };
});