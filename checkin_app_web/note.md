### Giải thích chi tiết luồng đăng nhập Google (Line-by-Line)

Đây là phân tích chi tiết về luồng đăng nhập, đi qua từng dòng code quan trọng và giải thích **tại sao nó lại được viết như vậy** trong bối cảnh kiến trúc của dự án.

---

### Bắt đầu tại: `src/application/pages/LoginPage.vue`

Đây là tầng giao diện, nơi người dùng bắt đầu mọi thứ.

**Code:**
```html
<GoogleLogin :callback="handleCredentialResponse" />
```

*   **Dòng này làm gì?**
    Nó sử dụng component `<GoogleLogin>` từ thư viện bên ngoài. Quan trọng nhất là prop `:callback`, nó ra lệnh: "Sau khi người dùng đăng nhập với Google thành công và Google trả về kết quả, hãy gọi hàm `handleCredentialResponse` của tôi".
*   **Tại sao lại viết vậy?**
    Để **ủy quyền** công việc phức tạp (mở popup, giao tiếp với Google, nhận token) cho thư viện. Component `LoginPage` không cần biết chi tiết đó, nó chỉ cần biết "khi nào xong thì gọi hàm này". Điều này giữ cho code của component gọn gàng và tập trung vào việc xử lý kết quả.

---

**Code:**
```javascript
// Bên trong methods của LoginPage.vue
async handleCredentialResponse(response) {
    // ...
}
```

*   **Dòng này làm gì?**
    Đây là hàm được gọi sau khi callback ở trên được thực thi. `response` là một object chứa `credential` - một chuỗi token (JWT) mà Google cung cấp để xác nhận danh tính người dùng.
*   **Tại sao lại có `async`?**
    Bởi vì ngay sau đây, chúng ta sẽ gọi một chuỗi các hành động bất đồng bộ (gọi store -> gọi API). Từ khóa `async` cho phép chúng ta dùng `await` ở các dòng dưới, giúp code chạy tuần tự và dễ đọc hơn rất nhiều so với việc dùng `.then().catch()` lồng nhau.

---

**Code:**
```javascript
const authStore = useAuthStore(); 
```

*   **Dòng này làm gì?**
    Lấy ra một "instance" (một bản thể) của `authStore` mà chúng ta đã định nghĩa bằng Pinia.
*   **Tại sao lại viết vậy?**
    Theo nguyên tắc **quản lý trạng thái tập trung**, component không nên tự mình xử lý logic hay giữ trạng thái quan trọng. Nó phải **nhờ Store làm việc đó**. `LoginPage` chỉ là người đưa tin, còn `authStore` mới là trung tâm xử lý.

---

**Code:**
```javascript
await authStore.handleGoogleLogin(response.credential);
```

*   **Dòng này làm gì?**
    Nó gọi action `handleGoogleLogin` trong `authStore` và truyền `credential` (token của Google) vào làm tham số. Từ khóa `await` bắt `LoginPage` phải **chờ** cho đến khi `authStore` xử lý xong xuôi việc đăng nhập (bao gồm cả việc gọi API và nhận kết quả).
*   **Tại sao lại viết vậy?**
    Đây là ví dụ điển hình của việc **phân tách trách nhiệm**. `LoginPage` nói: "Này Store, đây là token, việc của anh là xử lý nó", và nó không cần quan tâm Store làm điều đó như thế nào. Điều này giúp `LoginPage` không bị "ô nhiễm" bởi logic gọi API.

---

**Code:**
```javascript
if (authStore.isLoggedIn) {
  this.$router.push({ name: 'Home' });
}
```

*   **Dòng này làm gì?**
    Sau khi `await` ở trên hoàn tất, `authStore` đã cập nhật xong trạng thái. Dòng này kiểm tra trạng thái `isLoggedIn` ngay trong store. Nếu là `true`, nó sẽ điều hướng người dùng đến trang `Home`.
