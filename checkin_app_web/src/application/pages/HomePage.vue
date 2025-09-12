<template>
  <div class="home-page">
    <div class="container">
      <!-- Sidebar trái: Thông báo + Tin tức -->
      <aside class="sidebar">
        <!-- Thông báo -->
        <section class="card">
          <h3>📢 Thông báo mới</h3>
          <ul>
            <li v-for="(item, index) in announcements" :key="index">
              <strong>{{ item.title }}</strong>
              <p class="date">{{ item.date }}</p>
              <p>{{ item.content }}</p>
            </li>
          </ul>
        </section>

        <!-- Tin tức -->
        <section class="card">
          <h3>📰 Tin tức</h3>
          <div v-for="(news, index) in newsList" :key="index" class="news-item">
            <h4>{{ news.title }}</h4>
            <p class="date">{{ news.date }}</p>
            <p>{{ news.content }}</p>
          </div>
        </section>
      </aside>

      <!-- Main feed -->
      <main class="feed">
        <!-- Stories -->
        <section class="stories card">
          <div
            v-for="(story, index) in storiesList"
            :key="index"
            class="story-card"
          >
            <div class="story-img">
              <img :src="story.image" alt="story" />
              <div v-if="story.type === 'create'" class="create-overlay">
                <span class="plus">+</span>
              </div>
            </div>
            <p class="story-user">{{ story.user }}</p>
          </div>
        </section>

        <!-- Form đăng bài -->
        <div class="card create-post">
          <div class="create-header">
            <img class="avatar" src="@/assets/logo.png" alt="user" />
            <textarea v-model="newPost" placeholder="Bạn đang nghĩ gì vậy?"></textarea>
          </div>

          <div class="upload-options">
            <label class="upload-photo">
              🖼️ Đăng ảnh
              <input type="file" accept="image/*" @change="handleImageUpload" hidden />
            </label>
            <label class="upload-video">
              🎥 Đăng video
              <input type="file" accept="video/mp4,video/webm" @change="handleVideoUpload" hidden />
            </label>
            <button @click="addPost">Đăng</button>
          </div>
        </div>

        <!-- Danh sách bài viết -->
        <div v-for="(post, index) in posts" :key="index" class="card post">
          <div class="post-header">
            <img class="avatar" src="@/assets/logo.png" alt="user" />
            <div>
              <p class="author">{{ post.author }}</p>
              <p class="time">{{ post.time }}</p>
            </div>
          </div>

          <!-- Nếu là text -->
          <div v-if="post.type === 'text'" class="post-content">
            <p>{{ post.content }}</p>
          </div>

          <!-- Nếu là ảnh -->
          <div v-if="post.type === 'image'" class="post-image">
            <img :src="post.imageUrl" alt="post image" />
          </div>

          <!-- Nếu là video -->
          <div v-if="post.type === 'video'" class="post-video">
            <video controls :src="post.videoUrl"></video>
          </div>

          <!-- Actions -->
          <div class="post-actions">
            <button @click="toggleLike(index)">👍 {{ post.likes }} Thích</button>
            <button @click="focusComment(index)">💬 Bình luận</button>
            <button>↪️ Chia sẻ</button>
          </div>

          <!-- Comments -->
          <div class="comments">
            <div v-for="(cmt, cIndex) in post.comments" :key="cIndex" class="comment">
              <strong>{{ cmt.user }}</strong>: {{ cmt.text }}
            </div>
            <div class="comment-form">
              <input
                v-model="commentInputs[index]"
                placeholder="Viết bình luận..."
                @keyup.enter="addComment(index)"
              />
              <button @click="addComment(index)">Gửi</button>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue"

// Thông báo
const announcements = ref([
  { title: "Ra mắt tính năng mới", date: "12/09/2025", content: "Check-in nhiều địa điểm cùng lúc để nhận thêm điểm thưởng." },
  { title: "Sự kiện tháng 9", date: "10/09/2025", content: "Tham gia sự kiện check-in để nhận quà đặc biệt." }
])

