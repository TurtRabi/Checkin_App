import {
    loginWithEmailPasswordUseCase,
    loginWithGoogleUseCase,
    registerWithEmailPasswordUseCase,
    sendOtpEmailUseCase,
    verifyOtpUseCase,
    forgotPasswordUseCase,
    refreshTokenUseCase
} from "@/dependencies";
import { defineStore } from "pinia";
import { ref } from "vue";
import { useRedisStore } from "@/application/stores/redis";

// Hàm giải mã JWT đơn giản
function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        console.error("Lỗi khi giải mã JWT:", e);
        return null;
    }
}

export const useAuthStore = defineStore('auth', () => {
    // State
    const isLoggedIn = ref(!!localStorage.getItem('authTokenKey'));
    const user = ref(null);
    const role = ref(localStorage.getItem('userRole') || null);
    const isLoading = ref(false);
    const error = ref(null);

    // Hàm set dữ liệu xác thực (chỉ lưu key)
    function setAuthKeys(authResult, remember) {
        user.value = authResult.data.user;
        const authTokenKey = authResult.data.data.authToken;
        const refreshTokenKey = authResult.data.data.authRefresh;
        console.log("Auth Token Key:", authTokenKey);
        console.log("Refresh Token Key:", refreshTokenKey);

        localStorage.setItem('authTokenKey', authTokenKey);
        if (remember) {
            localStorage.setItem('refreshTokenKey', refreshTokenKey);
        }
        isLoggedIn.value = true;
    }

    // Hàm xóa dữ liệu xác thực
    function clearAuthData() {
        user.value = null;
        role.value = null;
        isLoggedIn.value = false;
        localStorage.removeItem('authTokenKey');
        localStorage.removeItem('refreshTokenKey');
        localStorage.removeItem('userRole');
    }

    // Lấy token từ Redis và thiết lập role
    async function fetchTokenAndSetRole() {
        const authTokenKey = localStorage.getItem('authTokenKey');
        if (!authTokenKey) {
            throw new Error('Không tìm thấy khóa của token.');
        }

        const redisStore = useRedisStore();
        const tokenResult = await redisStore.getValue(authTokenKey);
        console.log("Token Result from Redis:", tokenResult);

        if (!tokenResult.isSuccess || !tokenResult.data) {
            clearAuthData();
            throw new Error('Không thể lấy token từ Redis.');
        }

        const accessToken = tokenResult.data;
        const decodedToken = parseJwt(accessToken);
        console.log("Decoded Token:", decodedToken);

        if (decodedToken!=null) {
            const userRole = decodedToken['role'];
            role.value = userRole;
            console.log("User Role:", userRole);
            localStorage.setItem('userRole', userRole);
            return userRole;
        } else {
            const defaultRole = 'User';
            role.value = defaultRole;
            localStorage.setItem('userRole', defaultRole);
            console.warn("Sử dụng vai trò mặc định do không thể giải mã token.");
            return defaultRole;
        }
    }

    // Đăng nhập bằng Email/Password
    async function handleEmailPasswordLogin(username, password, remember) {
        isLoading.value = true;
        error.value = null;
        try {
            // B1: Đăng nhập để lấy key
            const userData = await loginWithEmailPasswordUseCase.execute(username, password);
            setAuthKeys(userData, remember);
            console.log("User data after login:", userData);

            // B2: Lấy token, giải mã và lấy role
            return await fetchTokenAndSetRole();

        } catch (err) {
            clearAuthData();
            error.value = err.response?.data?.message || 'Đăng nhập thất bại.';
            return null; // Trả về null khi có lỗi
        } finally {
            isLoading.value = false;
        }
    }

    // Đăng nhập bằng Google
    async function handleGoogleLogin(googleToken) {
        isLoading.value = true;
        error.value = null;
        try {
            // B1: Đăng nhập để lấy key
            const userData = await loginWithGoogleUseCase.execute(googleToken);
            setAuthKeys(userData, true);

            // B2: Lấy token, giải mã và lấy role
            return await fetchTokenAndSetRole();

        } catch (err) {
            clearAuthData();
            console.error('Google login failed:', err);
            error.value = err.message || 'Login failed';
            return null; // Trả về null khi có lỗi
        } finally {
            isLoading.value = false;
        }
    }

    // Đăng xuất
    function logout() {
        clearAuthData();
    }

    // Tự động đăng nhập
    async function tryAutoLogin() {
        const refreshTokenKey = localStorage.getItem('refreshTokenKey');
        if (!refreshTokenKey) return;

        const redisStore = useRedisStore();
        const tokenResult = await redisStore.getValue(refreshTokenKey);
        if (!tokenResult.isSuccess) return;
        
        const refreshToken = tokenResult.data;

        isLoading.value = true;
        try {
            const newAuthData = await refreshTokenUseCase.execute(refreshToken);
            setAuthKeys(newAuthData, true);
            await fetchTokenAndSetRole(); // Lấy lại role sau khi refresh token
        } catch (err) {
            console.error("Auto login failed:", err);
            clearAuthData();
        } finally {
            isLoading.value = false;
        }
    }

    // Các hàm khác không thay đổi...
    async function registerWithUsernameAndPassword(username, password, displayName) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await registerWithEmailPasswordUseCase.execute(username, password, displayName);
            setAuthKeys(userData, true);
            return await fetchTokenAndSetRole();
        } catch (err) {
            error.value = err.response?.data?.message || 'Đăng ký thất bại.';
            return null;
        } finally {
            isLoading.value = false;
        }
    }

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

    async function forgotPassword(email, username) {
        isLoading.value = true;
        error.value = null;
        try {
            var result = await forgotPasswordUseCase.execute(email, username);
            if(!result.isSuccess) {
                error.value = result.message || 'Xử lý quên mật khẩu thất bại.';
                return false;
            }
            return true;
        } catch (err) {
            error.value = err.response?.data?.message || 'Xử lý quên mật khẩu thất bại.';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    return { 
        isLoggedIn, 
        user, 
        role,
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
