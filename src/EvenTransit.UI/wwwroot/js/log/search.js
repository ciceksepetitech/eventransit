const logDetailModal = new bootstrap.Modal(document.getElementById('logDetailModal'), {});
const serviceDropdown = document.querySelector("#ServiceName");
document.querySelector("select#EventName").addEventListener("change", async (e) => {
    let selectedEvent = e.target.value;

    clearServiceDropdown();

    if (selectedEvent === "") return;

    const response = await fetch(`/Logs/GetServices/${selectedEvent}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const result = await response.json();
    for (let i = 0; i < result.length; i++) {
        let serviceName = result[i];
        let option = document.createElement("option");
        option.setAttribute("value", serviceName);
        option.text = serviceName;

        serviceDropdown.appendChild(option);
    }
});

async function getLogDetails(e) {
    const id = e.currentTarget.dataset.id;
    const response = await fetch(`/Logs/GetById/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    const result = await response.json();

    document.querySelector("#logDetailModal #EventName").value = result.eventName;
    document.querySelector("#logDetailModal #ServiceName").value = result.serviceName;
    document.querySelector("#logDetailModal #LogType").value = getLogType(result.logType);
    document.querySelector("#logDetailModal #Url").value = result.details.request.url;
    document.querySelector("#logDetailModal #Timeout").value = result.details.request.timeout;
    document.querySelector("#logDetailModal #RequestBody").innerHTML = result.details.request.body;
    document.querySelector("#logDetailModal #IsSuccess").value = result.details.response?.isSuccess;
    document.querySelector("#logDetailModal #StatusCode").value = result.details.response.statusCode;
    document.querySelector("#logDetailModal #Message").value = result.details.message;
    document.querySelector("#logDetailModal #ResponseBody").innerHTML = result.details.response.response;

    hljs.highlightAll();

    logDetailModal.show();
}

function bindPagination() {
    console.log("a");
    document.querySelectorAll('#logs-pagination button')
        .forEach(button => button.addEventListener('click', changePage));
}

async function changePage(e) {
    const page = parseInt(e.currentTarget.dataset.page);
    console.log(page);
    await search(page);
}

async function search(page = 1) {
    let tbodyRef = document.getElementById('logs').getElementsByTagName('tbody')[0];
    const rowCount = tbodyRef.rows.length;

    for (let i = 0; i < rowCount; i++) {
        tbodyRef.deleteRow(0);
    }

    removePagination();

    const data = {
        LogDateFrom: getFieldValue("#LogDateFrom", null),
        LogDateTo: getFieldValue("#LogDateTo", null),
        LogType: parseInt(getFieldValue("#LogType", 0)),
        EventName: getFieldValue("select#EventName", null),
        ServiceName: getFieldValue("select#ServiceName", null),
        Keyword: getFieldValue("#Keyword", null),
        Page: page
    };
    const response = await fetch('/Logs/Search', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
    const result = await response.json();

    if (result.items) {
        result.items.forEach((item, index) => {
            let newRow = tbodyRef.insertRow();
            let indexCell = newRow.insertCell();
            let eventNameCell = newRow.insertCell();
            let serviceNameCell = newRow.insertCell();
            let typeCell = newRow.insertCell();
            let createdOnCell = newRow.insertCell();
            let actionCell = newRow.insertCell();

            let logDetailActionIcon = document.createElement("i");
            logDetailActionIcon.setAttribute('class', 'fa fa-search fa-fw');

            let logDetailButton = document.createElement("button");
            logDetailButton.addEventListener('click', getLogDetails);
            logDetailButton.setAttribute('type', 'button');
            logDetailButton.setAttribute('data-id', item.id);
            logDetailButton.setAttribute('class', 'btn btn-sm btn-warning');
            logDetailButton.appendChild(logDetailActionIcon);
            actionCell.appendChild(logDetailButton);

            indexCell.innerHTML = index + 1;
            eventNameCell.innerHTML = item.eventName;
            serviceNameCell.innerHTML = item.serviceName;
            typeCell.innerHTML = getLogType(item.logType);
            createdOnCell.innerHTML = item.createdOn;
        });
    }

    let paginationRef = document.getElementById('logs-pagination');

    for (let i = 1; i <= result.totalPages; i++) {
        let numberButton = document.createElement("button");
        numberButton.setAttribute("class", "page-link");
        numberButton.setAttribute("data-page", i);
        numberButton.innerHTML = i;

        const cssClass = page === i ? "page-item active" : "page-item";

        let liElement = document.createElement("li");
        liElement.setAttribute("class", cssClass);
        liElement.appendChild(numberButton);

        paginationRef.appendChild(liElement);
    }

    bindPagination();
}

function clearServiceDropdown() {
    const length = serviceDropdown.options.length - 1;
    for (let i = length; i > 0; i--) {
        serviceDropdown.remove(i);
    }
}

function removePagination() {
    let paginationItems = document.querySelectorAll("#logs-pagination li");

    for (let i = 0; i < paginationItems.length; i++) {
        paginationItems[i].remove();
    }
}

function getFieldValue(selector, defaultValue) {
    const val = document.querySelector(selector).value;

    if (val === "" || val === undefined || val === null)
        return defaultValue;
    return val;
}

function getLogType(logType) {
    if (logType === 0) return "None";
    if (logType === 1) return "Success";
    if (logType === 2) return "Fail";
}
