<template>
  <div class="chart-component">
    <h3 class="component-title">{{ component.label }}</h3>
    <div class="chart-container">
      <canvas ref="chartCanvas"></canvas>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { Chart, registerables } from 'chart.js'
import type { FormComponent } from '../../stores/formBuilder'

Chart.register(...registerables)

const props = defineProps<{
  component: FormComponent
}>()

defineEmits<{
  remove: [id: string]
}>()

const chartCanvas = ref<HTMLCanvasElement>()
let chartInstance: Chart | null = null

const sampleData = {
  bar: {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May'],
    datasets: [{
      label: 'Sample Data',
      data: [12, 19, 3, 5, 2],
      backgroundColor: 'rgba(52, 152, 219, 0.6)',
      borderColor: 'rgba(52, 152, 219, 1)',
      borderWidth: 1
    }]
  },
  line: {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May'],
    datasets: [{
      label: 'Sample Data',
      data: [12, 19, 3, 5, 2],
      borderColor: 'rgba(52, 152, 219, 1)',
      backgroundColor: 'rgba(52, 152, 219, 0.1)',
      fill: true
    }]
  },
  pie: {
    labels: ['Red', 'Blue', 'Yellow', 'Green'],
    datasets: [{
      data: [12, 19, 3, 5],
      backgroundColor: [
        'rgba(231, 76, 60, 0.6)',
        'rgba(52, 152, 219, 0.6)',
        'rgba(241, 196, 15, 0.6)',
        'rgba(46, 204, 113, 0.6)'
      ]
    }]
  },
  doughnut: {
    labels: ['Red', 'Blue', 'Yellow', 'Green'],
    datasets: [{
      data: [12, 19, 3, 5],
      backgroundColor: [
        'rgba(231, 76, 60, 0.6)',
        'rgba(52, 152, 219, 0.6)',
        'rgba(241, 196, 15, 0.6)',
        'rgba(46, 204, 113, 0.6)'
      ]
    }]
  }
}

const createChart = () => {
  if (chartCanvas.value && !chartInstance) {
    const chartType = props.component.props.chartType || 'bar'
    const data = sampleData[chartType as keyof typeof sampleData]
    
    chartInstance = new Chart(chartCanvas.value, {
      type: chartType as any,
      data,
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: true,
            position: 'top'
          }
        }
      }
    })
  }
}

const destroyChart = () => {
  if (chartInstance) {
    chartInstance.destroy()
    chartInstance = null
  }
}

const updateChart = () => {
  destroyChart()
  createChart()
}

onMounted(() => {
  createChart()
})

onUnmounted(() => {
  destroyChart()
})

watch(() => props.component.props.chartType, () => {
  updateChart()
})
</script>

<style scoped>
.chart-component {
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background: white;
}

.component-title {
  margin: 0 0 1rem 0;
  color: #495057;
  font-size: 1.2rem;
}

.chart-container {
  width: 100%;
  height: 300px;
  position: relative;
}
</style>