*   **Tại sao lại viết vậy?**
    Component **phản ứng** dựa trên sự thay đổi của **trạng thái (state)**. Luồng logic là: "Hành động -> Thay đổi State -> Phản ứng lại State mới". Đây là nguyên tắc cốt lõi của các framework hiện đại như Vue.

---

### Chuyển tiếp đến: `src/application/stores/auth.js`

Đây là trung tâm quản lý trạng thái, nhận yêu cầu từ `LoginPage`.

**Code:**
```javascript
async function handleGoogleLogin(googleToken) {
    // ...
}
```

*   **Dòng này làm gì?**
    Đây là một "action" của store, nó nhận `googleToken` từ component.
*   **Tại sao lại có `async`?**
    Bởi vì nó sắp gọi đến `UseCase`, một hành động bất đồng bộ khác.

---

**Code:**
```javascript
const userData = await loginWithGoogleUseCase.execute(googleToken);
```

*   **Dòng này làm gì?**
    Đây là dòng code **quan trọng nhất** thể hiện kiến trúc của dự án. Store không tự mình gọi API. Nó **ủy quyền** việc xử lý logic nghiệp vụ cho một `UseCase`. Nó gọi phương thức `execute` của `loginWithGoogleUseCase` và chờ kết quả trả về.
*   **Tại sao lại viết vậy?**
    Đây là trái tim của **Clean Architecture**. Logic nghiệp vụ ("quy trình đăng nhập Google") được đóng gói riêng trong `UseCase`. Điều này giúp:
    1.  Store không bị phình to với logic.
    2.  `UseCase` có thể được tái sử dụng ở nơi khác.
    3.  `UseCase` cực kỳ dễ test mà không cần đến UI hay Store.

---

**Code:**
```javascript
localStorage.setItem('authToken', userData.token); 
isLoggedIn.value = true;
```

*   **Dòng này làm gì?**
    1.  Lưu `token` (do **backend của bạn** cấp, không phải token của Google) vào `localStorage` của trình duyệt.
    2.  Cập nhật trạng thái `isLoggedIn` trong store thành `true`.
*   **Tại sao lại viết vậy?**
    1.  `localStorage` dùng để **duy trì phiên đăng nhập**. Khi người dùng tải lại trang (F5), ứng dụng có thể đọc token từ đây để biết họ vẫn đang đăng nhập.
    2.  Cập nhật `isLoggedIn.value = true` sẽ khiến **mọi component** đang sử dụng state này tự động render lại (ví dụ: Header sẽ hiện tên người dùng thay vì nút "Đăng nhập").

---

### Chuyển tiếp đến: `src/domain/usecases/LoginWithGoogleUseCase.js`

Đây là "bộ não" chứa logic nghiệp vụ.

**Code:**
```javascript
constructor(authRepository) {
  this.authRepository = authRepository;
}
```

*   **Dòng này làm gì?**
    Constructor này không tự tạo `new AuthRepository()`. Thay vào đó, nó nhận một `authRepository` từ bên ngoài.
*   **Tại sao lại viết vậy?**
    Đây là nguyên tắc **Dependency Injection (Tiêm phụ thuộc)**. `UseCase` không bị phụ thuộc cứng vào một `AuthRepository` cụ thể. Khi viết test, chúng ta có thể "tiêm" vào một `MockAuthRepository` (một repository giả) để `UseCase` hoạt động mà không cần gọi API thật.

---

**Code:**
```javascript
return await this.authRepository.loginWithGoogle(googleToken);
```

*   **Dòng này làm gì?**
    `UseCase` tiếp tục **ủy quyền** công việc liên quan đến dữ liệu cho `Repository`. Nó nói: "Này Repository, tôi cần đăng nhập Google với token này, hãy làm và trả kết quả".
