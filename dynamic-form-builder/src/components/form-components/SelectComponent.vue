<template>
  <div class="select-component">
    <label class="component-label">{{ component.label }}</label>
    <select class="component-select" disabled>
      <option v-for="option in component.props.options" :key="option" :value="option">
        {{ option }}
      </option>
    </select>
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
.select-component {
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

.component-select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 1rem;
  background: white;
}
</style>