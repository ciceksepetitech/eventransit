const serviceModal = new bootstrap.Modal(document.getElementById('newServiceModal'), {});
const saveForm = document.querySelector("#save-service");
saveForm.addEventListener("submit", async e => {
    e.stopPropagation();
    e.preventDefault();

    const formData = Object.fromEntries(new FormData(e.target).entries());
    let timeout = 0;
    if (formData.Timeout !== "") timeout = parseInt(formData.Timeout);
    let delaySeconds = 0;
    if (formData.DelaySeconds !== "") delaySeconds = parseInt(formData.DelaySeconds);

    formData.Timeout = timeout;
    formData.DelaySeconds = delaySeconds;
    formData.Headers = {};
    formData.CustomBodyMap = {};

    let headerItems = document.querySelectorAll("#headers tbody tr");

    for (let i = 0; i < headerItems.length; i++) {
        let row = headerItems[i];
        let keyInput = row.querySelector("input.header-key").value;
        formData.Headers[keyInput] = row.querySelector("input.header-value").value;
    }

    let customBodyMapItems = document.querySelectorAll("#body-map tbody tr");

    for (let i = 0; i < customBodyMapItems.length; i++) {
        let row = customBodyMapItems[i];
        let keyInput = row.querySelector("input.header-key").value;
        formData.CustomBodyMap[keyInput] = row.querySelector("input.header-value").value;
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

    if (!result.isSuccess) {
        document.getElementById("service-errors").innerHTML = result.message;
        document.getElementById("service-errors").classList.remove("d-none");

        setTimeout(function () {
            document.getElementById("service-errors").classList.add("d-none");
            document.getElementById("service-errors").innerHTML = "";
        }, 5000);
        return;
    }

    saveForm.reset();

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

    if (response.status === 404) {
        alert("Service not found!");
        return;
    }

    if (!(response.status >= 200 && response.status <= 299)) {
        alert("Service update failed!");
        return;
    }

    const result = await response.json();

    const timeout = result.timeout === 0 ? "" : result.timeout;
    const delaySeconds = result.delaySeconds === 0 ? "" : result.delaySeconds;

    document.getElementById("HiddenServiceName").value = result.name;
    document.querySelector("#ServiceName").value = result.name;
    document.querySelector("#Url").value = result.url;
    document.querySelector("#Timeout").value = timeout;
    document.querySelector("#DelaySeconds").value = delaySeconds;
    document.querySelector("#Method").value = result.method;

    if (document.querySelector("#Url").value !== undefined)
        document.querySelector("#Url").setAttribute("readonly", "readonly");

    const headers = result.headers;
    clearTable("headers");

    for (const key in headers) {
        const value = headers[key];
        addNewKVItem(key, value, "headers");
    }

    const customBodyMap = result.customBodyMap;
    clearTable("body-map");

    for (const key in customBodyMap) {
        const value = customBodyMap[key];
        addNewKVItem(key, value, "body-map");
    }

    serviceModal.show();
}

async function deleteService(eventId, serviceName) {
    if (confirm("Are you sure?")) {
        const response = await fetch(`/Events/DeleteService/${eventId}/${serviceName}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const result = await response.json();

        if (!result.isSuccess) {
            alert("Service not deleted!");
            return;
        }

        window.location.reload();
    }
}

document.querySelector("#add-header").addEventListener("click", e => {
    addNewKVItem('', '', 'headers');
});

document.querySelector("#add-body-map").addEventListener("click", e => {
    addNewKVItem('', '', 'body-map');
});

function addNewKVItem(key, value, id) {
    let tbodyRef = document.getElementById(id).getElementsByTagName('tbody')[0];
    let newRow = tbodyRef.insertRow();
    let actionCell = newRow.insertCell();
    let keyCell = newRow.insertCell();
    let valueCell = newRow.insertCell();

    let removeActionIcon = document.createElement("i");
    removeActionIcon.setAttribute('class', 'fa fa-times');

    let removeActionButton = document.createElement("button");
    removeActionButton.addEventListener('click', removeKVItem(id));
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

function removeKVItem(id) {
    return function (e) {
        const rowIndex = e.target.parentElement.parentElement.parentElement.rowIndex - 1;
        document.querySelector('#' + id + ' tbody').deleteRow(rowIndex);
    }
}

function clearTable(id) {
    let tbody = document.getElementById(id).getElementsByTagName('tbody')[0];
    tbody.innerHTML = "";
}

function clearNewProcessModal() {
    document.getElementById("HiddenServiceName").value = "";
    saveForm.reset();
    clearTable("headers");
    clearTable("body-map");
    document.querySelector("#Url").removeAttribute("disabled");
}

async function logsbuttonClick(eventName, serviceName) {
    let url = `/Logs?logType=1&eventName=${eventName}&serviceName=${serviceName}`;

    window.open(url, '_blank');
}