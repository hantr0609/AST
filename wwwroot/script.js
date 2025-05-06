document.addEventListener('DOMContentLoaded', function () {
  const convertButton = document.getElementById('convertButton');
  const mlExpressionInput = document.getElementById('mlExpression');

  if (convertButton) {
    convertButton.addEventListener('click', convertAndDisplay);
  }

  if (mlExpressionInput) {
    mlExpressionInput.addEventListener('keypress', function (event) {
      if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        convertAndDisplay();
      }
    });
  }
});

// Convert the micro-ML expression and display directly in the code textarea
async function convertAndDisplay() {
  const mlExpression = document.getElementById('mlExpression').value.trim();
  const codeTextarea = document.getElementById('code');
  const errorElement = document.getElementById('ml-parse-error');

  // Clear previous error message
  errorElement.textContent = '';

  if (!mlExpression) {
    errorElement.textContent = 'Please enter a micro-ML expression';
    return;
  }

  // Show processing indicator
  errorElement.textContent = 'Converting...';
  errorElement.style.color = '#333';

  try {
    const response = await fetch('/api/convert-ml', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ expression: mlExpression }),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.error || 'Failed to convert expression');
    }

    const result = await response.json();

    // Update the code textarea with the syntax tree notation
    codeTextarea.value = result.syntaxTree;

    // Trigger the update for the visualizer by dispatching an input event
    // This will automatically update the syntax tree visualization
    const inputEvent = new Event('input', { bubbles: true });
    codeTextarea.dispatchEvent(inputEvent);

    // Show success message
    errorElement.textContent = 'Conversion successful!';
    errorElement.style.color = '#008800';

    // Clear the success message after a delay
    setTimeout(() => {
      errorElement.textContent = '';
    }, 3000);
  } catch (error) {
    console.error('Error converting expression:', error);
    errorElement.textContent = 'Error: ' + error.message;
    errorElement.style.color = '#cc0000';
  }
}