// Tin tức
const newsList = ref([
  { title: "Check-in Landmark 81", date: "11/09/2025", content: "Người dùng có thể check-in Landmark 81 và nhận 200 điểm." },
  { title: "Thêm 50 địa điểm mới", date: "09/09/2025", content: "Chúng tôi vừa thêm 50 địa điểm hot tại HCM và Hà Nội." }
])

// Stories
const storiesList = ref([
  { user: "Tạo tin", type: "create", image: "/assets/story-create.jpg" },
  { user: "Minh", image: "/assets/story1.jpg" },
  { user: "Vinh", image: "/assets/story2.jpg" },
  { user: "Hà", image: "/assets/story3.jpg" },
  { user: "Thủy", image: "/assets/story4.jpg" }
])

// Bài viết cộng đồng
const posts = ref([
  {
    type: "text",
    author: "Nguyễn Văn A",
    time: "5 phút trước",
    content: "Mình vừa check-in ở Bưu điện Thành phố, đẹp quá!",
    likes: 2,
    comments: [
      { user: "Lê Thị B", text: "Chuẩn luôn, chỗ đó sống ảo đẹp lắm." },
      { user: "Admin", text: "Cảm ơn bạn đã chia sẻ!" }
    ]
  }
])

const newPost = ref("")
const newVideo = ref(null)
const newImage = ref(null)
const commentInputs = ref([])

// Đăng bài
const addPost = () => {
  if (newVideo.value) {
    posts.value.unshift({
      type: "video",
      author: "Bạn",
      time: "Vừa xong",
      videoUrl: newVideo.value,
      likes: 0,
      comments: []
    })
    newVideo.value = null
  } else if (newImage.value) {
    posts.value.unshift({
      type: "image",
      author: "Bạn",
      time: "Vừa xong",
      imageUrl: newImage.value,
      likes: 0,
      comments: []
    })
    newImage.value = null
  } else if (newPost.value.trim()) {
    posts.value.unshift({
      type: "text",
      author: "Bạn",
      time: "Vừa xong",
      content: newPost.value,
      likes: 0,
      comments: []
    })
    newPost.value = ""
  }
}

// Upload video
const handleVideoUpload = (e) => {
  const file = e.target.files[0]
  if (file) {
    const url = URL.createObjectURL(file)
    newVideo.value = url
    newPost.value = ""
    newImage.value = null
  }
}

// Upload ảnh
const handleImageUpload = (e) => {
  const file = e.target.files[0]
  if (file) {
    const url = URL.createObjectURL(file)
    newImage.value = url
    newPost.value = ""
    newVideo.value = null
  }
}

// Comment
const addComment = (postIndex) => {
  const text = commentInputs.value[postIndex]
  if (text && text.trim()) {
    posts.value[postIndex].comments.push({ user: "Bạn", text })
    commentInputs.value[postIndex] = ""
  }
}

// Like
const toggleLike = (index) => {
  posts.value[index].likes++
}

// Focus comment
const focusComment = (index) => {
  document.querySelectorAll(".comment-form input")[index]?.focus()
}
</script>

<style scoped>
.home-page {
  background: #f0f2f5;
  min-height: 100vh;
  padding: 1rem;
}
.container {
  display: flex;
  max-width: 1200px;
  margin: auto;
  gap: 1.5rem;
}

/* Sidebar */
.sidebar {
  flex: 1;
}
.sidebar .card {
  margin-bottom: 1rem;
}
.sidebar h3 {
  margin-bottom: 0.5rem;
  color: #00a86b;
  font-weight: bold;
}
.news-item h4 {
  color: #222;
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}
.news-item .date {
  color: #666;
  font-size: 0.85rem;
}
.news-item p {
  color: #333;
  font-size: 0.95rem;
}
.sidebar ul li strong {
  color: #222;
  font-weight: 600;
}
.sidebar ul li .date {
  color: #666;
  font-size: 0.85rem;
}
.sidebar ul li p {
  color: #333;
  font-size: 0.9rem;
}

