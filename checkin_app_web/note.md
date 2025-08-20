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
