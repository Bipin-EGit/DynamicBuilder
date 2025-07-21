<template>
  <div class="table-component">
    <h3 class="component-title">{{ component.label }}</h3>
    <table class="component-table">
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
.table-component {
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

.component-table {
  width: 100%;
  border-collapse: collapse;
  border: 1px solid #dee2e6;
}

.component-table th,
.component-table td {
  padding: 0.75rem;
  text-align: left;
  border: 1px solid #dee2e6;
}

.component-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #495057;
}

.component-table tbody tr:nth-child(even) {
  background-color: #f8f9fa;
}
</style>