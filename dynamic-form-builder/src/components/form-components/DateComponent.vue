<template>
  <div class="date-component">
    <label class="component-label">{{ component.label }}</label>
    <input 
      type="date" 
      class="component-input"
      readonly
    />
  </div>
</template>

<script setup lang="ts">
import type { FormComponent } from '../../stores/formBuilder'

defineProps<{
  component: FormComponent
}>()

defineEmits<{
  remove: [id: string]
}>()
</script>

<style scoped>
.date-component {
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background: white;
}

.component-label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #495057;
}

.component-input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 1rem;
}
</style>