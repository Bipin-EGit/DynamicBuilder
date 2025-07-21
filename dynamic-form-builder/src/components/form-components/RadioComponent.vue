<template>
  <div class="radio-component">
    <label class="component-label">{{ component.label }}</label>
    <div class="radio-options">
      <label 
        v-for="option in component.props.options" 
        :key="option"
        class="radio-option"
      >
        <input 
          type="radio" 
          :name="component.id"
          :value="option"
          disabled
        />
        {{ option }}
      </label>
    </div>
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
.radio-component {
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

.radio-options {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.radio-option input[type="radio"] {
  margin: 0;
}
</style>