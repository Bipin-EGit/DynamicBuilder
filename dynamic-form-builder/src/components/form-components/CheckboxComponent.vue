<template>
  <div class="checkbox-component">
    <label class="component-checkbox-label">
      <input type="checkbox" disabled />
      {{ component.label }}
    </label>
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
.checkbox-component {
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background: white;
}

.component-checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
  color: #495057;
  cursor: pointer;
}

.component-checkbox-label input[type="checkbox"] {
  margin: 0;
}
</style>