import {
    loginWithEmailPasswordUseCase,
    loginWithGoogleUseCase,
    registerWithEmailPasswordUseCase,
    sendOtpEmailUseCase,
    verifyOtpUseCase,
    redisUseCase,
    forgotPasswordUseCase,
    refreshTokenUseCase
} from "@/dependencies";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useAuthStore = defineStore('auth', () => {
    const isLoggedIn = ref(false);
    const user = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    // Hàm trợ giúp set dữ liệu
    function setAuthData(authResult, remember) {
        user.value = authResult.data.user;
        if (remember) {
            localStorage.setItem('authRefresh', authResult.data.authRefresh);
        }
        localStorage.setItem('authToken', authResult.data.authToken); 
        isLoggedIn.value = true;
    }

    function clearAuthData() {
        user.value = null;
        isLoggedIn.value = false;
        localStorage.removeItem('authToken');
        localStorage.removeItem('authRefresh');
    }

    // Login Google
    async function handleGoogleLogin(googleToken) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithGoogleUseCase.execute(googleToken);
            setAuthData(userData, true);
        } catch (err) {
            console.error('Google login failed:', err);
            clearAuthData();
            error.value = err.message || 'Login failed';
        } finally {
            isLoading.value = false;
        }
    }

    // Login Email/Password
    async function handleEmailPasswordLogin(username, password, remember) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithEmailPasswordUseCase.execute(username, password);
            setAuthData(userData, remember);
            return true;
        } catch (err) {
            clearAuthData();
            error.value = err.response?.data?.message || 'Đăng nhập thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    // Logout
    function logout() {
        clearAuthData();
    }

    // Auto-login
    async function tryAutoLogin() {
        const auth_token = localStorage.getItem('authRefresh');
        if (!auth_token) return;
        const refresh_token = redisUseCase.getValue(auth_token);
        if (!refresh_token) return;

        isLoading.value = true;
        try {
            const newAuthData = await refreshTokenUseCase.execute(refresh_token);
            setAuthData(newAuthData, true);
        } catch (err) {
            console.error("Auto login failed:", err);
            clearAuthData();
        } finally {
            isLoading.value = false;
        }
    }

    // Register
    async function registerWithUsernameAndPassword(username, password, displayName) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await registerWithEmailPasswordUseCase.execute(username, password, displayName);
            setAuthData(userData, true);
            return true;
        } catch (err) {
            error.value = err.response?.data?.message || 'Đăng ký thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    // Send OTP
    async function sendOtp(email) {
        isLoading.value = true;
        error.value = null;
        try {
            await sendOtpEmailUseCase.execute(email);
            return true;
        } catch (err) {
            error.value = err.response?.data?.message || 'Gửi OTP thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    // Verify OTP
    async function verifyOtp(email, otp) {
        isLoading.value = true;
        error.value = null;
        try {
            const result = await verifyOtpUseCase.execute(email, otp);
            return result.success || false;
        } catch (err) {
            error.value = err.response?.data?.message || 'Xác thực OTP thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    // Forgot password
    async function forgotPassword(email, username) {
        isLoading.value = true;
        error.value = null;
        console.log('Processing forgot password for:', email, username);
        try {
            var result =await forgotPasswordUseCase.execute(email, username);
            console.log(result);
            if(!result.isSuccess) {
                console.error('Forgot password failed:', result.message);
                error.value = result.message || 'Xử lý quên mật khẩu thất bại.';
                return false;
            }
            return true;
        } catch (err) {
            console.error('Forgot password error:', err);
            error.value = err.response?.data?.message || 'Xử lý quên mật khẩu thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    return { 
        isLoggedIn, 
        user, 
        isLoading, 
        error, 
        handleGoogleLogin, 
        handleEmailPasswordLogin,
        logout, 
        tryAutoLogin,
        registerWithUsernameAndPassword,
        verifyOtp,
        forgotPassword,
        sendOtp
    };
});
