function refreshCart(autoOpen = false) {
    fetch('/Home/GetCartPartial')
        .then(res => res.text())
        .then(html => {
            document.getElementById('cartContentArea').innerHTML = html;
            
            if (autoOpen) {
                console.log("Attempting to open Offcanvas...");
                var cartSidebar = document.getElementById('votingCartSidebar');
                var bsOffcanvas = bootstrap.Offcanvas.getInstance(cartSidebar);
                if (!bsOffcanvas) {
                    bsOffcanvas = new bootstrap.Offcanvas(cartSidebar);
                }
                bsOffcanvas.show();
                console.log("Offcanvas show() triggered.");
            }
        })
        .catch(err => console.error("Error loading cart HTML:", err));
}

function toggleGlobalCandidate(id, shouldOpenCart) {
    console.log(`Button clicked! Candidate ID: ${id}, shouldOpenCart passed as: ${shouldOpenCart}`);

    fetch(`/Home/ToggleSavedCandidate?candidateId=${id}`, { method: 'POST' })
        .then(res => res.json())
        .then(data => {
            console.log("Response from server:", data);
            
            // This safely handles if your server returns "isSaved" OR "IsSaved" (capital I)
            const savedState = data.isSaved !== undefined ? data.isSaved : data.IsSaved;
            
            console.log(`Calculated saved state: ${savedState}`);

            refreshCart(savedState && shouldOpenCart);
            
            var localBtn = document.getElementById(`saveBtn-${id}`);
            if (localBtn) {
                if (savedState) {
                    localBtn.classList.replace('btn-outline-warning', 'btn-warning');
                    localBtn.innerHTML = '<i class="bi bi-star-fill"></i> Saved';
                } else {
                    localBtn.classList.replace('btn-warning', 'btn-outline-warning');
                    localBtn.innerHTML = '<i class="bi bi-star"></i> Save to Guide';
                }
            }
        })
        .catch(err => console.error("Error in toggle function:", err));
}

document.addEventListener("DOMContentLoaded", function() {
    refreshCart(false);
});