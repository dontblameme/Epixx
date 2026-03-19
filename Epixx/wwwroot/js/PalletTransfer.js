document.addEventListener("DOMContentLoaded", function () {
    const startupButton = document.getElementById("start-button-for-transfer");
    const shutdownButton = document.getElementById("cancel-mission-button-for-transfer");
    const locationOrBarcodeInput = document.getElementById("location-or-barcode");
    const menuButton = document.getElementById("back-to-menu-button-for-transfer");
    if (startupButton) {
        init();
    }
    async function checkAndUpdateBackend() {
        const response = await fetch('/Auto/ChangePalletStatusToConfirmTransfer', {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(locationOrBarcodeInput.value)
        })
        if (!response.ok)
            return null;
        const data = await response.text();
        return data
    }
    function showError(message) {
        clearError();
        const container = document.querySelector(".choosen");
        if (!container) return;
        const div = document.createElement("div");
        div.className = "error-message";
        div.textContent = message;
        container.appendChild(div);
    }

    function clearError() {
        const err = document.querySelector(".error-message");
        if (err) err.remove();
    }
    function updateUIUponPalletConfirmation(placeholder, htmlItem) {
        const tr = htmlItem.closest("tr");
        const nextRow = tr.nextElementSibling;

        const currentSelected = document.querySelector(".selected");
        if (currentSelected) currentSelected.classList.remove("selected");

        tr.remove();

        if (!nextRow) {
            console.log("Done scanning!");
            locationOrBarcodeInput.value = "";
            locationOrBarcodeInput.focus();
            return;
        }

        nextRow.classList.add("selected");

        locationOrBarcodeInput.value = "";

        if (placeholder === "Barcode") {
            const barcode = nextRow.querySelector(".barcode-list-item-2");
            locationOrBarcodeInput.placeholder = barcode.textContent;
        } else {
            const location = nextRow.querySelector(".location-list-item-2");
            locationOrBarcodeInput.placeholder = location.textContent;
        }

        locationOrBarcodeInput.focus();
    }
    function init() {
        menuButton.addEventListener("click", () => {
            window.location.href = "/Home/Index";
        });
        startupButton.addEventListener("click", async () => {
            const response = await fetch('/Auto/CheckIfNoPalletsAreScanned');
            const hasPallets = await response.json();
            if (!hasPallets) {
                showError("Du har inte skannat några pallar!");
            } else {
                window.location.href = '/Auto/PalletTransferConfirmation';
            }
        })
        shutdownButton.addEventListener("click", () => {
            fetch('/Auto/CancelMission');
            window.location.href = "/Home/Index";
        });
        locationOrBarcodeInput.addEventListener("input", async () => {
            const inputValue = locationOrBarcodeInput.value;
            clearError();
            const locations = document.querySelectorAll(".location-list-item-2");
            const barcodes = document.querySelectorAll(".barcode-list-item-2");
            if (locationOrBarcodeInput.value.length > 3) {
                for (const item of locations) {
                    if (item.textContent === inputValue) {
                        const tr = item.closest("tr");
                        const height = tr.querySelector(".height-list-item-2").textContent;
                        const response = await fetch('/Auto/TwoTenCheck?incomingpalletheight=' + height + "&status=ConfirmTransfer");
                        const isTwoTen = await response.json();
                        if (isTwoTen) {
                            showError("Du kan inte tåga 210 pallar, vänligen ta bort den!");
                            locationOrBarcodeInput.focus();
                            locationOrBarcodeInput.select();
                        } else {
                            locationOrBarcodeInput.disabled = true;
                            const placeholder = await checkAndUpdateBackend();
                            locationOrBarcodeInput.disabled = false;
                            updateUIUponPalletConfirmation(placeholder, item);
                            if (height == "210") {
                                window.location.href = '/Auto/PalletTransferConfirmation';
                            }
                            const trCount = document.querySelectorAll("tbody tr").length;
                            if (trCount == 0) {
                                window.location.href = '/Auto/PalletTransferConfirmation';
                            }
                        }

                        return;
                    }
                }

                for (const item of barcodes) {
                    if (item.textContent === inputValue) {
                        const tr = item.closest("tr");
                        const height = tr.querySelector(".height-list-item-2").textContent;
                        console.log(height);
                        const response = await fetch('/Auto/TwoTenCheck?incomingpalletheight=' + height + "&status=ConfirmTransfer");
                        const isTwoTen = await response.json();
                        if (isTwoTen) {
                            showError("Du kan inte tåga 210 pallar, vänligen ta bort den!");
                            locationOrBarcodeInput.focus();
                            locationOrBarcodeInput.select();
                        } else {
                            locationOrBarcodeInput.disabled = true;
                            const placeholder = await checkAndUpdateBackend();
                            locationOrBarcodeInput.disabled = false;
                            updateUIUponPalletConfirmation(placeholder, item);
                            if (height == "210") {
                                window.location.href = '/Auto/PalletTransferConfirmation';
                            }
                            const trCount = document.querySelectorAll("tbody tr").length;
                            if (trCount == 0) {
                                window.location.href = '/Auto/PalletTransferConfirmation';
                            }
                        }

                        return;
                    }
                }
            }
           
        });
    }

});