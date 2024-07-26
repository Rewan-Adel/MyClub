document.addEventListener('DOMContentLoaded', function () {
    const digitInputs = document.querySelectorAll('.digit-input');
    const form = document.querySelector('form');

    // move to the next input field
    function moveToNextInput(currentInput) {
        const nextInput = currentInput.nextElementSibling;
        if (currentInput.value.length === 1 && nextInput && nextInput.classList.contains('digit-input')) {
            nextInput.focus();
        }
    }

    // move to the previous input field
    function moveToPreviousInput(currentInput) {
        const previousInput = currentInput.previousElementSibling;
        if (currentInput.value === '' && previousInput && previousInput.classList.contains('digit-input')) {
            previousInput.focus();
        }
    }

    // Add event listeners to digit inputs
    digitInputs.forEach(function (input) {
        input.addEventListener('input', function (event) {
            moveToNextInput(event.target);
        });

        input.addEventListener('keydown', function (event) {
            if (event.key === 'Backspace') {
                moveToPreviousInput(event.target);
            }
        });
    });

    function getCodeFromInputs() {
        return Array.from(digitInputs).map(input => input.value).join('');
    }

    form.addEventListener('submit', function (event) {
        const codeInput = document.createElement('input');
        codeInput.setAttribute('type', 'hidden');
        codeInput.setAttribute('name', 'code');
        codeInput.setAttribute('value', getCodeFromInputs());
        form.appendChild(codeInput);

        digitInputs.forEach(input => input.remove());

    });
});
