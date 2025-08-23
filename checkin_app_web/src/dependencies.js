// src/dependencies.js

// Repositories
import AuthRepository from '@/infrastructure/repositories/AuthRepository';
import RedisRepository from '@/infrastructure/repositories/RedisRepository';
import UserRepository from '@/infrastructure/repositories/UserRepository';

// Use cases (class)
import LoginWithGoogleUseCase from '@/domain/usecases/LoginWithGoogleUseCase';
import LoginWithEmailPasswordUseCase from '@/domain/usecases/LoginWithEmailPasswordUseCase';
import RefreshTokenUseCase from '@/domain/usecases/RefreshTokenUseCase';
import RegisterWithEmailPasswordUseCase from '@/domain/usecases/RegisterWithEmailPasswordUseCase';
import RedisUseCase from '@/domain/usecases/RedisUseCase';
import SendOtpEmailUseCase from '@/domain/usecases/SendOtpEmailUseCase';
import VerifyOtpUseCase from '@/domain/usecases/VerifyOtpUseCase';
import ForgotPasswordUseCase from '@/domain/usecases/ForgotPasswordUseCase';
import UserUseCase from '@/domain/usecases/UserUseCase';

// Khởi tạo repositories
const authRepository = new AuthRepository();
const redisRepository = new RedisRepository();
const userRepository = new UserRepository();

// Khởi tạo instances
export const loginWithGoogleUseCase = new LoginWithGoogleUseCase(authRepository);
export const loginWithEmailPasswordUseCase = new LoginWithEmailPasswordUseCase(authRepository);
export const refreshTokenUseCase = new RefreshTokenUseCase(authRepository);
export const registerWithEmailPasswordUseCase = new RegisterWithEmailPasswordUseCase(authRepository);
export const redisUseCase = new RedisUseCase(redisRepository);
export const sendOtpEmailUseCase = new SendOtpEmailUseCase(authRepository);
export const verifyOtpUseCase = new VerifyOtpUseCase(authRepository);
export const forgotPasswordUseCase = new ForgotPasswordUseCase(authRepository);
export const userUseCase = new UserUseCase(userRepository);