*   **Tại sao lại viết vậy?**
    `UseCase` chỉ điều phối, nó không nên biết dữ liệu được lấy từ đâu (từ API, từ cache, hay từ database). Việc đó là của `Repository`. Sự phân tách này làm cho logic nghiệp vụ trở nên trong sáng.

---

### Cuối cùng tại: `src/infrastructure/repositories/AuthRepository.js`

Đây là lớp "tay chân", nơi thực sự làm việc với API.

**Code:**
```javascript
async loginWithGoogle(googleToken) {
    // ...
}
```

*   **Dòng này làm gì?**
    Đây là phương thức triển khai cụ thể cho việc đăng nhập. Nó thực hiện "lời hứa" đã được định nghĩa trong `IAuthRepository` (interface).
*   **Tại sao lại viết vậy?**
    Đây là nơi logic trừu tượng ("đăng nhập bằng google") được chuyển thành hành động cụ thể ("gọi API POST đến endpoint `/social-login`").

---

**Code:**
```javascript
const response = await apiClient.post("/social-login", { provider: 'Google',token: googleToken });
```

*   **Dòng này làm gì?**
    Sử dụng `apiClient` (là `axios` đã được cấu hình sẵn) để gửi một request `POST` đến backend.
*   **Tại sao lại viết vậy?**
    Việc sử dụng một `apiClient` chung giúp tái sử dụng các cấu hình quan trọng (`baseURL`, `headers`, và các `interceptors` - bộ chặn để xử lý lỗi chung hoặc tự động gắn token) mà không phải lặp lại code ở mọi nơi gọi API.

---

**Code:**
```javascript
return response.data;
```

*   **Dòng này làm gì?**
    `axios` trả về một object `response` lớn. Dữ liệu mà backend trả về thực sự nằm trong thuộc tính `response.data`. Chúng ta chỉ cần trả về phần dữ liệu này cho các tầng trên.
*   **Tại sao lại viết vậy?**
    Các tầng trên như `UseCase` hay `Store` không cần quan tâm đến những thứ như status code, headers của HTTP response. Chúng chỉ cần **dữ liệu (payload)** để làm việc.


======================================================================
### HƯỚNG DẪN: THÊM CHỨC NĂNG ĐĂNG NHẬP BẰNG EMAIL & MẬT KHẨU
======================================================================

Phần này sẽ hướng dẫn bạn tích hợp chức năng đăng nhập mới vào hệ thống hiện tại, tuân thủ theo đúng kiến trúc đã có.

---

### Bước 1: Cập nhật Interface `IAuthRepository.js`

Đầu tiên, chúng ta cần định nghĩa chức năng mới trong "bản thiết kế" (interface) của repository.

**Nội dung file `src/domain/repositories/IAuthRepository.js`:**
```javascript
/**
 * Lớp này hoạt động như một Interface (bản thiết kế) cho tất cả các AuthRepository.
 * Nó định nghĩa các phương thức bắt buộc mà mọi lớp repository xác thực phải triển khai.
 * Nếu một lớp kế thừa từ IAuthRepository mà không triển khai các phương thức này,
 * một lỗi sẽ được ném ra khi phương thức đó được gọi.
 */
export default class IAuthRepository {
  /**
   * @param {string} googleToken The token received from Google Sign-In.
   * @returns {Promise<any>} A promise that resolves with user data and system token.
   */
  loginWithGoogle(googleToken) {
    throw new Error("AuthRepository must implement loginWithGoogle()");
  }

  /**
   * @param {string} email The user's email.
   * @param {string} password The user's password.
   * @returns {Promise<any>} A promise that resolves with user data and system token.
   */
  loginWithEmailPassword(email, password) {
    throw new Error("AuthRepository must implement loginWithEmailPassword()");
  }
}
```

---

### Bước 2: Tạo Use Case `LoginWithEmailPasswordUseCase.js`

Tạo một file mới để chứa logic cho việc đăng nhập bằng email. File này không giao tiếp trực tiếp với API mà chỉ điều phối.

