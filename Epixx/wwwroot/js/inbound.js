document.addEventListener("DOMContentLoaded", function () {
    const barcodeInput = document.getElementById("barcode");
    const locationInput = document.getElementById("location");
    const barcodeListItems = document.querySelectorAll(".barcode-list-item");
    const cancelButton = document.getElementById("cancel-button-for-storing");
    const backToMenuButton = document.getElementById("back-to-menu-button");
    init();
    function init() {
        initCancelButton();
        initBackToMenuButton();
        checkBarCodeInputInit();
        checkLocationInput();
    }
    function initCancelButton() {
        if (cancelButton) {
            cancelButton.addEventListener("click", e => {
                e.preventDefault();
                fetch('/Warehouse/CancelMission');
                window.location.href = "/Warehouse/InboundLogistics";
            })
        }
    }
    function initBackToMenuButton() {
        if (backToMenuButton) {
            backToMenuButton.addEventListener("click", e => {
                e.preventDefault();
                fetch('/Warehouse/CancelMission');
                window.location.href = "/Home/Index";
            })
        }
      
    }
    function checkBarCodeInputInit() {
        if (barcodeInput) {
            barcodeInput.addEventListener("input", () => {
                barcodeListItems.forEach(item => {
                    if (item.textContent == barcodeInput.value) {
                        const currentSelected = document.querySelector(".selected");
                        const selectedPallet = item.closest("tr");
                        currentSelected.classList.remove("selected");
                        selectedPallet.classList.add("selected");
                        locationInput.disabled = false;
                        locationInput.focus();
                        barcodeInput.disabled = true;
                        locationInput.placeholder = selectedPallet.querySelector(".location-list-item").textContent;
                    }
                });
            });
        }
    }
    function checkLocationInput() {
        if (locationInput) {
            locationInput.addEventListener("input", () => {
                const currentSelected = document.querySelector(".selected");
                const selectedLocation = currentSelected.querySelector(".location-list-item").textContent;
                if (locationInput.value == selectedLocation) {
                    fetch('/Warehouse/StoreInboundShipment', {
                        method: 'POST',
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(barcodeInput.value)
                    })
                        .then(response => response.json())
                        .then(data => {
                            if (data.length == 0) {
                                window.location.href = "/Warehouse/InboundLogistics";
                            } else {
                                location.reload();
                            }
                        });

                }
            });
        }
    }
    
});
