<template>
    <div class="introduce-page">
        <div class="hero-section">
            <div class="stars">
                <div class="star" v-for="n in 100" :key="n" :style="getStarStyle()"></div>
            </div>
            <div class="container">
                <h1 class="animate__animated animate__fadeInDown">Khám phá và kết nối cùng Google Check-in</h1>
                <p class="animate__animated animate__fadeInUp">
                    Chào mừng bạn đến với <strong>Google Check-in</strong> – người bạn đồng hành khám phá những điều thú vị ngay gần bạn. Dự án của chúng tôi được tạo ra với mục tiêu giúp bạn khám phá và hiểu hơn về chính những nơi mình đang sống.
                </p>
            </div>
        </div>

        <div class="features-section">
            <div class="container">
                <h2>Các tính năng nổi bật</h2>
                <div class="features-grid">
                    <div class="feature-item animate__animated animate__fadeInUp">
                        <img src="https://picsum.photos/id/175/150/150" alt="Gợi ý địa điểm" class="feature-icon" />
                        <h3>Gợi ý thông minh, dẫn lối khám phá</h3>
                        <p>Với Google Check-in, bạn sẽ nhận được những gợi ý về các khu vực, địa điểm du lịch, ẩm thực, và văn hóa gần bạn. Hệ thống của chúng tôi sẽ phân tích vị trí và sở thích của bạn để đưa ra những điểm đến phù hợp, giúp bạn tiết kiệm thời gian tìm kiếm và bắt đầu chuyến đi ngay lập tức.</p>
                    </div>
                    <div class="feature-item animate__animated animate__fadeInUp">
                        <img src="https://picsum.photos/id/102/150/150" alt="Check-in và khám phá" class="feature-icon" />
                        <h3>Check-in và lưu giữ khoảnh khắc</h3>
                        <p>Dễ dàng check-in tại các địa điểm nổi tiếng, từ những quán cà phê ẩn mình đến các di tích lịch sử lâu đời. Mỗi lần check-in không chỉ là một kỷ niệm mà còn là một dấu ấn trên bản đồ khám phá của riêng bạn.</p>
                    </div>
                    <div class="feature-item animate__animated animate__fadeInUp">
                        <img src="https://picsum.photos/id/177/150/150" alt="Thẻ địa điểm" class="feature-icon" />
                        <h3>Sưu tầm thẻ địa điểm độc đáo</h3>
                        <p>Mỗi khi bạn check-in thành công tại một địa điểm, bạn sẽ nhận được một tấm thẻ kỹ thuật số chứa đựng thông tin thú vị về nơi đó. Những tấm thẻ này không chỉ là một món quà lưu niệm mà còn giúp bạn tích lũy kiến thức, hiểu sâu hơn về lịch sử, văn hóa và câu chuyện đằng sau mỗi địa danh.</p>
                    </div>
                </div>
            </div>
        </div>

        <div class="goals-section">
            <div class="flowing-background"></div>
            <div class="container">
                <h2>Mục tiêu của chúng tôi</h2>
                <div class="goals-content">
                    <div class="goals-text">
                        <ul>
                            <li><strong>Làm giàu kiến thức địa phương:</strong> Giúp bạn học hỏi và khám phá những điều mới mẻ về thành phố, vùng đất mà bạn đang sống.</li>
                            <li><strong>Tạo ra những trải nghiệm phong phú hơn:</strong> Thúc đẩy bạn ra ngoài, trải nghiệm những điều mới lạ và kết nối với cộng đồng.</li>
                            <li><strong>Biến khám phá thành niềm vui:</strong> Với tính năng sưu tầm thẻ, mỗi hành trình đều trở nên thú vị và mang lại cảm giác thành tựu.</li>
                        </ul>
                        <p>Hãy bắt đầu hành trình của bạn với <strong>Google Check-in</strong> ngay hôm nay và biến mỗi ngày thành một chuyến phiêu lưu!</p>
                    </div>
                    <div class="goals-image animate__animated animate__fadeInRight">
                        <img src="https://placehold.co/400x250/E9F8F8/42B983?text=Our+Goal" alt="Hình ảnh minh họa mục tiêu" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { onMounted } from 'vue';
import 'animate.css';

onMounted(() => {
    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate__active');
                observer.unobserve(entry.target);
            }
        });
    });

    document.querySelectorAll('.animate__animated').forEach(el => {
        observer.observe(el);
    });
});

function getStarStyle() {
    const size = Math.random() * 2 + 1; // Star size between 1px and 3px
    const top = Math.random() * -100; // Start above the screen
    const left = Math.random() * 100;
    const duration = Math.random() * 10 + 5; // Duration between 5s and 15s
    const delay = Math.random() * 5; // Delay up to 5s
    return {
        width: `${size}px`,
        height: `${size}px`,
        top: `${top}vh`,
        left: `${left}%`,
        animationDuration: `${duration}s`,
        animationDelay: `${delay}s`,
    };
}
</script>

<style scoped>
/* Khai báo biến CSS cho màu sắc và font chữ */
/* Bỏ đoạn ép màu đen toàn cục đi */