**Tạo file mới `src/domain/usecases/LoginWithEmailPasswordUseCase.js` với nội dung:**
```javascript
export default class LoginWithEmailPasswordUseCase {
  constructor(authRepository) {
    this.authRepository = authRepository;
  }

  async execute(email, password) {
    if (!email || !password) {
      throw new Error("Email and password are required");
    }
    return await this.authRepository.loginWithEmailPassword(email, password);
  }
}
```

---

### Bước 3: Cập nhật `AuthRepository.js` để triển khai phương thức mới

Bây giờ, chúng ta sẽ viết code để thực sự gọi API đăng nhập trong `AuthRepository`.

**Nội dung file `src/infrastructure/repositories/AuthRepository.js`:**
```javascript
import IAuthRepository from "@/domain/repositories/IAuthRepository";
import apiClient from "@/infrastructure/api/apiClient";

export default class AuthRepository extends IAuthRepository {
  async loginWithGoogle(googleToken) {
    try {
      const response = await apiClient.post("/social-login", { provider: 'Google',devide:'web',token: googleToken });
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.loginWithGoogle:",
        error.response?.data || error.message
      );
      throw error;
    }
  }

  /**
   * Gửi email và password lên backend để xác thực.
   * @param {string} email
   * @param {string} password
   * @returns {Promise<any>} Dữ liệu trả về từ backend.
   */
  async loginWithEmailPassword(email, password) {
    try {
      // Giả sử endpoint của bạn là /login
      const response = await apiClient.post("/login", { email, password, devide: 'web' });
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.loginWithEmailPassword:",
        error.response?.data || error.message
      );
      throw error;
    }
  }
}
```

---

### Bước 4: Cập nhật `dependencies.js` để "lắp ráp" Use Case mới

Thêm use case mới vào "nhà máy lắp ráp" để nó sẵn sàng được sử dụng trong ứng dụng.

**Nội dung file `src/dependencies.js`:**
```javascript
// src/dependencies.js
import AuthRepository from '@/infrastructure/repositories/AuthRepository';
import LoginWithGoogleUseCase from '@/domain/usecases/LoginWithGoogleUseCase';
import LoginWithEmailPasswordUseCase from '@/domain/usecases/LoginWithEmailPasswordUseCase'; // <-- IMPORT MỚI

const authRepository = new AuthRepository();

export const loginWithGoogleUseCase = new LoginWithGoogleUseCase(authRepository);

// <-- EXPORT USE CASE MỚI -->
export const loginWithEmailPasswordUseCase = new LoginWithEmailPasswordUseCase(authRepository);
```

---

### Bước 5: Cập nhật Store `auth.js`

Thêm một action mới vào store để xử lý trạng thái cho luồng đăng nhập bằng email.

**Nội dung file `src/application/stores/auth.js`:**
```javascript
import { defineStore } from 'pinia';
import { ref } from 'vue';
// Import cả hai use case
import { loginWithGoogleUseCase, loginWithEmailPasswordUseCase } from '@/dependencies';

export const useAuthStore = defineStore('auth', () => {
    const isLoggedIn = ref(false);
    const user = ref(null);
    const isLoading = ref(false); // Thêm trạng thái loading
    const error = ref(null); // Thêm trạng thái lỗi

    async function handleGoogleLogin(googleToken) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithGoogleUseCase.execute(googleToken);
            user.value = userData.data.user;
            localStorage.setItem('authRefesh', userData.data.authRefresh);
            localStorage.setItem('authToken',userData.data.authToken);
            isLoggedIn.value = true;
        } catch (err) {
            console.error('Failed to login with Google:', err);
            error.value = err.response?.data?.message || 'Đăng nhập Google thất bại.';
            isLoggedIn.value = false;
            user.value = null;
        } finally {
            isLoading.value = false;
        }
    }

    async function handleEmailPasswordLogin(email, password) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithEmailPasswordUseCase.execute(email, password);
            user.value = userData.data.user;
            localStorage.setItem('authRefesh', userData.data.authRefresh);
            localStorage.setItem('authToken',userData.data.authToken);
            isLoggedIn.value = true;
            return true; // Trả về true khi thành công
        } catch (err) {
            console.error('Failed to login with email/password:', err);
            error.value = err.response?.data?.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại tài khoản và mật khẩu.';
            isLoggedIn.value = false;
            user.value = null;
            return false; // Trả về false khi thất bại
        } finally {
            isLoading.value = false;
        }
    }

    function logout() {
        isLoggedIn.value = false;
        user.value = null;
        localStorage.removeItem('authToken');
        localStorage.removeItem('authRefesh');
    }

    return { isLoggedIn, user, isLoading, error, handleGoogleLogin, handleEmailPasswordLogin, logout };
});
```

