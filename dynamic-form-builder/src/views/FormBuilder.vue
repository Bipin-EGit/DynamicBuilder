<template>
  <div class="form-builder">
    <div class="builder-header">
      <input 
        v-model="formStore.formTitle" 
        class="form-title-input"
        placeholder="Enter form title..."
      />
      <div class="header-actions">
        <button @click="clearForm" class="btn btn-secondary">Clear Form</button>
        <router-link to="/preview" class="btn btn-primary">Generate & Preview</router-link>
      </div>
    </div>

    <div class="builder-content">
      <!-- Component Palette -->
      <div class="component-palette">
        <h3>Components</h3>
        <div class="palette-items">
          <div 
            v-for="componentType in componentTypes" 
            :key="componentType.type"
            class="palette-item"
            draggable="true"
            @dragstart="handleDragStart($event, componentType)"
          >
            <i :class="componentType.icon"></i>
            <span>{{ componentType.label }}</span>
          </div>
        </div>
      </div>

      <!-- Canvas -->
      <div class="canvas-container">
        <div 
          class="canvas"
          @drop="handleDrop"
          @dragover="handleDragOver"
          @click="selectComponent(null)"
        >
          <div v-if="formStore.components.length === 0" class="canvas-placeholder">
            Drag components here to build your form
          </div>
          
          <div 
            v-for="component in formStore.components"
            :key="component.id"
            class="canvas-component"
            :class="{ 'selected': formStore.selectedComponent?.id === component.id }"
            @click.stop="selectComponent(component)"
          >
            <component 
              :is="getComponentRenderer(component.type)"
              :component="component"
              @remove="removeComponent"
            />
          </div>
        </div>
      </div>

      <!-- Properties Panel -->
      <div class="properties-panel">
        <h3>Properties</h3>
        <div v-if="formStore.selectedComponent" class="property-editor">
          <div class="property-group">
            <label>Label:</label>
            <input 
              v-model="formStore.selectedComponent.label"
              @input="updateSelectedComponent"
              class="property-input"
            />
          </div>
          
          <div v-if="formStore.selectedComponent.type === 'text' || formStore.selectedComponent.type === 'textarea'">
            <div class="property-group">
              <label>Placeholder:</label>
              <input 
                v-model="formStore.selectedComponent.props.placeholder"
                @input="updateSelectedComponent"
                class="property-input"
              />
            </div>
          </div>

          <div v-if="formStore.selectedComponent.type === 'textarea'">
            <div class="property-group">
              <label>Rows:</label>
              <input 
                type="number"
                v-model="formStore.selectedComponent.props.rows"
                @input="updateSelectedComponent"
                class="property-input"
                min="2"
                max="10"
              />
            </div>
          </div>

          <div v-if="formStore.selectedComponent.type === 'select' || formStore.selectedComponent.type === 'radio'">
            <div class="property-group">
              <label>Options (one per line):</label>
              <textarea 
                :value="getOptionsText(formStore.selectedComponent.props.options)"
                @input="updateOptions"
                class="property-textarea"
                rows="4"
              ></textarea>
            </div>
          </div>

          <div v-if="formStore.selectedComponent.type === 'chart'">
            <div class="property-group">
              <label>Chart Type:</label>
              <select 
                v-model="formStore.selectedComponent.props.chartType"
                @change="updateSelectedComponent"
                class="property-input"
              >
                <option value="bar">Bar Chart</option>
                <option value="line">Line Chart</option>
                <option value="pie">Pie Chart</option>
                <option value="doughnut">Doughnut Chart</option>
              </select>
            </div>
          </div>

          <button @click="removeComponent(formStore.selectedComponent.id)" class="btn btn-danger">
            Remove Component
          </button>
        </div>
        <div v-else class="no-selection">
          Select a component to edit its properties
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, markRaw } from 'vue'
import { useFormBuilderStore, type FormComponent } from '../stores/formBuilder'
import TextComponent from '../components/form-components/TextComponent.vue'
import TextareaComponent from '../components/form-components/TextareaComponent.vue'
import DateComponent from '../components/form-components/DateComponent.vue'
import SelectComponent from '../components/form-components/SelectComponent.vue'
import CheckboxComponent from '../components/form-components/CheckboxComponent.vue'
import RadioComponent from '../components/form-components/RadioComponent.vue'
import TableComponent from '../components/form-components/TableComponent.vue'
import ChartComponent from '../components/form-components/ChartComponent.vue'

const formStore = useFormBuilderStore()

const componentTypes = [
  { type: 'text', label: 'Text Input', icon: 'fas fa-font' },
  { type: 'textarea', label: 'Textarea', icon: 'fas fa-align-left' },
  { type: 'date', label: 'Date Input', icon: 'fas fa-calendar' },
  { type: 'select', label: 'Select', icon: 'fas fa-chevron-down' },
  { type: 'checkbox', label: 'Checkbox', icon: 'fas fa-check-square' },
  { type: 'radio', label: 'Radio', icon: 'fas fa-dot-circle' },
  { type: 'table', label: 'Table', icon: 'fas fa-table' },
  { type: 'chart', label: 'Chart', icon: 'fas fa-chart-bar' },
]

