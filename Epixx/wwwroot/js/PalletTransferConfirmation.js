document.addEventListener("DOMContentLoaded", function () {
    const barcodeInput = document.getElementById("barcode-input-1");
    const destinationInput = document.getElementById("destination-input-1");
    if (barcodeInput) {
        init();
    }
    function init() {
        barcodeInput.addEventListener("input", () => {
            const barcodes = document.querySelectorAll(".list-item-barcode-3");
            for (let barcode of barcodes) {
                if (barcode.textContent === barcodeInput.value) {
                    barcodeInput.disabled = true;
                    const oldSelected = document.querySelector(".selected");
                    oldSelected.classList.remove("selected");
                    let tr = barcode.closest("tr");
                    tr.classList.add("selected");
                    destinationInput.disabled = false;
                    destinationInput.placeholder = tr.querySelector(".list-item-destination-3").textContent;
                    destinationInput.focus();

                }
            }
        });
        destinationInput.addEventListener("input", async () => {
            const selectedRow = document.querySelector(".selected");
            const correctDestination = selectedRow.querySelector(".list-item-destination-3");
            if (destinationInput.value === correctDestination.textContent) {
                barcodeInput.disabled = true;
                destinationInput.disabled = true;
                const response = await fetch("/Auto/ChangePalletLocation", {
                    method: 'POST',
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(barcodeInput.value)
                });
                if (response.ok) { 
                    correctDestination.closest("tr").remove();
                    const trCount = document.querySelectorAll("tbody tr").length;
                    console.log(trCount);
                    if (trCount != 0) {
                        const nextRow = correctDestination.closest("tr").nextElementSibling;
                        const firstRow = document.querySelector(".list-item-barcode-3");
                        const tr = firstRow.closest("tr");
                        console.log(firstRow);
                        if (nextRow) {
                            nextRow.classList.add("selected");


                            barcodeInput.placeholder = nextRow.querySelector(".list-item-barcode-3").textContent;
                            destinationInput.placeholder = nextRow.querySelector(".list-item-destination-3").textContent;

                        } else if (firstRow) {
                            tr.classList.add("selected");
                            barcodeInput.placeholder = firstRow.textContent;
                            destinationInput.placeholder = tr.querySelector(".list-item-destination-3").textContent;

                        }
                    }
                    else {
                        window.location.href = "/Auto/FindAutoMission";
                        return;
                    }
                    barcodeInput.disabled = false;
                    barcodeInput.value = "";
                    destinationInput.value = "";
                    barcodeInput.focus();

                } else {

                }
            }
        })
    }
});