---

### Bước 6: Cập nhật `LoginPage.vue` để kết nối mọi thứ

Cuối cùng, cập nhật giao diện để gọi action trong store và hiển thị trạng thái (loading, error) cho người dùng.

**Phần `<script>` của file `src/application/pages/LoginPage.vue`:**
```javascript
import { useAuthStore } from "@/application/stores/auth";
import { GoogleLogin } from "vue3-google-login";
import { ref, computed } from 'vue'; // Import thêm computed

export default {
  name: "LoginPage",
  components: {
    GoogleLogin,
  },
  setup() {
    const authStore = useAuthStore();
    const username = ref('');
    const password = ref('');
    const showPassword = ref(false);

    // Lấy trạng thái loading và error từ store
    const isLoading = computed(() => authStore.isLoading);
    const errorMessage = computed(() => authStore.error);

    const handleLogin = async () => {
      const success = await authStore.handleEmailPasswordLogin(username.value, password.value);
      if (success) {
        // this.$router không có sẵn trong setup, cần inject hoặc dùng cách khác
        // Tạm thời để trống, sẽ xử lý ở phần template
      }
    };

    const handleCredentialResponse = async (response) => {
      await authStore.handleGoogleLogin(response.credential);
      if (authStore.isLoggedIn) {
        // Xử lý chuyển trang
      }
    };

    return { 
      username, 
      password, 
      showPassword, 
      isLoading, 
      errorMessage, 
      handleLogin, 
      handleCredentialResponse,
      authStore // Trả về authStore để template có thể truy cập isLoggedIn
    };
  },
  watch: {
    // Theo dõi trạng thái đăng nhập và chuyển trang
    'authStore.isLoggedIn'(isLoggedIn) {
      if (isLoggedIn) {
        this.$router.push({ name: 'Home' });
      }
    }
  }
};
```

**Phần `<template>` của file `src/application/pages/LoginPage.vue`:**

Bạn cần sửa lại form để binding đúng với các biến trong `setup` và hiển thị lỗi/loading.

```html
<template>
  <!-- ... -->
      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="username" class="form-label">Tên đăng nhập hoặc Email</label>
          <input
            type="text"
            id="username"
            v-model="username" 
            class="form-input"
            required
            autocomplete="username"
          />
        </div>

        <div class="form-group relative">
          <label for="password" class="form-label">Mật khẩu</label>
          <input
            :type="showPassword ? 'text' : 'password'"
            id="password"
            v-model="password" 
            class="form-input pr-10"
            required
            autocomplete="current-password"
          />
        </div>

        <!-- Hiển thị thông báo lỗi -->
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <!-- ... -->

        <button type="submit" class="login-button" :disabled="isLoading">
          {{ isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
        </button>
      </form>
  <!-- ... -->
</template>

<style scoped>
/* Thêm style cho thông báo lỗi */
.error-message {
  color: #D32F2F; /* Màu đỏ đậm */
  background-color: #FFCDD2; /* Màu nền đỏ nhạt */
  border: 1px solid #D32F2F;
  border-radius: 8px;
  padding: 12px;
  margin-bottom: 1rem; 
  text-align: center;
  font-size: 0.95rem;
}
</style>
```

