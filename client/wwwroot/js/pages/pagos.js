import { addRowToTable } from '/js/formFunctions.js';
export function initPagosPage() {
    const forms = document.querySelectorAll(".form-pago")
    forms.forEach(form => {
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            const formData = new FormData(form);
            const button = form.querySelector("button");

            button.disabled = true;
            button.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Agregando...';

            await new Promise(r => setTimeout(r, 750));

            try {
                const res = await fetch("/Pago/Create", {
                    method: "POST",
                    body: formData
                });

                if (!res.ok) {
                    toast("Error en la solicitud al servidor", "danger");
                    button.disabled = false;
                    button.textContent = "Agregar";
                    return;
                }

                const result = await res.json();
                if (result.state === "error") {
                    toast(result.msg, "danger");
                    button.disabled = false;
                    button.textContent = "Agregar";
                    return;
                }

                toast(result.msg, result.state);

                form.reset();
                button.disabled = false;
                button.textContent = "Agregar";

                const mesActual = new Date().getMonth() + 1
                if (formData.get("FechaDePago")?.split("-")[1] == mesActual || formData.get("PrimerPago")?.split("-")[1] == mesActual) {
                    const tbody = document.querySelector('table tbody');
                    const newRowHtml = `
                            <td>${result.descripcion}</td>
                            <td>${result.tipoDeGasto}</td>
                            <td>${result.tipoDePago}</td>
                            <td>${result.metodo}</td>
                            <td>${result.fechaPago}</td>
                            <td>${result.montoTotal}</td>
                        `;
                    addRowToTable(tbody, newRowHtml);

                    // Update total
                    const totalField = document.querySelector("#expenses");
                    const Voriginal = Number(totalField.textContent);
                    const montoNuevo = Number(result.montoTotal);
                    const Vactual = (Voriginal + montoNuevo).toFixed(2);
                    totalField.textContent = Vactual;

                    // Update chart
                    if (window.expensesChart && result.fechaPago) {
                        const fecha = new Date(result.fechaPago);
                        const mesNumero = fecha.getMonth();

                        const meses = ["ene.", "feb.", "mar.", "abr.", "may.", "jun.", "jul.", "ago.", "set.", "oct.", "nov.", "dic."];
                        const mesLabel = meses[mesNumero];

                        const chart = window.expensesChart;
                        const index = chart.data.labels.indexOf(mesLabel);

                        if (index !== -1) {
                            chart.data.datasets[0].data[index] += Number(result.montoTotal);
                            chart.update();
                        }
                    }
                }


            } catch (err) {
                console.error("Error enviando el formulario:", err);
                toast("Error de conexión o del servidor", "error");
                button.disabled = false;
                button.textContent = "Agregar";
            }
        });
    });

    // select tipo de pago
    const paymentUnicoForm = document.getElementById("payment-unico");
    const paymentCuotasForm = document.getElementById("payment-cuotas");
    const paymentSubscripcionForm = document.getElementById("payment-subscripcion");

    const formsMap = {
        unico: paymentUnicoForm,
        cuotas: paymentCuotasForm,
        subscripcion: paymentSubscripcionForm
    };

    document.getElementById("paymentSelection").addEventListener("change", (e) => {
        const value = e.target.value;

        // Hide all
        Object.values(formsMap).forEach(form => {
            if (form) form.style.display = "none";
        });

        // Show selected
        if (formsMap[value]) {
            formsMap[value].style.display = "block";
        }
    });

}