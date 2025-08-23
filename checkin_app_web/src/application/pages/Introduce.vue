<template>
  <div class="introduce-page">
    <!-- Hero -->
    <section class="hero-section">
      <div class="stars">
        <div
          class="star"
          v-for="n in 100"
          :key="n"
          :style="getStarStyle()"
        ></div>
      </div>
      <div class="container hero-content">
        <h1 class="hero-title">Khám phá & Kết nối cùng Google Check-in</h1>
        <p class="hero-subtitle">
          Chào mừng bạn đến với <strong>Google Check-in</strong> – người bạn
          đồng hành khám phá những điều thú vị ngay gần bạn.
        </p>
      </div>
    </section>

    <!-- Features -->
    <section class="features-section container">
      <h2 class="section-title">Các tính năng nổi bật</h2>
      <div class="features-grid">
        <div class="feature-item animate-on-scroll" v-for="(f, idx) in features" :key="idx">
          <img :src="f.img" :alt="f.title" class="feature-icon" />
          <h3>{{ f.title }}</h3>
          <p>{{ f.desc }}</p>
        </div>
      </div>
    </section>

    <!-- Goals -->
    <section class="goals-section">
      <div class="container goals-content">
        <div class="goals-text animate-on-scroll">
          <h2 class="section-title">Mục tiêu của chúng tôi</h2>
          <ul>
            <li><strong>Làm giàu kiến thức địa phương:</strong> Giúp bạn học hỏi và khám phá những điều mới mẻ về thành phố, vùng đất mà bạn đang sống.</li>
            <li><strong>Tạo ra những trải nghiệm phong phú hơn:</strong> Thúc đẩy bạn ra ngoài, trải nghiệm mới lạ và kết nối với cộng đồng.</li>
            <li><strong>Biến khám phá thành niềm vui:</strong> Với tính năng sưu tầm thẻ, mỗi hành trình đều trở nên thú vị và mang lại cảm giác thành tựu.</li>
          </ul>
          <p>
            Hãy bắt đầu hành trình của bạn với
            <strong>Google Check-in</strong> ngay hôm nay và biến mỗi ngày
            thành một chuyến phiêu lưu!
          </p>
        </div>
        <div class="goals-image animate-on-scroll">
          <img
            src="https://placehold.co/450x280/E9F8F8/42B983?text=Our+Goal"
            alt="Hình minh họa mục tiêu"
          />
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { onMounted } from "vue";

const features = [
  {
    img: "https://picsum.photos/id/175/150/150",
    title: "Gợi ý thông minh, dẫn lối khám phá",
    desc: "Nhận gợi ý về du lịch, ẩm thực, văn hóa gần bạn. Hệ thống phân tích vị trí và sở thích để đưa ra điểm đến phù hợp."
  },
  {
    img: "https://picsum.photos/id/102/150/150",
    title: "Check-in và lưu giữ khoảnh khắc",
    desc: "Dễ dàng check-in tại địa điểm nổi tiếng. Mỗi lần check-in là một kỷ niệm, một dấu ấn trên bản đồ khám phá."
  },
  {
    img: "https://picsum.photos/id/177/150/150",
    title: "Sưu tầm thẻ địa điểm độc đáo",
    desc: "Mỗi check-in thành công mang lại 1 thẻ kỹ thuật số chứa thông tin thú vị, giúp bạn tích lũy kiến thức và trải nghiệm."
  }
];

onMounted(() => {
  // Scroll animation
  const observer = new IntersectionObserver(
    entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add("active");
          observer.unobserve(entry.target);
        }
      });
    },
    { threshold: 0.2 }
  );
  document.querySelectorAll(".animate-on-scroll").forEach(el => {
    observer.observe(el);
  });

  // 3D Tilt effect cho feature cards
  const cards = document.querySelectorAll(".feature-item");
  cards.forEach(card => {
    card.addEventListener("mousemove", e => {
      const rect = card.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const y = e.clientY - rect.top;
      const centerX = rect.width / 2;
      const centerY = rect.height / 2;
      const rotateX = ((y - centerY) / centerY) * 8;
      const rotateY = ((x - centerX) / centerX) * -8;
      card.style.transform = `rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale(1.05)`;
    });
    card.addEventListener("mouseleave", () => {
      card.style.transform = "rotateX(0) rotateY(0) scale(1)";
    });
  });
});