---

**KẾT THÚC.**

Sau khi áp dụng tất cả các thay đổi này, chức năng đăng nhập bằng email và mật khẩu của bạn sẽ hoàn chỉnh và tuân thủ đúng theo kiến trúc đã thiết kế. Bạn chỉ cần đảm bảo rằng API backend của bạn có một endpoint `/login` nhận `email`, `password` và trả về dữ liệu đúng định dạng.


======================================================================
### HƯỚNG DẪN: THÊM CHỨC NĂNG "GHI NHỚ TÔI" (TỰ ĐỘNG ĐĂNG NHẬP)
======================================================================

Phần này sẽ hướng dẫn bạn cách triển khai chức năng tự động đăng nhập khi người dùng quay lại ứng dụng.

---

### Bước 1: Cập nhật `LoginPage.vue`

Chúng ta cần lấy giá trị từ checkbox "Ghi nhớ tôi".

**Phần `<script>` của file `src/application/pages/LoginPage.vue`:**
```javascript
// ... imports
export default {
  // ... name, components
  setup() {
    // ... các ref khác
    const rememberMe = ref(false); // <-- THÊM DÒNG NÀY

    const handleLogin = async () => {
      // Truyền giá trị rememberMe.value vào action của store
      const success = await authStore.handleEmailPasswordLogin(username.value, password.value, rememberMe.value);
      if (success) {
        // Chuyển trang đã được xử lý bằng watch
      }
    };

    // ...
    return { 
      // ... các giá trị trả về khác
      rememberMe, // <-- THÊM DÒNG NÀY
    };
  },
  // ... watch
};
```

**Phần `<template>` của file `src/application/pages/LoginPage.vue`:**
```html
<!-- ... -->
<div class="remember-me">
    <input type="checkbox" id="remember" name="remember" class="checkbox-input" v-model="rememberMe" /> <!-- <-- THÊM v-model="rememberMe" -->
    <label for="remember" class="checkbox-label">Ghi nhớ tôi</label>
</div>
<!-- ... -->
```

---

### Bước 2: Thêm chức năng `refreshToken` vào Repository

Chúng ta cần một phương thức để làm mới token.

**Cập nhật `src/domain/repositories/IAuthRepository.js`:**
```javascript
// ...
export default class IAuthRepository {
  // ... loginWithGoogle, loginWithEmailPassword

  /**
   * @param {string} refreshToken The refresh token.
   * @returns {Promise<any>} A promise that resolves with new token data.
   */
  refreshToken(refreshToken) {
    throw new Error("AuthRepository must implement refreshToken()");
  }
}
```

**Cập nhật `src/infrastructure/repositories/AuthRepository.js`:**
```javascript
// ...
export default class AuthRepository extends IAuthRepository {
  // ... loginWithGoogle, loginWithEmailPassword

  async refreshToken(refreshToken) {
    try {
      // Giả sử endpoint của bạn là /auth/refresh-token
      const response = await apiClient.post("/auth/refresh-token", { refreshToken });
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.refreshToken:",
        error.response?.data || error.message
      );
      throw error;
    }
  }
}
```

---

### Bước 3: Tạo `RefreshTokenUseCase` và cập nhật `dependencies.js`

Tạo use case mới cho việc làm mới token.

**Tạo file mới `src/domain/usecases/RefreshTokenUseCase.js`:**
```javascript
export default class RefreshTokenUseCase {
  constructor(authRepository) {
    this.authRepository = authRepository;
  }

  async execute(refreshToken) {
    if (!refreshToken) {
      throw new Error("Refresh token is required");
    }
    return await this.authRepository.refreshToken(refreshToken);
  }
}
```

