import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface FormComponent {
  id: string
  type: 'text' | 'date' | 'table' | 'chart' | 'textarea' | 'select' | 'checkbox' | 'radio'
  label: string
  props: Record<string, any>
  position: { x: number; y: number }
  size: { width: number; height: number }
}

export interface EmailSchedule {
  id: string
  email: string
  frequency: 'daily' | 'weekly' | 'monthly' | 'yearly'
  time: string
  enabled: boolean
  formId: string
}

export const useFormBuilderStore = defineStore('formBuilder', () => {
  // Form components state
  const components = ref<FormComponent[]>([])
  const selectedComponent = ref<FormComponent | null>(null)
  const formTitle = ref('Untitled Form')
  
  // Email schedules state
  const emailSchedules = ref<EmailSchedule[]>([])
  
  // Actions for form components
  const addComponent = (component: Omit<FormComponent, 'id'>) => {
    const newComponent: FormComponent = {
      ...component,
      id: `component_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
    }
    components.value.push(newComponent)
    return newComponent
  }
  
  const removeComponent = (id: string) => {
    const index = components.value.findIndex(c => c.id === id)
    if (index > -1) {
      components.value.splice(index, 1)
      if (selectedComponent.value?.id === id) {
        selectedComponent.value = null
      }
    }
  }
  
  const updateComponent = (id: string, updates: Partial<FormComponent>) => {
    const component = components.value.find(c => c.id === id)
    if (component) {
      Object.assign(component, updates)
    }
  }
  
  const selectComponent = (component: FormComponent | null) => {
    selectedComponent.value = component
  }
  
  const clearComponents = () => {
    components.value = []
    selectedComponent.value = null
  }
  
  // Actions for email schedules
  const addEmailSchedule = (schedule: Omit<EmailSchedule, 'id'>) => {
    const newSchedule: EmailSchedule = {
      ...schedule,
      id: `schedule_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
    }
    emailSchedules.value.push(newSchedule)
    return newSchedule
  }
  
  const removeEmailSchedule = (id: string) => {
    const index = emailSchedules.value.findIndex(s => s.id === id)
    if (index > -1) {
      emailSchedules.value.splice(index, 1)
    }
  }
  
  const updateEmailSchedule = (id: string, updates: Partial<EmailSchedule>) => {
    const schedule = emailSchedules.value.find(s => s.id === id)
    if (schedule) {
      Object.assign(schedule, updates)
    }
  }
  
  // Computed properties
  const formHTML = computed(() => {
    let html = `<div class="dynamic-form">\n<h1>${formTitle.value}</h1>\n`
    
    components.value.forEach(component => {
      switch (component.type) {
        case 'text':
          html += `<div class="form-group">\n  <label>${component.label}</label>\n  <input type="text" placeholder="${component.props.placeholder || ''}" />\n</div>\n`
          break
        case 'textarea':
          html += `<div class="form-group">\n  <label>${component.label}</label>\n  <textarea placeholder="${component.props.placeholder || ''}" rows="${component.props.rows || 4}"></textarea>\n</div>\n`
          break
        case 'date':
          html += `<div class="form-group">\n  <label>${component.label}</label>\n  <input type="date" />\n</div>\n`
          break
        case 'select':
          html += `<div class="form-group">\n  <label>${component.label}</label>\n  <select>\n`
          component.props.options?.forEach((option: string) => {
            html += `    <option value="${option}">${option}</option>\n`
          })
          html += `  </select>\n</div>\n`
          break
        case 'checkbox':
          html += `<div class="form-group">\n  <label><input type="checkbox" /> ${component.label}</label>\n</div>\n`
          break
        case 'radio':
          html += `<div class="form-group">\n  <label>${component.label}</label>\n`
          component.props.options?.forEach((option: string) => {
            html += `  <label><input type="radio" name="${component.id}" value="${option}" /> ${option}</label>\n`
          })
          html += `</div>\n`
          break
        case 'table':
          html += `<div class="form-group">\n  <h3>${component.label}</h3>\n  <table border="1">\n`
          html += `    <thead><tr><th>Column 1</th><th>Column 2</th><th>Column 3</th></tr></thead>\n`
          html += `    <tbody><tr><td>Data 1</td><td>Data 2</td><td>Data 3</td></tr></tbody>\n`
          html += `  </table>\n</div>\n`
          break
        case 'chart':
          html += `<div class="form-group">\n  <h3>${component.label}</h3>\n  <div class="chart-placeholder" style="width: 400px; height: 300px; background: #f0f0f0; display: flex; align-items: center; justify-content: center;">Chart: ${component.props.chartType || 'bar'}</div>\n</div>\n`
          break
      }
    })
    
    html += '</div>'
    return html
  })
  
  return {
    // State
    components,
    selectedComponent,
    formTitle,
    emailSchedules,
    
    // Actions
    addComponent,
    removeComponent,
    updateComponent,
    selectComponent,
    clearComponents,
    addEmailSchedule,
    removeEmailSchedule,
    updateEmailSchedule,
    
    // Computed
    formHTML
  }
})