document.addEventListener("DOMContentLoaded", function () {
    const selectedPallets = new Map();
    const formsContainer = document.querySelector(".forms");
    const hiddenFormGroup = document.querySelector(".form-group.d-none");
    const hiddenListItem = document.querySelector("li.py-1.d-none");
    const palletListParent = document.querySelector("ol.ps-3");
    const startButton = document.getElementById("start-button");
    const cancelButton = document.getElementById("cancel-button");
    
    initialize();

    function initialize() {
        if (startButton) {
            setupStartButton();
            setupToggleTableButton();
            initializeExistingInputs();
            refreshPalletList();
            setupCancelButton();
            setInterval(refreshPalletList, 10000);
        }
  
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

    async function refreshPalletList() {
        const res = await fetch("/Warehouse/_PalletTable");
        const html = await res.text();
        document.getElementById("palletsBody").innerHTML = html;
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
            const barcode = input.value.trim();
            updateDeleteButtonVisibility(container, barcode);

            if (!barcode || selectedPallets.has(parseInt(barcode))) return;

            const result = await refreshSelectedList(barcode);
            if (result === null) return;
            selectedPallets.set(result.barcode, {
                location: result.location,
                height: result.height
            });
            let palletStatus = checkTwoTenStatusSingleCase(result.height);
            if (!palletStatus) {
               
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
            const code = input.value.trim();


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
        palletListParent.querySelectorAll("li:not(.d-none)").forEach(li => li.remove());
        const reversed = [...selectedPallets].reverse();

        reversed.forEach(([barcode, data]) => {
            const li = hiddenListItem.cloneNode(false);
            li.classList.remove("d-none");
            li.textContent = data.location;
            palletListParent.appendChild(li);
        });        
    }

});
