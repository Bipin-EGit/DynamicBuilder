// Form Builder JavaScript
class FormBuilder {
    constructor() {
        this.selectedComponent = null;
        this.isDragging = false;
        this.init();
    }

    init() {
        this.setupDragAndDrop();
        this.setupEventListeners();
        this.setupCanvas();
    }

    setupDragAndDrop() {
        // Make palette items draggable
        const paletteItems = document.querySelectorAll('.palette-item');
        paletteItems.forEach(item => {
            item.addEventListener('dragstart', (e) => this.handleDragStart(e));
            item.addEventListener('dragend', (e) => this.handleDragEnd(e));
        });

        // Setup canvas as drop zone
        const canvas = document.getElementById('formCanvas');
        if (canvas) {
            canvas.addEventListener('dragover', (e) => this.handleDragOver(e));
            canvas.addEventListener('drop', (e) => this.handleDrop(e));
            canvas.addEventListener('click', (e) => this.handleCanvasClick(e));
        }
    }

    setupEventListeners() {
        // Form title updates
        const formTitle = document.getElementById('formTitle');
        if (formTitle) {
            formTitle.addEventListener('blur', () => this.updateFormTitle());
            formTitle.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') {
                    formTitle.blur();
                }
            });
        }

        // Clear form button
        const clearForm = document.getElementById('clearForm');
        if (clearForm) {
            clearForm.addEventListener('click', () => this.clearForm());
        }

        // Property inputs
        this.setupPropertyListeners();
    }

    setupPropertyListeners() {
        document.addEventListener('input', (e) => {
            if (e.target.matches('.property-label, .property-placeholder, .property-rows, .property-options, .property-charttype')) {
                this.updateComponentProperties();
            }
        });
    }

    setupCanvas() {
        // Setup existing components
        const components = document.querySelectorAll('.canvas-component');
        components.forEach(component => {
            component.addEventListener('click', (e) => this.selectComponent(e, component));
        });
    }

    handleDragStart(e) {
        this.isDragging = true;
        const componentType = e.target.getAttribute('data-component-type');
        e.dataTransfer.setData('componentType', componentType);
        e.target.classList.add('drag-source');
    }

    handleDragEnd(e) {
        this.isDragging = false;
        e.target.classList.remove('drag-source');
    }

    handleDragOver(e) {
        e.preventDefault();
        if (this.isDragging) {
            e.currentTarget.classList.add('drop-zone');
        }
    }

    handleDrop(e) {
        e.preventDefault();
        e.currentTarget.classList.remove('drop-zone');
        
        const componentType = e.dataTransfer.getData('componentType');
        if (!componentType) return;

        const rect = e.currentTarget.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        this.addComponent(parseInt(componentType), x, y);
    }

    handleCanvasClick(e) {
        if (e.target === e.currentTarget) {
            this.deselectAllComponents();
        }
    }

    async addComponent(type, x, y) {
        const formId = document.getElementById('formId').value;
        
        const defaultLabels = {
            1: 'Text Input',
            2: 'Textarea',
            3: 'Date Input',
            4: 'Select',
            5: 'Checkbox',
            6: 'Radio Group',
            7: 'Data Table',
            8: 'Chart'
        };

        const defaultProperties = {
            1: { placeholder: 'Enter text...' },
            2: { placeholder: 'Enter text...', rows: 4 },
            3: {},
            4: { options: ['Option 1', 'Option 2', 'Option 3'] },
            5: {},
            6: { options: ['Option 1', 'Option 2', 'Option 3'] },
            7: {},
            8: { chartType: 'bar' }
        };

        const componentData = {
            Type: type,
            Label: `New ${defaultLabels[type]}`,
            Properties: JSON.stringify(defaultProperties[type] || {}),
            PositionX: Math.round(x),
            PositionY: Math.round(y),
            FormId: parseInt(formId)
        };

        try {
            const response = await fetch('/FormBuilder/AddComponent', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(componentData)
            });

            const result = await response.json();
            if (result.success) {
                await this.refreshCanvas();
                this.hideCanvasPlaceholder();
            } else {
                this.showError('Failed to add component: ' + (result.message || 'Unknown error'));
            }
        } catch (error) {
            this.showError('Failed to add component: ' + error.message);
        }
    }

    selectComponent(e, componentElement) {
        e.stopPropagation();
        
        // Remove selection from all components
        this.deselectAllComponents();
        
        // Select current component
        componentElement.classList.add('selected');
        this.selectedComponent = componentElement;
        
        // Load properties
        this.loadComponentProperties(componentElement);
    }

    deselectAllComponents() {
        const components = document.querySelectorAll('.canvas-component');
        components.forEach(comp => comp.classList.remove('selected'));
        this.selectedComponent = null;
        this.showNoSelectionMessage();
    }

    loadComponentProperties(componentElement) {
        const componentId = componentElement.getAttribute('data-component-id');
        const componentType = parseInt(componentElement.getAttribute('data-component-type'));
        
        // Get component data from the element or fetch from server
        const label = componentElement.querySelector('.component-label')?.textContent || '';
        
        // Show appropriate property template
        this.showPropertyPanel(componentType, componentId, label);
    }

    showPropertyPanel(type, componentId, label) {
        const propertiesContent = document.getElementById('propertiesContent');
        const noSelectionMessage = document.getElementById('noSelectionMessage');
        
        if (noSelectionMessage) {
            noSelectionMessage.style.display = 'none';
        }

        let templateId;
        switch (type) {
            case 1: // Text
            case 2: // Textarea
                templateId = 'textPropertiesTemplate';
                break;
            case 4: // Select
            case 6: // Radio
                templateId = 'selectPropertiesTemplate';
                break;
            case 8: // Chart
                templateId = 'chartPropertiesTemplate';
                break;
            default:
                templateId = 'defaultPropertiesTemplate';
                break;
        }

        const template = document.getElementById(templateId);
        if (template && propertiesContent) {
            propertiesContent.innerHTML = template.innerHTML;
            
            // Set current values
            const labelInput = propertiesContent.querySelector('.property-label');
            if (labelInput) {
                labelInput.value = label;
                labelInput.setAttribute('data-component-id', componentId);
            }

            // Show/hide textarea-specific properties
            if (type === 2) {
                const textareaOnly = propertiesContent.querySelector('.textarea-only');
                if (textareaOnly) {
                    textareaOnly.style.display = 'block';
                }
            }

            // Load existing properties from component
            this.loadExistingProperties(componentId, propertiesContent);
        }
    }

    showNoSelectionMessage() {
        const propertiesContent = document.getElementById('propertiesContent');
        const noSelectionMessage = document.getElementById('noSelectionMessage');
        
        if (propertiesContent) {
            propertiesContent.innerHTML = '';
            if (noSelectionMessage) {
                propertiesContent.appendChild(noSelectionMessage.cloneNode(true));
                propertiesContent.querySelector('#noSelectionMessage').style.display = 'block';
            }
        }
    }

    async loadExistingProperties(componentId, propertiesPanel) {
        // This would typically fetch from server, but for now we'll use data attributes
        const componentElement = document.querySelector(`[data-component-id="${componentId}"]`);
        if (!componentElement) return;

        // Extract properties from the component element
        const placeholderInput = componentElement.querySelector('input[placeholder]');
        if (placeholderInput) {
            const placeholderProperty = propertiesPanel.querySelector('.property-placeholder');
            if (placeholderProperty) {
                placeholderProperty.value = placeholderInput.getAttribute('placeholder') || '';
            }
        }

        const textarea = componentElement.querySelector('textarea');
        if (textarea) {
            const rowsProperty = propertiesPanel.querySelector('.property-rows');
            if (rowsProperty) {
                rowsProperty.value = textarea.getAttribute('rows') || 4;
            }
        }
    }

    async updateComponentProperties() {
        if (!this.selectedComponent) return;

        const componentId = this.selectedComponent.getAttribute('data-component-id');
        const propertiesPanel = document.getElementById('propertiesContent');
        
        if (!propertiesPanel || !componentId) return;

        const label = propertiesPanel.querySelector('.property-label')?.value || '';
        const placeholder = propertiesPanel.querySelector('.property-placeholder')?.value || '';
        const rows = propertiesPanel.querySelector('.property-rows')?.value || 4;
        const options = propertiesPanel.querySelector('.property-options')?.value || '';
        const chartType = propertiesPanel.querySelector('.property-charttype')?.value || 'bar';

        const properties = {};
        if (placeholder) properties.placeholder = placeholder;
        if (rows) properties.rows = parseInt(rows);
        if (options) properties.options = options.split('\n').filter(opt => opt.trim());
        if (chartType) properties.chartType = chartType;

        const updateData = {
            Id: parseInt(componentId),
            Label: label,
            Properties: JSON.stringify(properties),
            PositionX: 0,
            PositionY: 0
        };

        try {
            const response = await fetch('/FormBuilder/UpdateComponent', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(updateData)
            });

            const result = await response.json();
            if (result.success) {
                // Update the component display
                await this.refreshCanvas();
                // Re-select the component
                setTimeout(() => {
                    const updatedComponent = document.querySelector(`[data-component-id="${componentId}"]`);
                    if (updatedComponent) {
                        this.selectComponent({stopPropagation: () => {}}, updatedComponent);
                    }
                }, 100);
            }
        } catch (error) {
            console.error('Failed to update component:', error);
        }
    }

    async refreshCanvas() {
        const formId = document.getElementById('formId').value;
        
        try {
            const response = await fetch(`/FormBuilder/GetFormComponents?formId=${formId}`);
            const components = await response.json();
            
            // This would typically re-render the canvas
            // For now, we'll just reload the page
            window.location.reload();
        } catch (error) {
            console.error('Failed to refresh canvas:', error);
        }
    }

    async updateFormTitle() {
        const formTitle = document.getElementById('formTitle');
        const formId = document.getElementById('formId').value;
        
        if (!formTitle || !formId) return;

        try {
            const response = await fetch('/FormBuilder/UpdateForm', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: `id=${formId}&title=${encodeURIComponent(formTitle.value)}&description=`
            });

            const result = await response.json();
            if (result.success) {
                formTitle.classList.add('success-state');
                setTimeout(() => formTitle.classList.remove('success-state'), 1000);
            }
        } catch (error) {
            formTitle.classList.add('error-state');
            setTimeout(() => formTitle.classList.remove('error-state'), 1000);
        }
    }

    async clearForm() {
        if (!confirm('Are you sure you want to clear all components from this form?')) {
            return;
        }

        const components = document.querySelectorAll('.canvas-component');
        for (const component of components) {
            const componentId = component.getAttribute('data-component-id');
            if (componentId) {
                await this.removeComponent(parseInt(componentId));
            }
        }

        this.showCanvasPlaceholder();
        this.deselectAllComponents();
    }

    hideCanvasPlaceholder() {
        const placeholder = document.getElementById('canvasPlaceholder');
        if (placeholder) {
            placeholder.style.display = 'none';
        }
    }

    showCanvasPlaceholder() {
        const placeholder = document.getElementById('canvasPlaceholder');
        const components = document.querySelectorAll('.canvas-component');
        if (placeholder && components.length === 0) {
            placeholder.style.display = 'block';
        }
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    showError(message) {
        // Simple error display - in production, use a proper notification system
        alert(message);
    }

    showSuccess(message) {
        // Simple success display - in production, use a proper notification system
        console.log(message);
    }
}

// Global function for removing components (called from property panel)
async function removeSelectedComponent() {
    if (window.formBuilder && window.formBuilder.selectedComponent) {
        const componentId = window.formBuilder.selectedComponent.getAttribute('data-component-id');
        if (componentId) {
            await window.formBuilder.removeComponent(parseInt(componentId));
        }
    }
}

// Add remove component method to FormBuilder
FormBuilder.prototype.removeComponent = async function(componentId) {
    try {
        const response = await fetch('/FormBuilder/DeleteComponent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': this.getAntiForgeryToken()
            },
            body: `id=${componentId}`
        });

        const result = await response.json();
        if (result.success) {
            const componentElement = document.querySelector(`[data-component-id="${componentId}"]`);
            if (componentElement) {
                componentElement.remove();
            }
            this.deselectAllComponents();
            this.showCanvasPlaceholder();
        } else {
            this.showError('Failed to remove component');
        }
    } catch (error) {
        this.showError('Failed to remove component: ' + error.message);
    }
};

// Initialize form builder when page loads
document.addEventListener('DOMContentLoaded', function() {
    if (document.querySelector('.form-builder-container')) {
        window.formBuilder = new FormBuilder();
    }
});