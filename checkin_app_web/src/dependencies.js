// src/dependencies.js
// File này đóng vai trò là một "Dependency Injection Container" đơn giản.
// Nó là nơi duy nhất trong ứng dụng chịu trách nhiệm khởi tạo và "lắp ráp" các lớp
// từ tầng infrastructure và domain (Repositories, Use Cases, Services, etc.)

// --- Import các lớp triển khai cụ thể từ tầng Infrastructure ---
import AuthRepository from '@/infrastructure/repositories/AuthRepository';

// --- Import các lớp Use Case từ tầng Domain ---
import LoginWithGoogleUseCase from '@/domain/usecases/LoginWithGoogleUseCase';

// ===================================================================
// KHỞI TẠO VÀ LẮP RÁP (COMPOSITION ROOT)
// ===================================================================

// 1. Khởi tạo các Repository
const authRepository = new AuthRepository();

// 2. Khởi tạo các Use Case và "tiêm" (inject) các dependency cần thiết vào chúng
export const loginWithGoogleUseCase = new LoginWithGoogleUseCase(authRepository);

/*
Khi ứng dụng của bạn lớn hơn, bạn có thể khởi tạo và export thêm các use case khác từ đây:

import LogoutUseCase from '@/domain/usecases/LogoutUseCase';
export const logoutUseCase = new LogoutUseCase(authRepository);

*/