const componentRenderers = {
  text: markRaw(TextComponent),
  textarea: markRaw(TextareaComponent),
  date: markRaw(DateComponent),
  select: markRaw(SelectComponent),
  checkbox: markRaw(CheckboxComponent),
  radio: markRaw(RadioComponent),
  table: markRaw(TableComponent),
  chart: markRaw(ChartComponent),
}

const getComponentRenderer = (type: string) => {
  return componentRenderers[type as keyof typeof componentRenderers]
}

const handleDragStart = (event: DragEvent, componentType: any) => {
  event.dataTransfer?.setData('component-type', componentType.type)
}

const handleDragOver = (event: DragEvent) => {
  event.preventDefault()
}

const handleDrop = (event: DragEvent) => {
  event.preventDefault()
  const componentType = event.dataTransfer?.getData('component-type')
  
  if (componentType) {
    const rect = (event.target as HTMLElement).getBoundingClientRect()
    const x = event.clientX - rect.left
    const y = event.clientY - rect.top
    
    const defaultProps: Record<string, any> = {
      text: { placeholder: 'Enter text...' },
      textarea: { placeholder: 'Enter text...', rows: 4 },
      date: {},
      select: { options: ['Option 1', 'Option 2', 'Option 3'] },
      checkbox: {},
      radio: { options: ['Option 1', 'Option 2', 'Option 3'] },
      table: {},
      chart: { chartType: 'bar' },
    }
    
    formStore.addComponent({
      type: componentType as any,
      label: `New ${componentType.charAt(0).toUpperCase() + componentType.slice(1)}`,
      props: defaultProps[componentType] || {},
      position: { x, y },
      size: { width: 200, height: 40 }
    })
  }
}

const selectComponent = (component: FormComponent | null) => {
  formStore.selectComponent(component)
}

const removeComponent = (id: string) => {
  formStore.removeComponent(id)
}

const updateSelectedComponent = () => {
  if (formStore.selectedComponent) {
    formStore.updateComponent(formStore.selectedComponent.id, {
      label: formStore.selectedComponent.label,
      props: formStore.selectedComponent.props
    })
  }
}

const getOptionsText = (options: string[] = []) => {
  return options.join('\n')
}

const updateOptions = (event: Event) => {
  const target = event.target as HTMLTextAreaElement
  const options = target.value.split('\n').filter(opt => opt.trim())
  
  if (formStore.selectedComponent) {
    formStore.selectedComponent.props.options = options
    updateSelectedComponent()
  }
}

const clearForm = () => {
  if (confirm('Are you sure you want to clear the entire form?')) {
    formStore.clearComponents()
  }
}
</script>

<style scoped>
.form-builder {
  height: calc(100vh - 120px);
  display: flex;
  flex-direction: column;
}

.builder-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  margin-bottom: 1rem;
}

.form-title-input {
  font-size: 1.5rem;
  font-weight: bold;
  border: none;
  outline: none;
  padding: 0.5rem;
  border-bottom: 2px solid transparent;
  transition: border-color 0.3s;
}

.form-title-input:focus {
  border-bottom-color: #3498db;
}

.header-actions {
  display: flex;
  gap: 1rem;
}

.builder-content {
  display: flex;
  flex: 1;
  gap: 1rem;
  min-height: 0;
}

.component-palette {
  width: 250px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 1rem;
}

.component-palette h3 {
  margin: 0 0 1rem 0;
  color: #2c3e50;
}

.palette-items {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.palette-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: grab;
  transition: all 0.3s;
  background: #f8f9fa;
}

.palette-item:hover {
  background: #e9ecef;
  border-color: #3498db;
}

.palette-item:active {
  cursor: grabbing;
}

.canvas-container {
  flex: 1;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  overflow: hidden;
}

.canvas {
  height: 100%;
  position: relative;
  background: linear-gradient(45deg, #f8f9fa 25%, transparent 25%),
              linear-gradient(-45deg, #f8f9fa 25%, transparent 25%),
              linear-gradient(45deg, transparent 75%, #f8f9fa 75%),
              linear-gradient(-45deg, transparent 75%, #f8f9fa 75%);
  background-size: 20px 20px;
  background-position: 0 0, 0 10px, 10px -10px, -10px 0px;
  overflow: auto;
}

.canvas-placeholder {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: #6c757d;
  font-size: 1.2rem;
  text-align: center;
  pointer-events: none;
}

.canvas-component {
  position: relative;
  margin: 1rem;
  cursor: pointer;
  transition: all 0.3s;
}

.canvas-component:hover {
  transform: scale(1.02);
}

.canvas-component.selected {
  outline: 2px solid #3498db;
  outline-offset: 2px;
}

.properties-panel {
  width: 300px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 1rem;
}

.properties-panel h3 {
  margin: 0 0 1rem 0;
  color: #2c3e50;
}

.property-editor {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.property-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.property-group label {
  font-weight: 500;
  color: #495057;
}

.property-input, .property-textarea {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.9rem;
}

.property-input:focus, .property-textarea:focus {
  outline: none;
  border-color: #3498db;
}

.no-selection {
  color: #6c757d;
  text-align: center;
  padding: 2rem;
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

.btn-danger {
  background: #e74c3c;
  color: white;
}

.btn-danger:hover {
  background: #c0392b;
}
</style>