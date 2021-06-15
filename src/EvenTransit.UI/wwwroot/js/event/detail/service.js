const serviceModal = new bootstrap.Modal(document.getElementById('newServiceModal'), {});
const saveForm = document.querySelector("#save-service");
saveForm.addEventListener("submit", async e => {
    e.stopPropagation();
    e.preventDefault();

    const formData = Object.fromEntries(new FormData(e.target).entries());
    let timeout = 0;
    if (formData.Timeout !== "") timeout = parseInt(formData.Timeout);

    formData.Timeout = timeout;
    formData.Headers = {};

    let headerItems = document.querySelectorAll("#headers tbody tr");

    for (let i = 0; i < headerItems.length; i++) {
        let row = headerItems[i];
        let keyInput = row.querySelector("input.header-key").value;
        formData.Headers[keyInput] = row.querySelector("input.header-value").value;
    }

    const response = await fetch('/Events/SaveService', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
    });

    const result = await response.json();
    document.getElementById("service-errors").classList.add("d-none");

    if (!result.isSuccess){
        let errorMessage = "";
        for (let i = 0; i < result.errors.length; i++) {
            errorMessage += result.errors[i] + "<br/>";
        }
        document.getElementById("service-errors").innerHTML = errorMessage;
        document.getElementById("service-errors").classList.remove("d-none");
        return;
    }

    serviceModal.hide();
    window.location.reload();
});

async function editService(eventId, serviceName) {
    const response = await fetch(`/Events/GetServiceDetails/${eventId}/${serviceName}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    const result = await response.json();

    document.querySelector("#ServiceName").value = result.name;
    document.querySelector("#Url").value = result.url;
    document.querySelector("#Timeout").value = result.timeout;

    const headers = result.headers;
    for (const key in headers) {
        const value = headers[key];
        addNewHeaderItem(key, value);
    }

    serviceModal.show();
}

async function deleteService(eventId, serviceName) {
    if (confirm("Are you sure?")) {
        const response = await fetch(`/Events/DeleteService/${eventId}/${serviceName}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const result = await response.json();
        
        if(!result.success){
            alert("Service not deleted!");
            return;
        }

        window.location.reload();
    }
}

document.querySelector("#add-header").addEventListener("click", e => {
    addNewHeaderItem('', '');
});

function addNewHeaderItem(key, value) {
    let tbodyRef = document.getElementById('headers').getElementsByTagName('tbody')[0];
    let newRow = tbodyRef.insertRow();
    let actionCell = newRow.insertCell();
    let keyCell = newRow.insertCell();
    let valueCell = newRow.insertCell();

    let removeActionIcon = document.createElement("i");
    removeActionIcon.setAttribute('class', 'fa fa-times');

    let removeActionButton = document.createElement("button");
    removeActionButton.addEventListener('click', removeHeaderItem);
    removeActionButton.setAttribute('type', 'button');
    removeActionButton.setAttribute('class', 'btn btn-sm btn-danger');
    removeActionButton.appendChild(removeActionIcon);
    actionCell.appendChild(removeActionButton);

    let keyInput = document.createElement("input");
    keyInput.setAttribute('type', 'text');
    keyInput.setAttribute('class', 'form-control form-control-sm header-key');
    keyInput.value = key;
    keyCell.appendChild(keyInput);

    let valueInput = document.createElement("input");
    valueInput.setAttribute('type', 'text');
    valueInput.setAttribute('class', 'form-control form-control-sm header-value');
    valueInput.value = value;
    valueCell.appendChild(valueInput);
}

function removeHeaderItem(e) {
    const rowIndex = e.target.parentElement.parentElement.parentElement.rowIndex - 1;
    document.querySelector('#headers tbody').deleteRow(rowIndex);
}