**Cập nhật `src/dependencies.js`:**
```javascript
// ... các import khác
import RefreshTokenUseCase from '@/domain/usecases/RefreshTokenUseCase'; // <-- IMPORT MỚI

// ... khởi tạo repository

// ... các export use case khác
export const refreshTokenUseCase = new RefreshTokenUseCase(authRepository); // <-- EXPORT MỚI
```

---

### Bước 4: Cập nhật Store `auth.js` với logic tự động đăng nhập

Đây là phần quan trọng nhất, thêm action `tryAutoLogin` và cập nhật các action khác.

**Nội dung file `src/application/stores/auth.js`:**
```javascript
import { defineStore } from 'pinia';
import { ref } from 'vue';
// Import thêm use case mới
import { loginWithGoogleUseCase, loginWithEmailPasswordUseCase, refreshTokenUseCase } from '@/dependencies';

export const useAuthStore = defineStore('auth', () => {
    const isLoggedIn = ref(false);
    const user = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    // Hàm trợ giúp để set dữ liệu đăng nhập
    function setAuthData(authResult, remember) {
        user.value = authResult.data.user;
        // Chỉ lưu vào localStorage nếu người dùng yêu cầu "Ghi nhớ tôi"
        if (remember) {
            localStorage.setItem('authRefresh', authResult.data.authRefresh);
        }
        localStorage.setItem('authToken', authResult.data.authToken); // authToken vẫn lưu để dùng ngay
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
        // ... (giữ nguyên logic cũ, nhưng gọi setAuthData)
        // Giả sử đăng nhập Google luôn "ghi nhớ"
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
        } catch (error) {
            console.error('Failed to login with Google:', error);
            isLoggedIn.value = false;
            user.value = null;
            error.value = error.message || 'Login failed';
        }finally{
            isLoading.value = false;
        }
        setAuthData(userData, true); 
    }

    async function handleEmailPasswordLogin(email, password, remember) {
        isLoading.value = true;
        error.value = null;
        try {
            const userData = await loginWithEmailPasswordUseCase.execute(email, password);
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
        clearAuthData(); // <-- SỬ DỤNG HÀM TRỢ GIÚP
    }

    async function tryAutoLogin() {
        const refresh_token = localStorage.getItem('authRefresh');
        if (!refresh_token) {
            return; // Không có refresh token, không cần làm gì cả
        }

        isLoading.value = true;
        try {
            const newAuthData = await refreshTokenUseCase.execute(refresh_token);
            setAuthData(newAuthData, true); // <-- LƯU LẠI DỮ LIỆU MỚI
        } catch (err) {
            console.error("Auto login failed, token might be expired.", err);
            clearAuthData(); // Xóa token hỏng
        } finally {
            isLoading.value = false;
        }
    }

    return { isLoggedIn, user, isLoading, error, handleGoogleLogin, handleEmailPasswordLogin, logout, tryAutoLogin };
});
```

---

### Bước 5: Kích hoạt `tryAutoLogin` khi ứng dụng khởi động

Chúng ta cần gọi action `tryAutoLogin` một lần duy nhất khi ứng dụng được tải. File `App.vue` là nơi lý tưởng để làm việc này.

**Cập nhật file `src/App.vue`:**
```vue
<script>
import { onMounted } from 'vue';
import { useAuthStore } from '@/application/stores/auth';

export default {
  name: 'App',
  setup() {
    const authStore = useAuthStore();

    // onMounted là hook được gọi sau khi component đã được render lần đầu
    onMounted(() => {
      authStore.tryAutoLogin();
    });

    return {};
  }
}
</script>

<template>
  <div id="app">
    <router-view />
  </div>
</template>

<style>
/* CSS của bạn */
</style>
```

---
**KẾT THÚC.**

Với những thay đổi này, chức năng "Ghi nhớ tôi" đã hoàn tất. Khi người dùng tick vào ô đó và đăng nhập, lần sau khi mở lại trang web, họ sẽ được tự động đăng nhập.