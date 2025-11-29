document.addEventListener("DOMContentLoaded", function () {
    const selectedPallets = new Map();
    let totalWeight = parseFloat(document.getElementById("totalWeight").textContent);
    const formsContainer = document.querySelector(".forms");
    const hiddenFormGroup = document.querySelector(".form-group.d-none");
    const hiddenListItem = document.querySelector("li.py-1.d-none");
    const palletListParent = document.querySelector("ol.ps-3");
    const startButton = document.getElementById("start-button");
    const cancelButton = document.getElementById("cancel-button");
    
    initialize();

    function initialize() {
        if (startButton) {
            initSelectedPallets();
            setupStartButton();
            setupToggleTableButton();
            initializeExistingInputs();
            setupCancelButton();
        }
  
    }
    function initSelectedPallets() {
        const storedPallets = document.querySelectorAll(".form-group:not(.d-none)");
        storedPallets.forEach(group => {
            const input = group.querySelector(".pallet-barcode-input");
            if (input.value != "") {
                selectedPallets.set(parseInt(input.value), {
                    location: 0,
                    height: 0,
                    weight: 0
                });
            }
        });
    }
    function initializeExistingInputs() {
        formsContainer.querySelectorAll(".pallet-barcode-input").forEach(setupInputField);
        formsContainer.querySelectorAll(".delete-pallet-button").forEach(setupDeleteButton);
    }

    function showError(message) {
        clearError();
        const div = document.createElement("div");
        div.className = "error-message";
        div.textContent = message;
        formsContainer.appendChild(div);
    }

    function clearError() {
        const err = formsContainer.querySelector(".error-message");
        if (err) err.remove();
    }
    function addPalletWeight(weight) {
        totalWeight += weight;
        document.getElementById("totalWeight").textContent = totalWeight.toFixed(1);
    }
    function removePalletWeight(weight) {
        totalWeight -= weight;
        if (totalWeight < 0) totalWeight = 0;
        document.getElementById("totalWeight").textContent = totalWeight.toFixed(1);
    }

    function setupCancelButton() {
        cancelButton.addEventListener("click", e => {
            e.preventDefault();
            fetch('/Warehouse/CancelMission');
            window.location.href = "/Home/Index";

        })
    }
    function setupStartButton() {
        if (startButton) {
            startButton.addEventListener("click", (e) => {
                console.log(selectedPallets);
                if (selectedPallets.size === 0) {
                    e.preventDefault();
                    showError("Du måste skanna minst 1 pall!");
                }
                if (checkTwoTenStatusAllPallets()) {
                    e.preventDefault();
                    clearError();
                    showError("Du kan inte tåga 210 pallar, vänligen ta bort den!");
                }
            });
        }
      
    }

    function setupToggleTableButton() {
        const button = document.getElementById("toggle-table-info");
        const wrapper = document.querySelector(".table-wrapper");
        if (button) {
            button.addEventListener("click", () => {
                wrapper.classList.toggle("show");
                button.textContent = wrapper.classList.contains("show")
                    ? "Göm Inkommande Pallar"
                    : "Visa Inkommande Pallar";
            });
        }
       
    }
    function createFormGroup() {
        const group = hiddenFormGroup.cloneNode(true);
        const input = group.querySelector(".pallet-barcode-input");
        const deleteBtn = group.querySelector(".delete-pallet-button");

        group.classList.remove("d-none");
        group.querySelectorAll("*").forEach(x => x.removeAttribute("disabled"));
        deleteBtn.classList.add("d-none");

        formsContainer.appendChild(group);
        setupInputField(input);
        setupDeleteButton(deleteBtn);

        input.focus();
        return group;
    }

    async function setupInputField(input) {
        input.addEventListener("input", async () => {
            clearError();
            const container = input.closest(".form-group");
            const barcode = input.value;
            updateDeleteButtonVisibility(container, barcode);

            if (!barcode || selectedPallets.has(parseInt(barcode))) return;

            const result = await refreshSelectedList(barcode);
            if (result === null) return;
            selectedPallets.set(result.barcode, {
                location: result.location,
                height: result.height,
                weight: result.weight
            });
            let palletStatus = checkTwoTenStatusSingleCase(result.height);
            if (!palletStatus) {
                addPalletWeight(result.weight);
                input.disabled = true;
                updateSelectedUI();
                if (result.height != 210) {
                    createFormGroup();
                }


            } else {
                input.classList.add("border-error");
                input.blur();
                showError("Du kan inte tåga 210 pallar, vänligen ta bort den!");
            }
           
        });
    }
    function checkTwoTenStatusAllPallets() {
        if (selectedPallets.size > 1) {
            for (const [barcode, data] of selectedPallets) {
                if (data.height === 210) return true;
            }
        }
        
        return false;
    }

    function checkTwoTenStatusSingleCase(height) {
        if (selectedPallets.size > 1 && height == 210) {
            return true;
        }
        return false;
    }
    function updateDeleteButtonVisibility(container, id) {
        const btn = container.querySelector(".delete-pallet-button");
        if (btn) btn.classList.toggle("d-none", !id);
    }
    function setupDeleteButton(button) {
        button.addEventListener("click", async () => {
            const container = button.closest(".form-group");
            const input = container.querySelector(".pallet-barcode-input");
            const code = input.value;


            if (!input.disabled) {

                const tryFindPallet = selectedPallets.get(parseInt(input.value));
                if (tryFindPallet != undefined && checkTwoTenStatusSingleCase(tryFindPallet.height)) {
                    selectedPallets.delete(parseInt(code));
                    await fetch("/Warehouse/RemovePalletFromQueueByBarcode", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(code)
                    });
                    button.classList.add("d-none");
                    input.classList.remove("border-error");
                    clearError();
                    updateSelectedUI()
                }
                input.value = "";
                button.classList.add("d-none");
            } else {
                const palletData = selectedPallets.get(parseInt(code));
                removePalletWeight(palletData.weight);
                selectedPallets.delete(parseInt(code));
                updateSelectedUI();
                if (inputCount() > 1) {
                    container.remove();
                } else {
                    input.value = "";
                    input.disabled = false;
                    button.classList.add("d-none");
                }
                    
                await fetch("/Warehouse/RemovePalletFromQueueByBarcode", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(code)
                });
            }
        });
    }
    function inputCount() {
        const container = document.querySelector(".forms");
        return container.querySelectorAll(".form-group:not(.d-none)").length;
    }
    async function refreshSelectedList(val) {
        const response = await fetch("/Warehouse/GetPalletsByBarcodes", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(val)
        });
        if (!response.ok) return null;
        const data = await response.json();
        return data;
    }

    function updateSelectedUI() {
        palletListParent.querySelectorAll("li:not(.d-none):not(.old)").forEach(li => li.remove());
        const pallets = [...selectedPallets];

        for (const [barcode, data] of pallets) {
            if (data.location === 0) continue;

            const li = hiddenListItem.cloneNode(true);
            li.classList.remove("d-none");
            li.textContent = data.location;
            palletListParent.appendChild(li);
        }
    
    }

});
