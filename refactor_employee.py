import re
import os

file_path = r'C:\Users\Admin\Documents\HRManagement\HRManagement\Controllers\EmployeeController.cs'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'var result = await _employeeService.SearchAsync(search);',
    'var resultResult = await _employeeService.SearchAsync(search);\n        var result = resultResult.Data;'
)
content = content.replace(
    'await _deptService.GetAllAsync(),',
    '(await _deptService.GetAllAsync()).Data,'
)
content = content.replace(
    'await _posService.GetAllAsync(),',
    '(await _posService.GetAllAsync()).Data,'
)
content = content.replace(
    'var employee = await _employeeService.GetByIdWithDetailsAsync(id);',
    'var employeeResult = await _employeeService.GetByIdWithDetailsAsync(id);\n        var employee = employeeResult.Data;'
)
# We haven't refactored TurnoverPredictionService yet, so let's check its return type later. Let's just leave it if it's not a generic service.

content = content.replace(
    'var emp = await _employeeService.GetByIdAsync(id);',
    'var empResult = await _employeeService.GetByIdAsync(id);\n        var emp = empResult.Data;'
)
content = content.replace(
    'var result = await _employeeService.CreateAsync(dto);\n        if (!result)',
    'var result = await _employeeService.CreateAsync(dto);\n        if (!result.Success)'
)
content = content.replace(
    'var result = await _employeeService.UpdateAsync(id, dto);\n        if (!result)',
    'var result = await _employeeService.UpdateAsync(id, dto);\n        if (!result.Success)'
)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print("Updated EmployeeController.cs")
