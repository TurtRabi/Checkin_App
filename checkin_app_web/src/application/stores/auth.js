import { defineStore } from 'pinia';
import { ref } from 'vue';
// Bước 1: Import use case đã được lắp ráp sẵn từ dependency container
import { loginWithGoogleUseCase } from '@/dependencies';

export const useAuthStore = defineStore('auth', () => {
    const isLoggedIn = ref(false);
    const user = ref(null);

    // Bước 2: Xóa bỏ việc khởi tạo thủ công repository và use case ở đây.
    // Store bây giờ không cần biết AuthRepository hay LoginWithGoogleUseCase được tạo ra như thế nào.

    async function handleGoogleLogin(googleToken) {
        try {
            // Bước 3: Sử dụng trực tiếp use case đã được import.
            const userData = await loginWithGoogleUseCase.execute(googleToken);
            
            // Giả định backend trả về { user: ..., token: ... }
            // Trong AuthRepository.js của bạn đang trả về googleToken, nên cần điều chỉnh ở đó để có dữ liệu thật
            user.value = userData.user; 
            localStorage.setItem('authToken', userData.token); 
            isLoggedIn.value = true;
        } catch (error) {
            console.error('Failed to login with Google:', error);
            isLoggedIn.value = false;
            user.value = null;
        }
    }

    function logout() {
        isLoggedIn.value = false;
        user.value = null;
        localStorage.removeItem('authToken');
    }

    function login() {
        isLoggedIn.value = true;
    }

    return { isLoggedIn, user, login, logout, handleGoogleLogin };
});