function getStarStyle() {
  const size = Math.random() * 2 + 1;
  const top = Math.random() * -100;
  const left = Math.random() * 100;
  const duration = Math.random() * 20 + 10;
  const delay = Math.random() * 10;
  const opacity = Math.random() * 0.8 + 0.2;
  return {
    width: `${size}px`,
    height: `${size}px`,
    top: `${top}vh`,
    left: `${left}%`,
    opacity,
    animationDuration: `${duration}s`,
    animationDelay: `${delay}s`
  };
}
</script>

<style scoped>
.introduce-page {
  font-family: var(--font-primary, "Inter", sans-serif);
  color: #222;
  line-height: 1.6;
}

/* Hero */
.hero-section {
  background: linear-gradient(-45deg, #1a2533, #0f2027, #203a43, #2c5364);
  background-size: 400% 400%;
  animation: gradientBG 15s ease infinite;
  color: #fff;
  padding: 7rem 2rem;
  text-align: center;
  position: relative;
  overflow: hidden;
}
@keyframes gradientBG {
  0% { background-position: 0% 50% }
  50% { background-position: 100% 50% }
  100% { background-position: 0% 50% }
}
.hero-title {
  font-size: 3rem;
  font-weight: 700;
  background: linear-gradient(90deg, #42b983, #00c6ff, #ff758c);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  animation: textGlow 3s ease-in-out infinite alternate;
}
.hero-subtitle {
  max-width: 700px;
  margin: 1rem auto 0;
  font-size: 1.2rem;
  color: #f1f1f1;
}
@keyframes textGlow {
  from { text-shadow: 0 0 10px rgba(66,185,131,0.6); }
  to { text-shadow: 0 0 20px rgba(0,198,255,0.9); }
}

/* Stars */
.stars {
  position: absolute;
  top: 0; left: 0;
  width: 100%; height: 100%;
  z-index: 0;
}
.star {
  position: absolute;
  background: white;
  border-radius: 50%;
  animation-name: fall;
  animation-timing-function: linear;
  animation-iteration-count: infinite;
}
@keyframes fall {
  from { transform: translateY(0vh) }
  to   { transform: translateY(110vh) }
}

/* Sections */
.section-title {
  font-size: 2rem;
  text-align: center;
  margin-bottom: 2.5rem;
  position: relative;
}
.section-title::after {
  content: '';
  position: absolute;
  bottom: -10px; left: 50%;
  transform: translateX(-50%);
  width: 80px; height: 3px;
  background-color: var(--color-primary, #42b983);
}

/* Features */
.features-section {
  padding: 5rem 1rem;
  background: #f9f9f9;
}
.features-grid {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 2rem;
}
.feature-item {
  flex: 1 1 280px;
  background: #fff;
  border-radius: 15px;
  padding: 2rem;
  text-align: center;
  box-shadow: 0 10px 20px rgba(0,0,0,0.08);
  transition: transform 0.2s ease, box-shadow 0.3s ease;
  animation: float 6s ease-in-out infinite;
  will-change: transform;
}
@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
}
.feature-icon {
  width: 100px;
  height: 100px;
  margin-bottom: 1rem;
  transition: transform 0.3s ease, filter 0.3s ease;
}
.feature-item:hover .feature-icon {
  transform: scale(1.15);
  filter: drop-shadow(0 0 10px rgba(66,185,131,0.7));
}

/* Goals */
.goals-section {
  background: #fff;
  padding: 5rem 1rem;
}
.goals-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 3rem;
  flex-wrap: wrap;
}
.goals-text {
  flex: 1 1 400px;
  opacity: 0;
  transform: translateX(-40px);
  transition: all 0.8s ease;
}
.goals-image {
  flex: 1 1 400px;
  text-align: center;
  opacity: 0;
  transform: translateX(40px);
  transition: all 0.8s ease;
}
.goals-text.active,
.goals-image.active {
  opacity: 1;
  transform: translateX(0);
}
.goals-text ul {
  list-style: none;
  padding: 0;
}
.goals-text li {
  margin-bottom: 1rem;
  padding-left: 25px;
  position: relative;
}
.goals-text li::before {
  content: "✓";
  color: var(--color-primary, #42b983);
  font-weight: bold;
  position: absolute;
  left: 0;
}

/* Responsive */
@media (max-width: 768px) {
  .hero-title { font-size: 2rem; }
  .goals-content { flex-direction: column; }
}
</style>
