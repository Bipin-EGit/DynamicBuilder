<template>
  <div class="preview-view">
    <div class="preview-header">
      <h2>Form Preview & HTML Generation</h2>
      <div class="preview-actions">
        <button @click="activeTab = 'preview'" :class="{ active: activeTab === 'preview' }" class="tab-btn">
          Live Preview
        </button>
        <button @click="activeTab = 'html'" :class="{ active: activeTab === 'html' }" class="tab-btn">
          Generated HTML
        </button>
        <button @click="downloadHTML" class="btn btn-primary">
          Download HTML
        </button>
      </div>
    </div>

    <div class="preview-content">
      <!-- Live Preview Tab -->
      <div v-if="activeTab === 'preview'" class="preview-tab">
        <div class="live-preview">
          <div v-if="formStore.components.length === 0" class="empty-state">
            <h3>No components to preview</h3>
            <p>Go back to the form builder and add some components.</p>
            <router-link to="/" class="btn btn-primary">Back to Builder</router-link>
          </div>
          
          <div v-else class="form-preview">
            <h1 class="form-title">{{ formStore.formTitle }}</h1>
            
            <div 
              v-for="component in formStore.components"
              :key="component.id"
              class="preview-component"
            >
              <div v-if="component.type === 'text'" class="form-group">
                <label>{{ component.label }}</label>
                <input 
                  type="text" 
                  :placeholder="component.props.placeholder || ''"
                  class="form-control"
                />
              </div>
              
              <div v-else-if="component.type === 'textarea'" class="form-group">
                <label>{{ component.label }}</label>
                <textarea 
                  :placeholder="component.props.placeholder || ''"
                  :rows="component.props.rows || 4"
                  class="form-control"
                ></textarea>
              </div>
              
              <div v-else-if="component.type === 'date'" class="form-group">
                <label>{{ component.label }}</label>
                <input type="date" class="form-control" />
              </div>
              
              <div v-else-if="component.type === 'select'" class="form-group">
                <label>{{ component.label }}</label>
                <select class="form-control">
                  <option v-for="option in component.props.options" :key="option" :value="option">
                    {{ option }}
                  </option>
                </select>
              </div>
              
              <div v-else-if="component.type === 'checkbox'" class="form-group">
                <label class="checkbox-label">
                  <input type="checkbox" />
                  {{ component.label }}
                </label>
              </div>
              
              <div v-else-if="component.type === 'radio'" class="form-group">
                <label class="group-label">{{ component.label }}</label>
                <div class="radio-group">
                  <label 
                    v-for="option in component.props.options" 
                    :key="option"
                    class="radio-label"
                  >
                    <input 
                      type="radio" 
                      :name="component.id"
                      :value="option"
                    />
                    {{ option }}
                  </label>
                </div>
              </div>
              
              <div v-else-if="component.type === 'table'" class="form-group">
                <h3>{{ component.label }}</h3>
                <table class="preview-table">
                  <thead>
                    <tr>
                      <th>Column 1</th>
                      <th>Column 2</th>
                      <th>Column 3</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>Sample Data 1</td>
                      <td>Sample Data 2</td>
                      <td>Sample Data 3</td>
                    </tr>
                    <tr>
                      <td>Sample Data 4</td>
                      <td>Sample Data 5</td>
                      <td>Sample Data 6</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              
              <div v-else-if="component.type === 'chart'" class="form-group">
                <h3>{{ component.label }}</h3>
                <div class="chart-preview">
                  <canvas :ref="el => chartRefs[component.id] = el as HTMLCanvasElement"></canvas>
                </div>
              </div>
            </div>
            
            <div class="form-actions">
              <button type="submit" class="btn btn-primary">Submit Form</button>
              <button type="reset" class="btn btn-secondary">Reset</button>
            </div>
          </div>
        </div>
      </div>

      <!-- HTML Code Tab -->
      <div v-if="activeTab === 'html'" class="html-tab">
        <div class="html-header">
          <h3>Generated HTML Code</h3>
          <button @click="copyHTML" class="btn btn-secondary">
            {{ copyText }}
          </button>
        </div>
        <pre class="html-code"><code>{{ formattedHTML }}</code></pre>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'
import { useFormBuilderStore } from '../stores/formBuilder'
import { Chart, registerables } from 'chart.js'

Chart.register(...registerables)

const formStore = useFormBuilderStore()
const activeTab = ref('preview')
const copyText = ref('Copy HTML')
const chartRefs = ref<Record<string, HTMLCanvasElement>>({})
const chartInstances = ref<Record<string, Chart>>({})

const formattedHTML = computed(() => {
  const html = formStore.formHTML
  return html
    .replace(/></g, '>\n<')
    .replace(/\n\s*\n/g, '\n')
    .split('\n')
    .map(line => line.trim())
    .filter(line => line)
    .map((line, index, arr) => {
      const indent = getIndentLevel(line, index, arr)
      return '  '.repeat(indent) + line
    })
    .join('\n')
})

