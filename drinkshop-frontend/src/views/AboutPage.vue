<template>
  <MainLayout>
    <div class="about-page">
      <!-- 公司介紹 -->
      <section class="company-intro">
        <div class="container">
          <div class="intro-content">
            <div class="intro-text">
              <h1>關於清涼飲品</h1>
              <p class="lead">
                自2020年創立以來，清涼飲品致力於為每一位顧客提供最優質的飲品體驗。
                我們堅持使用新鮮、天然的食材，結合創新的調製技術，為您帶來健康美味的飲品享受。
              </p>
            </div>
            <div class="intro-image">
              <img src="/images/about-hero.jpg" alt="清涼飲品團隊" />
            </div>
          </div>
        </div>
      </section>

      <!-- 我們的使命 -->
      <section class="mission-section">
        <div class="container">
          <h2 class="section-title">我們的使命</h2>
          <div class="mission-grid">
            <div class="mission-item">
              <div class="mission-icon">
                <i class="fas fa-leaf"></i>
              </div>
              <h3>天然健康</h3>
              <p>堅持使用無添加的天然食材，為顧客提供健康的飲品選擇。</p>
            </div>
            
            <div class="mission-item">
              <div class="mission-icon">
                <i class="fas fa-heart"></i>
              </div>
              <h3>用心服務</h3>
              <p>每一杯飲品都經過精心調製，用心對待每一位顧客的需求。</p>
            </div>
            
            <div class="mission-item">
              <div class="mission-icon">
                <i class="fas fa-lightbulb"></i>
              </div>
              <h3>創新研發</h3>
              <p>不斷創新飲品配方，為顧客帶來驚喜的味覺體驗。</p>
            </div>
          </div>
        </div>
      </section>

      <!-- 我們的優勢 -->
      <section class="advantages-section">
        <div class="container">
          <h2 class="section-title">為什麼選擇我們</h2>
          <div class="advantages-content">
            <div class="advantage-list">
              <div class="advantage-item">
                <i class="fas fa-check-circle"></i>
                <div>
                  <h4>嚴選食材</h4>
                  <p>所有原料均來自信譽良好的供應商，確保品質與安全。</p>
                </div>
              </div>
              
              <div class="advantage-item">
                <i class="fas fa-check-circle"></i>
                <div>
                  <h4>現做新鮮</h4>
                  <p>每杯飲品都是接單後現場製作，確保最佳口感。</p>
                </div>
              </div>
              
              <div class="advantage-item">
                <i class="fas fa-check-circle"></i>
                <div>
                  <h4>營養均衡</h4>
                  <p>專業營養師調配，兼顧美味與健康的完美平衡。</p>
                </div>
              </div>
              
              <div class="advantage-item">
                <i class="fas fa-check-circle"></i>
                <div>
                  <h4>快速服務</h4>
                  <p>高效率的製作流程，讓您在最短時間內享用美味飲品。</p>
                </div>
              </div>
            </div>
            
            <div class="advantage-image">
              <img src="/images/fresh-ingredients.jpg" alt="新鮮食材" />
            </div>
          </div>
        </div>
      </section>

      <!-- 團隊介紹 -->
      <section class="team-section">
        <div class="container">
          <h2 class="section-title">我們的團隊</h2>
          <div class="team-grid">
            <div class="team-member">
              <div class="member-photo">
                <img src="/images/team-1.jpg" alt="創辦人" />
              </div>
              <div class="member-info">
                <h3>張美玲</h3>
                <p class="role">創辦人 & CEO</p>
                <p class="bio">擁有15年餐飲業經驗，致力於推廣健康飲食文化。</p>
              </div>
            </div>
            
            <div class="team-member">
              <div class="member-photo">
                <img src="/images/team-2.jpg" alt="營養師" />
              </div>
              <div class="member-info">
                <h3>李志明</h3>
                <p class="role">首席營養師</p>
                <p class="bio">專業營養師背景，負責產品營養配比與健康把關。</p>
              </div>
            </div>
            
            <div class="team-member">
              <div class="member-photo">
                <img src="/images/team-3.jpg" alt="調飲師" />
              </div>
              <div class="member-info">
                <h3>王雅芬</h3>
                <p class="role">首席調飲師</p>
                <p class="bio">國際調飲大賽得主，為您調製最完美的每一杯飲品。</p>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  </MainLayout>
</template>

<script setup>
import MainLayout from '@/layouts/MainLayout.vue'
</script>

<style scoped>
.about-page {
  padding: 2rem 0;
}

.company-intro {
  margin-bottom: 4rem;
}

.intro-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
}

.intro-text h1 {
  font-size: 3rem;
  color: #1f2937;
  margin-bottom: 1.5rem;
  font-weight: 700;
}

.lead {
  font-size: 1.25rem;
  line-height: 1.7;
  color: #4b5563;
}

.intro-image img {
  width: 100%;
  height: auto;
  border-radius: 1rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}

.mission-section, .advantages-section, .team-section {
  padding: 4rem 0;
  background-color: #f9fafb;
  margin: 2rem 0;
}

.section-title {
  text-align: center;
  font-size: 2.5rem;
  color: #1f2937;
  margin-bottom: 3rem;
  font-weight: 600;
}

.mission-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.mission-item {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  text-align: center;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.mission-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.mission-icon {
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #10b981, #059669);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem auto;
}

.mission-icon i {
  font-size: 2rem;
  color: white;
}

.mission-item h3 {
  font-size: 1.5rem;
  color: #1f2937;
  margin-bottom: 1rem;
  font-weight: 600;
}

.mission-item p {
  color: #6b7280;
  line-height: 1.6;
}

.advantages-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
}

.advantage-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.advantage-item {
  display: flex;
  gap: 1rem;
  align-items: flex-start;
}

.advantage-item i {
  color: #10b981;
  font-size: 1.25rem;
  margin-top: 0.25rem;
  flex-shrink: 0;
}

.advantage-item h4 {
  font-size: 1.25rem;
  color: #1f2937;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.advantage-item p {
  color: #6b7280;
  line-height: 1.6;
}

.advantage-image img {
  width: 100%;
  height: auto;
  border-radius: 1rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}

.team-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.team-member {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.team-member:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.member-photo {
  height: 250px;
  overflow: hidden;
}

.member-photo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.member-info {
  padding: 1.5rem;
  text-align: center;
}

.member-info h3 {
  font-size: 1.25rem;
  color: #1f2937;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.role {
  color: #2563eb;
  font-weight: 500;
  margin-bottom: 1rem;
}

.bio {
  color: #6b7280;
  line-height: 1.6;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .intro-content, .advantages-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }

  .intro-text h1 {
    font-size: 2rem;
  }

  .section-title {
    font-size: 2rem;
  }

  .mission-grid, .team-grid {
    grid-template-columns: 1fr;
  }

  .mission-section, .advantages-section, .team-section {
    padding: 2rem 0;
  }
}
</style>
