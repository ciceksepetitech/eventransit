const saveForm = document.querySelector("#save-service");
saveForm.addEventListener("submit", async e => {
    e.stopPropagation();
    e.preventDefault();

    const formData = Object.fromEntries(new FormData(e.target).entries());
    let timeout = 0;
    if (formData.Timeout !== "") timeout = parseInt(formData.Timeout);
    
    formData.Timeout = timeout;
    
    const response = await fetch('/Events/SaveService', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
    });

    const result = await response.json();
    console.log(result);
});