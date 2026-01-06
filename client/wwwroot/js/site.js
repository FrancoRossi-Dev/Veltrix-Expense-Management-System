// toast
function toast(txt, type, timer = 2500) {
    setTimeout(function () {
        const elToast = document.querySelector("#toastContainer");
        const toast = document.createElement('div');
        toast.classList.add(`toast`, `${type}`);
        toast.innerHTML = `
            <div class="content">
              <div>
                <p>${txt}</p>
              </div>
            </div>
           `;
        elToast.appendChild(toast);
        toast.style.animation = `toastPopUP ${timer}ms ease-in-out forwards`;

        setTimeout(function () {
            elToast.removeChild(toast);
        }, timer);
    });
}


// Code for modal
document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("deleteModal");
    const confirmBtn = document.getElementById("confirmDeleteBtn");
    const modalMsg = document.getElementById("modalMessage");
    const cancelBtn = document.getElementById("cancelDeleteBtn");
    let currentForm = null;

    document.querySelectorAll(".delete-form").forEach(form => {
        form.addEventListener("submit", function (e) {
            e.preventDefault();
            currentForm = form;
            modalMsg.textContent = "¿Esta seguro que desea borrar esta informacion de forma permanente?"
            confirmBtn.textContent = "Borrar"
            modal.style.display = "flex";
        });
    });

    const logoutForm = document.querySelector(".logout-form");
    if (logoutForm) {
        logoutForm.addEventListener("submit", function (e) {
            e.preventDefault();
            currentForm = logoutForm;
            modalMsg.textContent = "¿Esta seguro que desea salir del sistema?"
            confirmBtn.textContent = "Salir"
            modal.style.display = "flex";
        });
    }

    confirmBtn.addEventListener("click", function () {
        if (currentForm) {
            currentForm.submit();
        }
        modal.style.display = "none";
        currentForm = null;
    });

    cancelBtn.addEventListener("click", function () {
        modal.style.display = "none";
        currentForm = null;
    });
});




