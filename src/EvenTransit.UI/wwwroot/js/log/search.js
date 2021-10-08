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

    if (response.status === 404){
        alert("Log not found!");
        return;
    }

    if (!(response.status >= 200 && response.status <= 299)) {
        alert("Get log failed!");
        return;
    }
    
    const result = await response.json();

    document.querySelector("#logDetailModal #EventName").value = result.eventName;
    document.querySelector("#logDetailModal #ServiceName").value = result.serviceName;
    document.querySelector("#logDetailModal #LogType").value = getLogType(result.logType);
    document.querySelector("#logDetailModal #Url").value = result.details.request.url;
    document.querySelector("#logDetailModal #Timeout").value = result.details.request.timeout;
    document.querySelector("#logDetailModal #RequestBody").innerHTML = result.details.request.body;
    document.querySelector("#logDetailModal #IsSuccess").value = result.details.response?.isSuccess;
    document.querySelector("#logDetailModal #StatusCode").value = result.details.response?.statusCode;
    document.querySelector("#logDetailModal #Message").value = result.details.message;
    document.querySelector("#logDetailModal #ResponseBody").innerHTML = result.details.response?.response;

    hljs.highlightAll();

    logDetailModal.show();
}

async function filterByCorrelationId(e){
    const id = e.currentTarget.dataset.id;
    const response = await fetch(`/Logs/SearchByCorrelationId/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    const result = await response.json();

    let tbodyRef = document.getElementById('logs').getElementsByTagName('tbody')[0];

    removeLogTableRows(tbodyRef);

    fillLogTableRows(result, tbodyRef, 1);
}

function removeLogTableRows(tbodyRef){
    const rowCount = tbodyRef.rows.length;

    for (let i = 0; i < rowCount; i++) {
        tbodyRef.deleteRow(0);
    }

    removePagination();
}

async function search(page = 1) {
    let tbodyRef = document.getElementById('logs').getElementsByTagName('tbody')[0];
    
    removeLogTableRows(tbodyRef);

    const logDateFrom = getFieldValue("#LogDateFrom", "");
    const logDateTo = getFieldValue("#LogDateTo", "");
    const logType = parseInt(getFieldValue("#LogType", 0));
    const eventName = getFieldValue("select#EventName", "");
    const serviceName = getFieldValue("select#ServiceName", "");
     
    const response = await fetch(`/Logs/Search?LogDateFrom=${logDateFrom}&LogDateTo=${logDateTo}&LogType=${logType}&EventName=${eventName}&ServiceName=${serviceName}&Page=${page}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    const result = await response.json();

    fillLogTableRows(result, tbodyRef, page);
}

function fillLogTableRows(result, tbodyRef, page){
    document.getElementById("errors").classList.add("d-none");

    if (!result.isSuccess){
        document.getElementById("errors").innerHTML = result.message;
        document.getElementById("errors").classList.remove("d-none");
        return;
    }

    if (result.data.items) {
        result.data.items.forEach((item, index) => {
            let newRow = tbodyRef.insertRow();
            let indexCell = newRow.insertCell();
            let eventNameCell = newRow.insertCell();
            let serviceNameCell = newRow.insertCell();
            let typeCell = newRow.insertCell();
            let createdOnCell = newRow.insertCell();
            let actionCell = newRow.insertCell();

            let logDetailActionIcon = document.createElement("i");
            logDetailActionIcon.setAttribute('class', 'fa fa-search fa-fw');

            let logFilterActionIcon = document.createElement("i");
            logFilterActionIcon.setAttribute('class', 'fa fa-filter fa-fw');

            let logDetailButton = document.createElement("button");
            logDetailButton.addEventListener('click', getLogDetails);
            logDetailButton.setAttribute('type', 'button');
            logDetailButton.setAttribute('data-id', item.id);
            logDetailButton.setAttribute("title", "Log Details");
            logDetailButton.setAttribute('class', 'btn btn-sm btn-warning me-1');
            logDetailButton.appendChild(logDetailActionIcon);

            let logFilterButton = document.createElement("button");
            logFilterButton.addEventListener('click', filterByCorrelationId);
            logFilterButton.setAttribute('type', 'button');
            logFilterButton.setAttribute('data-id', item.correlationId);
            logFilterButton.setAttribute("title", "Filter by Correlation Id");
            logFilterButton.setAttribute('class', 'btn btn-sm btn-cs text-white');
            logFilterButton.appendChild(logFilterActionIcon);

            actionCell.appendChild(logDetailButton);
            actionCell.appendChild(logFilterButton);

            actionCell.setAttribute("class", "text-center");

            indexCell.innerHTML = index + 1;
            eventNameCell.innerHTML = item.eventName;
            serviceNameCell.innerHTML = item.serviceName;
            typeCell.innerHTML = getLogType(item.logType);
            createdOnCell.innerHTML = item.createdOn;
        });
    }

    let paginationRef = document.getElementById('logs-pagination');

    let totalPage = result.data.totalPages;
    let firstPage = 1;

    for (let i = 1; i <= totalPage; i++) {
        let liElements = addPaginationItems(i, firstPage, totalPage, page);

        for (let j = 0; j < liElements.length; j++){
            paginationRef.appendChild(liElements[j]);
        }
    }

    activePassivePrevNextItem(page, firstPage, totalPage);

    bindPagination();
}

function bindPagination() {
    document.querySelectorAll('#logs-pagination button')
        .forEach(button => button.addEventListener('click', changePage));
}

async function changePage(e) {
    const page = parseInt(e.currentTarget.dataset.page);
    await search(page);
}

function addPaginationItems(i, firstPage, totalPage, page){
    let elementList = [];
    let dots = "...";

    //Add first page element and prev element
    if (i === firstPage){
        let prevElement = createPaginationElement(page - 1, page, "<<", true);
        prevElement.setAttribute("id", "prev")
        elementList.push(prevElement);

        elementList.push(createPaginationElement(i, page, i, true));
    }

    //Add left dots element
    if (i === page && page > firstPage + 1){
        elementList.push(createPaginationElement(i, page, dots, false, true));
    }

    //Add active page element
    if (i === page && page > firstPage && page < totalPage){
        elementList.push(createPaginationElement(i, page, i, true));
    }

    //Add right dots element
    if (i === page && page < totalPage - 1){
        elementList.push(createPaginationElement(i, page, dots, false, true));
    }

    //Add last page element 
    if (i === totalPage && totalPage > firstPage){
        elementList.push(createPaginationElement(i, page, i, true));
        console.log(i, " last element", page);
    }

    //Add next element
    if (i === totalPage){
        let nextElement = createPaginationElement(page + 1, page, ">>", true);
        nextElement.setAttribute("id", "next");
        elementList.push(nextElement);
    }

    return elementList;
}

function createPaginationElement(i, page, innerHtml, activeControl, isDisabled){
    let numberButton = document.createElement("button");
    numberButton.setAttribute("class", "page-link");
    numberButton.setAttribute("data-page", i);
    numberButton.innerHTML = innerHtml;

    numberButton.disabled = isDisabled === true;

    let cssClass = "page-item";
    if (activeControl !== undefined && activeControl && page === i){
        cssClass = "page-item active";
    }

    let liElement = document.createElement("li");
    liElement.setAttribute("class", cssClass);
    liElement.appendChild(numberButton);
    return liElement;
}

function activePassivePrevNextItem(page, firstPage, totalPage){
    let prev = document.querySelector("#prev button");
    let next = document.querySelector("#next button");

    prev.disabled = page <= firstPage;

    next.disabled = page >= totalPage;
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
