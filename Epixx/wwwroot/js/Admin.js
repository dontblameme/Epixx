document.addEventListener("DOMContentLoaded", function () {
    const submitBtn = document.getElementById("submitBtn");
    const qtyInputs = document.querySelectorAll(".qty-input");

    function updateButtonState() {
        let isZero = true;

        for (const input of qtyInputs) {
            if (Number(input.value) !== 0) {
                isZero = false;
                break;
            }
        }

        submitBtn.disabled = isZero;
    }

    qtyInputs.forEach(input => {
        input.addEventListener("input", updateButtonState);
    });

    // Run once on load
    updateButtonState();
});