const getIndentLevel = (line: string, index: number, arr: string[]): number => {
  let level = 0
  
  for (let i = 0; i < index; i++) {
    const prevLine = arr[i]
    if (prevLine.includes('<div') || prevLine.includes('<table') || prevLine.includes('<thead') || prevLine.includes('<tbody')) {
      level++
    }
    if (prevLine.includes('</div>') || prevLine.includes('</table>') || prevLine.includes('</thead>') || prevLine.includes('</tbody>')) {
      level--
    }
  }
  
  if (line.includes('</div>') || line.includes('</table>') || line.includes('</thead>') || line.includes('</tbody>')) {
    level--
  }
  
  return Math.max(0, level)
}

const copyHTML = async () => {
  try {
    await navigator.clipboard.writeText(formStore.formHTML)
    copyText.value = 'Copied!'
    setTimeout(() => {
      copyText.value = 'Copy HTML'
    }, 2000)
  } catch (err) {
    console.error('Failed to copy HTML:', err)
  }
}

const downloadHTML = () => {
  const htmlContent = `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>${formStore.formTitle}</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            max-width: 800px;
            margin: 0 auto;
            padding: 2rem;
            background: #f5f5f5;
        }
        .dynamic-form {
            background: white;
            padding: 2rem;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .form-group {
            margin-bottom: 1.5rem;
        }
        label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: bold;
            color: #333;
        }
        input, textarea, select {
            width: 100%;
            padding: 0.75rem;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 1rem;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 0.5rem;
        }
        th, td {
            padding: 0.75rem;
            text-align: left;
            border: 1px solid #ddd;
        }
        th {
            background-color: #f8f9fa;
            font-weight: bold;
        }
        .chart-placeholder {
            background: #f0f0f0;
            border: 1px solid #ddd;
            border-radius: 4px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #666;
            font-weight: bold;
        }
    </style>
</head>
<body>
    ${formStore.formHTML}
</body>
</html>`

  const blob = new Blob([htmlContent], { type: 'text/html' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `${formStore.formTitle.replace(/\s+/g, '_').toLowerCase()}.html`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}

const createCharts = async () => {
  await nextTick()
  
  formStore.components.forEach(component => {
    if (component.type === 'chart' && chartRefs.value[component.id]) {
      const canvas = chartRefs.value[component.id]
      const chartType = component.props.chartType || 'bar'
      
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
      
      chartInstances.value[component.id] = new Chart(canvas, {
        type: chartType as any,
        data: sampleData[chartType as keyof typeof sampleData],
        options: {
          responsive: true,
          maintainAspectRatio: false
        }
      })
    }
  })
}

const destroyCharts = () => {
  Object.values(chartInstances.value).forEach(chart => {
    chart.destroy()
  })
  chartInstances.value = {}
}

onMounted(() => {
  createCharts()
})

onUnmounted(() => {
  destroyCharts()
})
</script>

<style scoped>
.preview-view {
  max-width: 1200px;
  margin: 0 auto;
}

.preview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding: 1rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.preview-header h2 {
  margin: 0;
  color: #2c3e50;
}

.preview-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.tab-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  background: #f8f9fa;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
}

.tab-btn.active {
  background: #3498db;
  color: white;
  border-color: #3498db;
}

.tab-btn:hover {
  background: #e9ecef;
}

.tab-btn.active:hover {
  background: #2980b9;
}

.preview-content {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  overflow: hidden;
}

.preview-tab, .html-tab {
  padding: 2rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  color: #6c757d;
}

.empty-state h3 {
  margin-bottom: 1rem;
}

.form-preview {
  max-width: 800px;
  margin: 0 auto;
}

.form-title {
  margin-bottom: 2rem;
  color: #2c3e50;
  text-align: center;
}

.preview-component {
  margin-bottom: 1.5rem;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label, .group-label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #495057;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 1rem;
}

.checkbox-label, .radio-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  font-weight: normal;
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-left: 1rem;
}

.preview-table {
  width: 100%;
  border-collapse: collapse;
  border: 1px solid #dee2e6;
}

.preview-table th,
.preview-table td {
  padding: 0.75rem;
  text-align: left;
  border: 1px solid #dee2e6;
}

.preview-table th {
  background-color: #f8f9fa;
  font-weight: 600;
}

.chart-preview {
  width: 100%;
  height: 300px;
  position: relative;
}

.form-actions {
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #dee2e6;
  display: flex;
  gap: 1rem;
}

.html-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.html-code {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 4px;
  padding: 1rem;
  overflow-x: auto;
  font-family: 'Courier New', monospace;
  font-size: 0.9rem;
  line-height: 1.4;
  white-space: pre;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  text-decoration: none;
  display: inline-block;
  text-align: center;
  font-size: 0.9rem;
  transition: all 0.3s;
}

.btn-primary {
  background: #3498db;
  color: white;
}

.btn-primary:hover {
  background: #2980b9;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}
</style>