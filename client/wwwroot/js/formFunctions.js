// Utility: add a fade-in row animation (CSS handles the animation)
export function addRowToTable(tbody, rowHtml) {
    const newRow = document.createElement('tr');
    newRow.innerHTML = rowHtml;
    newRow.classList.add('fade-in-row');
    tbody.insertBefore(newRow, tbody.firstChild);
    return newRow;
}

// Utility: delete a row with fade-out animation
export function deleteRow(row) {
    row.classList.add('fade-out-row');
    setTimeout(() => row.remove(), 300);
}

// Utility: attach delete event listener
export function attachDeleteHandler(button, handler) {
    button.addEventListener('click', async () => {
        button.disabled = true;
        const original = button.innerHTML;
        button.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Borrando...';
        await handler(button);
        button.disabled = false;
        button.innerHTML = original;
    });
}