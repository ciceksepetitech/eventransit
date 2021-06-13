const serviceModal = new bootstrap.Modal(document.getElementById('newEventModal'), {});
const saveForm = document.querySelector("#save-event");
saveForm.addEventListener("submit", async e => {
    e.stopPropagation();
    e.preventDefault();

    const formData = Object.fromEntries(new FormData(e.target).entries());

    const response = await fetch('/Events/SaveEvent', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
    });

    const result = await response.json();

    serviceModal.hide();
    window.location.reload();
});