// src/dependencies.js
// File này đóng vai trò là một "Dependency Injection Container" đơn giản.
// Nó là nơi duy nhất trong ứng dụng chịu trách nhiệm khởi tạo và "lắp ráp" các lớp
// từ tầng infrastructure và domain (Repositories, Use Cases, Services, etc.)

// --- Import các lớp triển khai cụ thể từ tầng Infrastructure ---
import AuthRepository from '@/infrastructure/repositories/AuthRepository';
import RedisRepository from './infrastructure/repositories/RedisRepository';

// --- Import các lớp Use Case từ tầng Domain ---
import LoginWithGoogleUseCase from '@/domain/usecases/LoginWithGoogleUseCase';
import LoginWithEmailPasswordUseCase from '@/domain/usecases/LoginWithEmailPasswordUseCase';
import RefreshTokenUseCase from '@/domain/usecases/RefreshTokenUseCase';
import RegisterWithEmailPasswordUseCase from '@/domain/usecases/RegisterWithEmailPasswordUseCase';
import RedisUseCase from './domain/usecases/RedisUseCase';
import SendOtpEmailUseCase from './domain/usecases/SendOtpEmailUseCase';
import verifyOtp from './domain/usecases/VerifyOtpUseCase';

// ===================================================================
// KHỞI TẠO VÀ LẮP RÁP (COMPOSITION ROOT)
// ===================================================================

// 1. Khởi tạo các Repository
const authRepository = new AuthRepository();
const redisRepository = new RedisRepository();

// 2. Khởi tạo các Use Case và "tiêm" (inject) các dependency cần thiết vào chúng
export const loginWithGoogleUseCase = new LoginWithGoogleUseCase(authRepository);
export const loginWithEmailPasswordUseCase = new LoginWithEmailPasswordUseCase(authRepository);
export const refreshTokenUseCase = new RefreshTokenUseCase(authRepository);
export const registerWithEmailPasswordUseCase = new RegisterWithEmailPasswordUseCase(authRepository);
export const redisUseCase = new RedisUseCase(redisRepository);
export const sendOtpEmailUseCase = new SendOtpEmailUseCase(authRepository);
export const verifyOtpUseCase = new verifyOtp(authRepository);
/*
Khi ứng dụng của bạn lớn hơn, bạn có thể khởi tạo và export thêm các use case khác từ đây:

import LogoutUseCase from '@/domain/usecases/LogoutUseCase';
export const logoutUseCase = new LogoutUseCase(authRepository);

*/