/* Feed */
.feed {
  flex: 2;
}
.card {
  background: #fff;
  border-radius: 10px;
  box-shadow: 0 2px 5px rgba(0,0,0,0.05);
  padding: 1rem;
  margin-bottom: 1rem;
}
.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  margin-right: 0.75rem;
}

/* Stories */
.stories {
  display: flex;
  gap: 0.75rem;
  overflow-x: auto;
  padding: 0.5rem 0;
}
.story-card {
  min-width: 110px;
  height: 180px;
  border-radius: 10px;
  background: #ddd;
  position: relative;
  flex-shrink: 0;
  cursor: pointer;
}
.story-img {
  width: 100%;
  height: 140px;
  border-radius: 10px 10px 0 0;
  overflow: hidden;
  position: relative;
}
.story-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.create-overlay {
  position: absolute;
  bottom: -15px;
  left: 50%;
  transform: translateX(-50%);
  background: #00c46a;
  border-radius: 50%;
  width: 35px;
  height: 35px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 20px;
  font-weight: bold;
  border: 3px solid white;
}
.story-user {
  text-align: center;
  font-size: 0.9rem;
  margin-top: 5px;
  font-weight: 500;
  color: #222;
}

/* Create post */
.create-post {
  display: flex;
  flex-direction: column;
}
.create-header {
  display: flex;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}
.create-post textarea {
  flex: 1;
  border-radius: 20px;
  border: 1px solid #ddd;
  padding: 0.5rem 1rem;
  resize: none;
  min-height: 60px;
  color: #222;
}
.upload-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.upload-photo,
.upload-video {
  cursor: pointer;
  color: #555;
  font-size: 0.9rem;
}
.upload-photo:hover,
.upload-video:hover {
  color: #00a86b;
}
.create-post button {
  background: #00c46a;
  border: none;
  padding: 0.4rem 1.2rem;
  color: #fff;
  border-radius: 20px;
  cursor: pointer;
  font-weight: bold;
}
.create-post button:hover {
  background: #00a86b;
}

/* Post */
.post-header {
  display: flex;
  align-items: center;
  margin-bottom: 0.75rem;
}
.post-header .author {
  font-weight: 600;
  color: #222;
}
.post-header .time {
  font-size: 0.85rem;
  color: #666;
}
.post-content {
  margin-bottom: 0.75rem;
}
.post-content p {
  color: #333;
  font-size: 1rem;
  line-height: 1.4;
}
.post-image {
  margin-bottom: 0.75rem;
}
.post-image img {
  width: 100%;
  border-radius: 8px;
  object-fit: cover;
  max-height: 500px;
}
.post-video {
  margin-bottom: 0.75rem;
}
.post-video video {
  width: 100%;
  border-radius: 8px;
  max-height: 400px;
  object-fit: cover;
}
.post-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 0.75rem;
}
.post-actions button {
  border: none;
  background: transparent;
  cursor: pointer;
  color: #444;
  font-weight: 500;
}
.post-actions button:hover {
  color: #00a86b;
}
.comments {
  border-top: 1px solid #eee;
  padding-top: 0.5rem;
}
.comment {
  margin-bottom: 0.5rem;
  font-size: 0.95rem;
  color: #333;
}
.comment strong {
  color: #222;
}
.comment-form {
  display: flex;
  gap: 0.5rem;
}
.comment-form input {
  flex: 1;
  border: 1px solid #ddd;
  border-radius: 20px;
  padding: 0.4rem 0.8rem;
  color: #222;
}
.comment-form button {
  background: #00c46a;
  border: none;
  border-radius: 20px;
  padding: 0.4rem 0.8rem;
  color: #fff;
  cursor: pointer;
}
.comment-form button:hover {
  background: #00a86b;
}
</style>
