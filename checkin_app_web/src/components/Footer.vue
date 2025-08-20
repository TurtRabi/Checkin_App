<template>
  <footer class="footer">
    <div class="footer-content container">
      <div class="footer-section about">
        <div class="logo-text">
          <img src="@/assets/Logo.png" alt="Logo" class="footer-logo"/>
          <span>Google Check-in</span>
        </div>
        <p>
          Đơn giản hóa hành trình khám phá và check-in của bạn với sức mạnh của Google. Gọn gàng, hiệu quả và đáng tin cậy.
        </p>
        <div class="map-placeholder-container">
            <div class="map-pin-large"></div>
        </div>
      </div>

      <div class="footer-section social-media">
        <h2>Theo dõi chúng tôi</h2>
        <p>Kết nối với chúng tôi trên các mạng xã hội để không bỏ lỡ thông tin cập nhật mới nhất.</p>
        <ul class="social-list">
          <li>
            <a href="#" target="_blank">
              <i class="fab fa-facebook-f"></i>
              <span>Facebook</span>
            </a>
          </li>
          <li>
            <a href="#" target="_blank">
              <i class="fab fa-instagram"></i>
              <span>Instagram</span>
            </a>
          </li>
          <li>
            <a href="#" target="_blank">
              <i class="fab fa-twitter"></i>
              <span>Twitter</span>
            </a>
          </li>
        </ul>
      </div>

      <div class="footer-section contact">
        <h2>Liên hệ</h2>
        <div class="contact-item">
          <span class="material-icons">mail</span>
          <a href="mailto:contact@googlecheckin.com">contact@googlecheckin.com</a>
        </div>
        <div class="contact-item">
          <span class="material-icons">phone</span>
          <a href="tel:+1234567890">+1 (234) 567-890</a>
        </div>
      </div>

    </div>

    <div class="footer-bottom">
      &copy; {{ new Date().getFullYear() }} Google Check-in | Tất cả quyền được bảo lưu.
    </div>
  </footer>
</template>

<script setup>
</script>

<style scoped>
/* Nhập font Poppins từ Google Fonts để giao diện chuyên nghiệp hơn */
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap');
/* Nhập icon từ Google Fonts và Font Awesome */
@import url('https://fonts.googleapis.com/icon?family=Material+Icons');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css');

.footer {
  font-family: 'Poppins', sans-serif;
  background-color: #1e2a3a;
  color: #a9b3c1;
  padding-top: 1.5rem;
}

.footer-content.container {
  display: grid;
  grid-template-columns: 1.5fr 1fr 1fr; /* Bố cục 3 cột tùy chỉnh */
  gap: 2rem; /* Giảm gap */
  max-width: 1200px; /* Giảm max-width */
  margin: 0 auto;
  padding-bottom: 2rem;
}

.footer-section h2 {
  color: #ffffff;
  margin-bottom: 1.5rem;
  font-size: 1.2rem;
  font-weight: 600;
  position: relative;
}

.footer-section h2::after {
  content: '';
  position: absolute;
  left: 0;
  bottom: -8px;
  width: 50px;
  height: 2px;
  background-color: #1fcd87;
  transition: transform 0.3s ease-in-out;
}

.footer-section p {
    margin-bottom: 1rem;
    line-height: 1.7;
    font-weight: 300;
}

.logo-text {
  display: flex;
  align-items: center;
  margin-bottom: 1rem;
  font-size: 1.5rem;
  font-weight: bold;
  color: #ffffff;
}

.footer-logo {
  width: 40px;
  height: 40px;
  margin-right: 0.5rem;
}

.footer-section a {
  color: #a9b3c1;
  text-decoration: none;
  transition: all 0.3s cubic-bezier(0.215, 0.610, 0.355, 1);
  font-weight: 400;
}

.footer-section a:hover {
  color: #1fcd87;
}

.contact-item, .social-list li a {
  display: flex;
  align-items: center;
  margin-bottom: 1rem;
  font-weight: 300;
}

.contact-item span, .social-list i {
  margin-right: 1rem;
  color: #1fcd87;
  font-size: 1.2rem;
  line-height: 1;
  width: 20px;
  text-align: center;
}

.contact-item a:hover, .social-list a:hover {
    transform: translateX(5px);
}

.social-list {
    list-style: none;
    padding: 0;
}

.map-placeholder-container {
    height: 150px; /* Giảm chiều cao */
    background-color: #e5e3df;
    margin-top: 1.5rem; /* Thêm margin top */
    position: relative;
    overflow: hidden;
    border-radius: 8px;
    background-image: 
        linear-gradient(rgba(0,0,0,0.05) 1px, transparent 1px),
        linear-gradient(90deg, rgba(0,0,0,0.05) 1px, transparent 1px);
    background-size: 15px 15px; /* Giảm kích thước lưới */
    display: flex;
    justify-content: center;
    align-items: center;
}

.map-pin-large {
    width: 30px; /* Giảm kích thước pin */
    height: 30px;
    border-radius: 50% 50% 50% 0;
    background: #1fcd87;
    position: absolute;
    transform: rotate(-45deg);
    border: 2px solid white;
}

.map-pin-large::after {
    content: '';
    width: 8px; /* Giảm kích thước chấm */
    height: 8px;
    margin: auto;
    background: white;
    border-radius: 50%;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
}

.footer-bottom {
  text-align: center;
  padding-top: 2rem; 
  border-top: 1px solid #2a3a4b;
  max-width: 1200px;
  margin: 0 auto;
  color: #6c757d;
  font-weight: 300;
}

@media (max-width: 992px) {
    .footer-content.container {
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    }
}

@media (max-width: 768px) {
  .footer-section {
    text-align: center;
  }
  
  .footer-section h2::after {
    left: 50%;
    transform: translateX(-50%);
  }

  .contact-item, .social-list li a {
      justify-content: center;
  }
}
</style>
