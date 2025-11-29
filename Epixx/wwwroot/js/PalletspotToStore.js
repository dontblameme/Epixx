document.addEventListener("DOMContentLoaded", function () {
    const palletSpotInput = document.getElementById("location-scan-input");
    const codeInput = document.getElementById("code-input");
    const cancelBtn = document.getElementById("quit-mission");
    const startBtn = document.getElementById("start-packing-area-transfer-button");
    let total_weight = 0;
    init();

    function init() {
        if (startBtn)
            initStartButton();
        if (cancelBtn) 
            initCancelButton();
        if (palletSpotInput)
            initPalletSpotInput();
        if (codeInput) {
            codeInput.addEventListener("input", async e => {
                const input = e.target.value.trim();
                const actualCode = document.getElementById("actual-code").value.trim();
                if (input.length > 2) {
                    if (input === actualCode) {
                        await fetch('/Auto/CheckOffPalletsFromDriver');
                        window.location.href = "/Auto/PackingAreaTransfer";
                    } else {
                        e.target.focus();
                        e.target.select();
                    }
                }
            });
        }
    }
    function initStartButton() {
        startBtn.addEventListener("click", async e => {
            e.preventDefault();
            const response = await fetch('/Auto/GetPalletsCountOnDriver');
            const data = await response.json();
            if (data == 0) {
                showError("Du har inte skannat några pallar!");
            } else {
                window.location.href = "/Auto/PackingAreaConfirmation";
            }
        });
    }
    function initCancelButton() {
        cancelBtn.addEventListener("click", e => {
            e.preventDefault();
            fetch('/Auto/CancelMission');
            window.location.href = "/Home/Index";
        });
    }
    // --- Error Handling ---
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

    // --- Weight Handling ---
    function addPalletWeight(weight) {
        total_weight += weight;
        const el = document.getElementById("totalWeight");
        if (el) el.textContent = total_weight.toFixed(1);
    }

    // --- Placeholder Logic ---
    function findPlaceholderOrChangeView() {
        const pallets = document.querySelectorAll(".list-item-location");

        for (let pallet of pallets) {
            const tr = pallet.closest("tr");
            if (!tr.classList.contains("warning-row") &&
                !tr.classList.contains("d-none")) {
                return pallet.textContent;  // next placeholder
            }
        }

        // If no pallets left → redirect
        window.location.href = "/Auto/PackingAreaConfirmation";
        return "";
    }

    // --- Height + 210 Logic ---
    function checkTwoTenStatusAllCases(height) {
        const hasHidden = document.querySelector("tr.d-none") !== null;
        return hasHidden && height === 210;
    }

    // --- Input Handling ---
    function initPalletSpotInput() {
        palletSpotInput.focus();

        palletSpotInput.addEventListener("input", async e => {
            const input = e.target.value;
            if (input === "") return;
            clearError();

            const pallets = document.querySelectorAll(".list-item-location");

            for (let location of pallets) {
                if (location.textContent !== input) continue;

                const tr = location.closest("tr");
                const height = Number(tr.querySelector(".list-item-height").textContent);

                // 210 rule check
                if (checkTwoTenStatusAllCases(height)) {
                    showError("Du kan inte tåga 210 pallar!");
                    break;
                }

                // Assign pallet
                await fetch('/Auto/AssignPalletToQueue', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(input)
                });

                // If height 210 → auto redirect
                if (height === 210) {
                    window.location.href = "/Auto/PackingAreaConfirmation";
                    return;
                }

                // Add weight
                const weight = parseFloat((tr.querySelector(".list-item-weight").textContent));
                addPalletWeight(weight);

                // Hide row
                tr.classList.add("d-none");

                // Reset input + new placeholder
                palletSpotInput.value = "";
                palletSpotInput.placeholder = findPlaceholderOrChangeView();

                return; 
            }
        });
    }
});
