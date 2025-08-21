import { loginWithEmailPasswordUseCase, loginWithGoogleUseCase,registerWithEmailPasswordUseCase } from "@/dependencies";
import RefreshTokenUseCase from "@/domain/usecases/RefreshTokenUseCase";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useAuthStore = defineStore('auth', () => {
    const isLoggedIn = ref(false);
    const user = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    // Hàm trợ giúp để set dữ liệu đăng nhập
    function setAuthData(authResult, remember) {
        user.value = authResult.data.user;
        
        if (remember) {
            localStorage.setItem('authRefresh', authResult.data.authRefresh);
        }
        localStorage.setItem('authToken', authResult.data.authToken); 
        isLoggedIn.value = true;
    }

    // Hàm trợ giúp để xóa dữ liệu
    function clearAuthData() {
        user.value = null;
        isLoggedIn.value = false;
        localStorage.removeItem('authToken');
        localStorage.removeItem('authRefresh');
    }

    async function handleGoogleLogin(googleToken) {
        
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithGoogleUseCase.execute(googleToken);
            console.log('User data from Google login:', userData);
            user.value = userData.data.user;
            console.log('user.value:', user.value);
            localStorage.setItem('authRefesh', userData.data.authRefresh);
            localStorage.setItem('authToken',userData.data.authToken);
            isLoggedIn.value = true;
            setAuthData(userData, true);
        } catch (error) {
            console.error('Failed to login with Google:', error);
            isLoggedIn.value = false;
            user.value = null;
            error.value = error.message || 'Login failed';
        }finally{
            isLoading.value = false;
        }
         
    }

    async function handleEmailPasswordLogin(username, password, remember) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithEmailPasswordUseCase.execute(username, password);
            setAuthData(userData, remember); // <-- SỬ DỤNG HÀM TRỢ GIÚP
            return true;
        } catch (err) {
            clearAuthData();
            error.value = err.response?.data?.message || 'Đăng nhập thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    function logout() {
        clearAuthData(); 
    }

    async function tryAutoLogin() {
        const refresh_token = localStorage.getItem('authRefresh');
        if (!refresh_token) {
            return; 
        }

        isLoading.value = true;
        try {
            const newAuthData = await RefreshTokenUseCase.execute(refresh_token);
            setAuthData(newAuthData, true); 
        } catch (err) {
            console.error("Auto login failed, token might be expired.", err);
            clearAuthData(); // Xóa token hỏng
        } finally {
            isLoading.value = false;
        }
    }

    async function registerWithUsernameAndPassword(username, password, displayName) {
        try {
            const userData = await registerWithEmailPasswordUseCase.execute(username, password, displayName);
            setAuthData(userData, true); // Lưu thông tin đăng nhập
            return true; // Trả về true nếu đăng ký thành công
        } catch (err) {
            error.value = err.response?.data?.message || 'Đăng ký thất bại.';
            return false; // Trả về false nếu có lỗi
        }
    }

    return { isLoggedIn, user, isLoading, error, handleGoogleLogin, handleEmailPasswordLogin, logout, tryAutoLogin,registerWithUsernameAndPassword };
});