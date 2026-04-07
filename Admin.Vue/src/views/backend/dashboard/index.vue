<template>
  <div class="dashboard-container">
    <h1 class="dashboard-title">仪表盘</h1>
    <div class="dashboard-stats">
      <el-card
        shadow="hover"
        class="stat-card"
      >
        <template #header>
          <div class="card-header">
            <span>总用户数</span>
          </div>
        </template>
        <div class="stat-value">{{ stats.totalUsers }}</div>
        <div class="stat-change positive">+5.2%</div>
      </el-card>
      <el-card
        shadow="hover"
        class="stat-card"
      >
        <template #header>
          <div class="card-header">
            <span>今日访问</span>
          </div>
        </template>
        <div class="stat-value">{{ stats.todayVisits }}</div>
        <div class="stat-change positive">+12.8%</div>
      </el-card>
      <el-card
        shadow="hover"
        class="stat-card"
      >
        <template #header>
          <div class="card-header">
            <span>系统状态</span>
          </div>
        </template>
        <div class="stat-value">{{ stats.systemStatus }}</div>
        <div class="stat-change neutral">正常</div>
      </el-card>
      <el-card
        shadow="hover"
        class="stat-card"
      >
        <template #header>
          <div class="card-header">
            <span>存储空间</span>
          </div>
        </template>
        <div class="stat-value">{{ stats.storageUsed }}</div>
        <div class="stat-change negative">+2.1%</div>
      </el-card>
    </div>
    <div class="dashboard-charts">
      <el-card
        shadow="hover"
        class="chart-card"
      >
        <template #header>
          <div class="card-header">
            <span>用户增长趋势</span>
          </div>
        </template>
        <div class="chart-placeholder">
          <el-empty description="图表区域" />
        </div>
      </el-card>
      <el-card
        shadow="hover"
        class="chart-card"
      >
        <template #header>
          <div class="card-header">
            <span>访问来源分析</span>
          </div>
        </template>
        <div class="chart-placeholder">
          <el-empty description="图表区域" />
        </div>
      </el-card>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { onMounted, ref } from 'vue'
  import { rolesApi, usersApi } from '@/api'

  // 统计数据
  const stats = ref({
    storageUsed: '0%',
    systemStatus: '正常',
    todayVisits: '0',
    totalUsers: '0',
  })

  const loadDashboardData = async () => {
    try {
      // 获取用户总数
      const usersRes = await usersApi.getList()
      if (usersRes.success && usersRes.data) {
        stats.value.totalUsers = usersRes.data.length.toString()
      }

      // 模拟今日访问数据
      stats.value.todayVisits = Math.floor(Math.random() * 1000).toString()

      // 模拟存储空间使用
      stats.value.storageUsed = Math.floor(Math.random() * 50 + 20) + '%'
    } catch (error) {
      console.error('加载仪表盘数据失败:', error)
    }
  }

  onMounted(() => {
    loadDashboardData()
  })
</script>

<style scoped lang="scss">
  .dashboard-container {
    padding: 1rem;
    background-color: var(--el-bg-color-page);
  }

  .dashboard-title {
    font-size: 1.5rem;
    margin-bottom: 1.5rem;
    color: var(--el-text-color-primary);
    font-weight: 600;
  }

  .dashboard-stats {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1rem;
    margin-bottom: 1.5rem;
  }

  .stat-card {
    transition: all 0.3s;
    background-color: var(--el-bg-color);

    &:hover {
      transform: translateY(-5px);
      box-shadow: var(--el-box-shadow-dark);
    }
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .stat-value {
    font-size: 2rem;
    font-weight: bold;
    margin: 1rem 0;
    color: var(--el-text-color-primary);
  }

  .stat-change {
    font-size: 0.9rem;
    font-weight: 500;

    &.positive {
      color: var(--el-color-success);
    }

    &.negative {
      color: var(--el-color-danger);
    }

    &.neutral {
      color: var(--el-text-color-secondary);
    }
  }

  .dashboard-charts {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
    gap: 1rem;
  }

  .chart-card {
    height: 300px;
    background-color: var(--el-bg-color);
  }

  .chart-placeholder {
    height: 250px;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  @media (max-width: 768px) {
    .dashboard-charts {
      grid-template-columns: 1fr;
    }

    .chart-card {
      height: 250px;
    }

    .chart-placeholder {
      height: 200px;
    }
  }
</style>