/* Hero Section (nền tối) */
.hero-section h1 {
    color: #ffffff !important; /* trắng */
}

.hero-section p {
    color: #e0e0e0 !important; /* xám sáng */
}

/* Features Section (nền sáng) */
.features-section h2,
.features-section h3,
.features-section p {
    color: #000000; /* đen */
}

/* Goals Section (nền sáng nhẹ) */
.goals-section h2,
.goals-section li,
.goals-section p {
    color: #111111; /* đen đậm */
}

/* Các thuộc tính chung */
* {
    box-sizing: border-box;
}

.introduce-page {
    font-family: var(--font-primary);
    padding: 0;
    line-height: 1.6;
    color: var(--color-text);
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 2rem;
    position: relative; /* Ensure content is above stars */
    z-index: 1;
}

h1, h2, h3 {
    text-align: center;
    color: var(--color-heading);
}

h1 {
    font-size: 2.5rem;
    margin-bottom: 1rem;
    color: var(--color-primary);
}

h2 {
    font-size: 2rem;
    margin-bottom: 2.5rem;
    position: relative;
    padding-bottom: 1rem;
    font-weight: 600;
}

h2::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 50%;
    transform: translateX(-50%);
    width: 80px;
    height: 4px;
    background-color: var(--color-primary);
    transition: width 0.3s ease-in-out;
}

h2:hover::after {
    width: 120px;
}

/* Phần giới thiệu chính (Hero Section) */
.hero-section {
    background-color: #1a2533; /* Dark blue background */
    padding: 4rem 0;
    text-align: center;
    position: relative;
    overflow: hidden;
}

.hero-section h1 {
    color: #fff; /* White text */
}

.hero-section p {
    font-size: 1.2rem;
    max-width: 800px;
    margin: 0 auto;
    color: #e0e0e0; /* Light grey text */
}

/* Star Animation */
.stars {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 0;
}

.star {
    position: absolute;
    background-color: white;
    border-radius: 50%;
    animation-name: fall;
    animation-timing-function: linear;
    animation-iteration-count: infinite;
}

@keyframes fall {
    from {
        transform: translateY(0vh);
    }
    to {
        transform: translateY(110vh);
    }
}


/* Phần các tính năng */
.features-section {
    padding: 4rem 0;
    background-color: #f9f9f9; /* Lighter background for contrast */
}

.features-grid {
    display: flex;
    justify-content: center;
    gap: 2rem;
    flex-wrap: wrap;
}

.feature-item {
    flex: 1 1 300px;
    text-align: center;
    padding: 2rem;
    border: 1px solid #e0e0e0; /* Lighter border */
    border-radius: 12px; /* Softer corners */
    transition: transform 0.4s ease, box-shadow 0.4s ease, border-color 0.4s ease, background-color 0.4s ease; /* Smoother transition */
    background-color: #ffffff; /* White background */
    opacity: 0;
    transform: translateY(20px);
}

.animate__active {
    opacity: 1;
    transform: translateY(0);
    transition: opacity 0.8s ease-out, transform 0.8s ease-out;
}

.feature-item:hover {
    transform: translateY(-15px); /* A bit more lift */
    box-shadow: 0 20px 40px rgba(44, 62, 80, 0.15); /* Stronger shadow using heading color */
    border-color: var(--color-primary); /* Highlight with primary color */
}

.feature-icon {
    width: 100px;
    height: 100px;
    margin-bottom: 1rem;
    transition: transform 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94); /* Bouncy transition */
}

.feature-item:hover .feature-icon {
    transform: scale(1.15) rotate(5deg); /* Scale and rotate */
}

/* Phần mục tiêu */
.goals-section {
    background-color: var(--color-secondary);
    padding: 4rem 0;
    position: relative; /* Added */
    overflow: hidden; /* Added */
}

.flowing-background {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 0;
    background: linear-gradient(
        -45deg,
        rgba(var(--color-primary-rgb), 0.1) 25%,
        transparent 25%,
        transparent 50%,
        rgba(var(--color-primary-rgb), 0.1) 50%,
        rgba(var(--color-primary-rgb), 0.1) 75%,
        transparent 75%,
        transparent
    );
    background-size: 60px 60px;
    animation: flowWaves 15s linear infinite;
}

@keyframes flowWaves {
    from {
        background-position: 0 0;
    }
    to {
        background-position: 60px 60px;
    }
}

.goals-content {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 3rem;
    flex-wrap: wrap;
}

.goals-text {
    flex: 1;
    max-width: 600px;
}

.goals-text ul {
    list-style: none;
    padding-left: 0;
}

.goals-text li {
    margin-bottom: 1rem;
    position: relative;
    padding-left: 25px;
    font-weight: 400;
}

.goals-text li::before {
    content: '✓';
    color: var(--color-primary);
    font-weight: bold;
    position: absolute;
    left: 0;
}

.goals-image img {
    max-width: 100%;
    height: auto;
    border-radius: 8px;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

/* Media Queries cho thiết bị nhỏ hơn */
@media (max-width: 768px) {
    .features-grid, .goals-content {
        flex-direction: column;
    }
}
</style>