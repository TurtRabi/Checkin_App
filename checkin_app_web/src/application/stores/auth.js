import {
    loginWithEmailPasswordUseCase,
    loginWithGoogleUseCase,
    registerWithEmailPasswordUseCase,
    sendOtpEmailUseCase,
    verifyOtpUseCase,
    forgotPasswordUseCase,
    refreshTokenUseCase,
    linkSocialAccountUseCase
} from "@/dependencies";
import { defineStore } from "pinia";
import { ref } from "vue";
import { useRedisStore } from "@/application/stores/redis";
import { startSignalRConnection, stopSignalRConnection } from '@/infrastructure/services/signalrService';
import { useSessionStore } from "@/application/stores/session";

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
    // State - Khởi tạo từ localStorage để giữ trạng thái sau khi tải lại trang
    const isLoggedIn = ref(!!localStorage.getItem('authTokenKey'));
    const storedUser = localStorage.getItem('user');
    const user = ref((storedUser && storedUser !== 'undefined') ? JSON.parse(storedUser) : null);
    const role = ref(localStorage.getItem('userRole') || null);
    const isLoading = ref(false);
    const error = ref(null);
    

    // Hàm set dữ liệu xác thực
    function setAuthKeys(authResult, remember) {
        user.value = authResult.data.data.user;
        const authTokenKey = authResult.data.data.authToken;
        const refreshTokenKey = authResult.data.data.authRefresh;

        console.log("user.value:", user.value);
        localStorage.setItem('authTokenKey', authTokenKey);
        localStorage.setItem('user', JSON.stringify(user.value)); // Lưu user vào localStorage

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
        localStorage.removeItem('user'); // Xóa user khỏi localStorage
    }

    // Lấy token từ Redis và thiết lập role
    async function fetchTokenAndSetRole() {
        const authTokenKey = localStorage.getItem('authTokenKey');
        if (!authTokenKey) {
            throw new Error('Không tìm thấy khóa của token.');
        }

        const redisStore = useRedisStore();
        const tokenResult = await redisStore.getValue(authTokenKey);

        if (!tokenResult.isSuccess || !tokenResult.data) {
            clearAuthData();
            throw new Error('Không thể lấy token từ Redis.');
        }

        const accessToken = tokenResult.data;
        localStorage.setItem('authToken', accessToken); 
        const decodedToken = parseJwt(accessToken);
        if (decodedToken!=null) {
            const userRole = decodedToken['role'];
            role.value = userRole;
            localStorage.setItem('userRole', userRole);
            return userRole;
        } else {
            const defaultRole = 'User';
            role.value = defaultRole;
            localStorage.setItem('userRole', defaultRole);
            return defaultRole;
        }
    }

    // Đăng nhập bằng Email/Password
    async function handleEmailPasswordLogin(username, password, remember) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithEmailPasswordUseCase.execute(username, password);
            setAuthKeys(userData, remember);
            const userRole = await fetchTokenAndSetRole();
            await startSignalRConnection(); // Bắt đầu kết nối SignalR
            return userRole;
        } catch (err) {
            clearAuthData();
            error.value = err.response?.data?.message || 'Đăng nhập thất bại.';
            return null;
        } finally {
            isLoading.value = false;
        }
    }

    // Đăng nhập bằng Google
    async function handleGoogleLogin(googleToken) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithGoogleUseCase.execute(googleToken);
            setAuthKeys(userData, true);
            const userRole = await fetchTokenAndSetRole();
            await startSignalRConnection(); // Bắt đầu kết nối SignalR
            

            return userRole;
        } catch (err) {
            clearAuthData();
            console.error('Google login failed:', err);
            error.value = err.message || 'Login failed';
            return null;
        } finally {
            isLoading.value = false;
        }
    }

    // Đăng xuất
    function logout() {
        const userSession = useSessionStore();
        stopSignalRConnection();
        const logoutSession = userSession.fetchCurrentSession();
        console.log("Current session ID on logout:", logoutSession);
        clearAuthData();
    }

   
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
            await fetchTokenAndSetRole();
            await startSignalRConnection();
        } catch (err) {
            console.error("Auto login failed:", err);
            clearAuthData();
        } finally {
             isLoading.value = false;
        }
    }
    
    
    function updateUserProfile(updatedUser) {
        if (updatedUser) {
            user.value = { ...user.value, ...updatedUser };
            localStorage.setItem('user', JSON.stringify(user.value));
        }
    }

    // Các hàm khác không thay đổi...
    async function registerWithUsernameAndPassword(username, password, displayName) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await registerWithEmailPasswordUseCase.execute(username, password, displayName);
            setAuthKeys(userData, true);
            const userRole = await fetchTokenAndSetRole();
            await startSignalRConnection(); // Bắt đầu kết nối SignalR
            return userRole;
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

    async function linkGoogleAccount(credential) {
        isLoading.value = true;
        error.value = null;
        try {
            const result = await linkSocialAccountUseCase.execute('Google', credential);
            if (result.isSuccess) {
                // Optionally update user info if the backend returns it
                if (result.data) {
                    updateUserProfile(result.data);
                }
                return true;
            } else {
                error.value = result.message || 'Liên kết tài khoản Google thất bại.';
                return false;
            }
        } catch (err) {
            console.error("Lỗi khi liên kết Google:", err);
            error.value = err.response?.data?.message || 'Đã xảy ra lỗi máy chủ khi liên kết tài khoản.';
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
        sendOtp,
        updateUserProfile,
        linkGoogleAccount
    };
});