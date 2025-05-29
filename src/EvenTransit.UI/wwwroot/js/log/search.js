const logDetailModal = new bootstrap.Modal(document.getElementById('logDetailModal'), {});
const serviceDropdown = document.querySelector("#ServiceName");
const dateFormat = "DD-MM-YYYY HH:mm:ss";

$('document').ready(function () {
    $('#LogDate').daterangepicker({
        timePicker: true,
        timePicker24Hour: true,
        startDate: moment().startOf('day'),
        endDate: moment().endOf('day'),
        locale: {
            format: 'DD/MM/YYYY HH:mm'
        },
        autoApply: true
    });

    let queryParams = new URLSearchParams(window.location.search);
    window.history.replaceState(null, '', window.location.pathname);
    processQueryParams(queryParams);
});

const select2me = $('.select2me');
select2me.select2();
$("#custom-page").on("select2:select", function (e) {
    if (!e.params.data.id) return;
    search(parseInt(e.params.data.id));
});


$("select#EventName").on('select2:select', async function (e) {
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

function processQueryParams(queryParams) {
    if (!queryParams || queryParams.size === 0)
        return;
        
    let id = queryParams.get('id');
    if (id) {
        $('#LogDate').val(null);
        $('#ServiceName').val(null).trigger('select2:select');
        $('#EventName').val(null).trigger('select2:select');
        $('#LogType').val(null).trigger('select2:select');
        $('#Query').val(null);

        getLogDetails(null, id).then((logDetail) => {
            if (logDetail.createdOn) {
                let createdOn = moment(logDetail.createdOn)
                let dateRangeFrom = createdOn;
                let dateRangeTo = createdOn.add(1, 'minutes').format(dateFormat);
                debugger;
                setTimeout(() => {
                    let datePicker = $('#LogDate').data('daterangepicker');
                    datePicker.setStartDate(dateRangeFrom.format(dateFormat));
                    datePicker.setEndDate(dateRangeTo.format(dateFormat));
                }, 500);
            } else {
                $('#LogDate').val(null);
            }
            
            $('#EventName').val(logDetail.eventName).trigger('select2:select').trigger('change');
            setTimeout(() => {
                $('#ServiceName').val(logDetail.serviceName).trigger('select2:select');
                $('#LogType').val(logDetail.logType).trigger('select2:select');
            }, 500);
        });
        return;
    }
    
    let dateRangeFrom = moment(queryParams.get('logDateFrom'));
    let dateRangeTo = moment(queryParams.get('logDateTo'));
    let serviceName = queryParams.get('serviceName');
    let eventName = queryParams.get('eventName');
    let logType = queryParams.get('logType');
    if (isNaN(logType))
        logType = getLogTypeAsInt(logType);
    let query = queryParams.get('query');

    if (dateRangeFrom.isValid() && dateRangeTo.isValid()) {
        setTimeout(() => {
            let datePicker = $('#LogDate').data('daterangepicker');
            datePicker.setStartDate(dateRangeFrom.format(dateFormat));
            datePicker.setEndDate(dateRangeTo.format(dateFormat));
        }, 500);
    }

    $('#EventName').val(eventName).trigger('select2:select').trigger('change');
    setTimeout(() => {
        $('#ServiceName').val(serviceName).trigger('select2:select');
        $('#LogType').val(logType).trigger('select2:select');
        $('#Query').val(query);

        if (serviceName) {
            search(page = 1).then(() => {
            });
        }
    }, 500);
}

async function getLogDetails(e, idParam) {
    const id = idParam || e.currentTarget.dataset.id;
    const response = await fetch(`/Logs/GetById/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.status === 404) {
        alert("Log not found!");
        return;
    }

    if (!(response.status >= 200 && response.status <= 299)) {
        alert("Get log failed!");
        return;
    }

    const result = await response.json();

    console.log(result);

    document.querySelector("#logDetailModal #Id").value = result.id;
    document.querySelector("#logDetailModal #EventName").value = result.eventName;
    document.querySelector("#logDetailModal #ServiceName").value = result.serviceName;
    document.querySelector("#logDetailModal #LogType").value = getLogType(result.logType);
    document.querySelector("#logDetailModal #Url").innerHTML = result.details.request.url;
    document.querySelector("#logDetailModal #Timeout").innerHTML = result.details.request.timeout;
    document.querySelector("#logDetailModal #RequestBody").innerHTML = result.details.request.body;
    document.querySelector("#logDetailModal #RequestHeaders").innerHTML = JSON.stringify(result.details.request.headers, null, 2);
    document.querySelector("#logDetailModal #IsSuccess").value = result.details.response?.isSuccess;
    document.querySelector("#logDetailModal #StatusCode").value = result.details.response?.statusCode;
    document.querySelector("#logDetailModal #Message").value = result.details.message;
    document.querySelector("#logDetailModal #ResponseBody").innerHTML = result.details.response?.response;
    document.querySelector("#logDetailModal #CreatedOn").value = moment(moment.utc(result.createdOn).toDate()).format(dateFormat) ;
    document.querySelector("#logDetailModal #PublishDate").value = result.details.publishDate && moment(moment.utc(result.details.publishDate).toDate()).format(dateFormat);
    document.querySelector("#logDetailModal #ConsumeDate").value = result.details.consumeDate && moment(moment.utc(result.details.consumeDate).toDate()).format(dateFormat);
    document.querySelector("#logDetailModal #CorrelationId").value = result.details.correlationId;
    document.querySelector("#logDetailModal #Retry").value = result.details.retry;
    document.querySelector("#logDetailModal #TotalDuration").value = result.totalDuration;
    document.querySelector("#logDetailModal #ConsumeDuration").value = result.consumeDuration;

    hljs.highlightAll();

    logDetailModal.show();

    return {
        logType: result.logType,
        eventName: result.eventName,
        serviceName: result.serviceName,
        createdOn: result.createdOn
    }
}

async function filterByCorrelationId(e) {
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

function removeLogTableRows(tbodyRef) {
    const rowCount = tbodyRef.rows.length;

    for (let i = 0; i < rowCount; i++) {
        tbodyRef.deleteRow(0);
    }

    removePagination();
}

async function search(page = 1) {
    let tbodyRef = document.getElementById('logs').getElementsByTagName('tbody')[0];

    const btnSearch = $("#btnSearch");
    const loadingClass = "button--loading";
    btnSearch.addClass(loadingClass);

    removeLogTableRows(tbodyRef);

    const logDate = $("#LogDate");
    const logDateFrom = logDate.data('daterangepicker').startDate.utc().format("DD-MM-YYYY HH:mm");
    const logDateTo = logDate.data('daterangepicker').endDate.utc().format("DD-MM-YYYY HH:mm");

    const logType = parseInt(getFieldValue("#LogType", 0));
    const eventName = getFieldValue("select#EventName", "");
    const serviceName = getFieldValue("select#ServiceName", "");
    const query = getFieldValue("input#Query", "");

    const defaultResponse = {
        message: "Unknown error",
        isSuccess: false
    };
    const promise = fetch(`/Logs/Search?LogDateFrom=${logDateFrom}&LogDateTo=${logDateTo}&LogType=${logType}&EventName=${eventName}&ServiceName=${serviceName}&Page=${page}&Query=${query}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    }).catch(() => {
        setTimeout(() => btnSearch.removeClass(loadingClass), 500);
    });

    const response = await promise;
    setTimeout(() => btnSearch.removeClass(loadingClass), 500);

    let result = response?.json && await response.json();
    result ??= defaultResponse

    fillLogTableRows(result, tbodyRef, page);
}

function toggleCustomPager(totalPage) {
    const div = $(".select2-toggle");
    if (!div.is(":visible") && totalPage > 1) {
        div.show();
    } else if (div.is(":visible") && totalPage <= 1) {
        div.hide();
    }
}

function fillLogTableRows(result, tbodyRef, page) {
    document.getElementById("errors").classList.add("d-none");

    if (!result.isSuccess) {
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
            let retryCell = newRow.insertCell();
            let publishedCell = newRow.insertCell();
            let consumedCell = newRow.insertCell();
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
            retryCell.innerHTML = item.retry;
            publishedCell.innerHTML = item.publishDateString && moment(moment.utc(item.publishDateString, dateFormat).toDate()).format(dateFormat);
            consumedCell.innerHTML = item.consumeDateString && moment(moment.utc(item.consumeDateString, dateFormat).toDate()).format(dateFormat);
            createdOnCell.innerHTML = moment(moment.utc(item.createdOnString, dateFormat).toDate()).format(dateFormat);
        });
    }

    let paginationRef = document.getElementById('logs-pagination');

    let totalPage = result.data.totalPages;
    let firstPage = 1;

    const customPage = $('#custom-page');
    customPage.empty().trigger("change");

    toggleCustomPager(totalPage);

    for (let i = 1; i <= totalPage; i++) {
        let liElements = addPaginationItems(i, firstPage, totalPage, page);

        const newOption = new Option(i, i, false, false);
        customPage.append(newOption).trigger('change');

        for (let j = 0; j < liElements.length; j++) {
            paginationRef.appendChild(liElements[j]);
        }
    }

    customPage.val(page);
    customPage.trigger('change');

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

function addPaginationItems(i, firstPage, totalPage, page) {
    let elementList = [];
    let dots = "...";

    //Add first page element and prev element
    if (i === firstPage) {
        let prevElement = createPaginationElement(page - 1, page, "<<", true);
        prevElement.setAttribute("id", "prev")
        elementList.push(prevElement);

        elementList.push(createPaginationElement(i, page, i, true));
    }

    //Add left dots element
    if (i === page && page > firstPage + 1) {
        elementList.push(createPaginationElement(i, page, dots, false, true));
    }

    //Add active page element
    if (i === page && page > firstPage && page < totalPage) {
        elementList.push(createPaginationElement(i, page, i, true));
    }

    //Add right dots element
    if (i === page && page < totalPage - 1) {
        elementList.push(createPaginationElement(i, page, dots, false, true));
    }

    //Add last page element
    if (i === totalPage && totalPage > firstPage) {
        elementList.push(createPaginationElement(i, page, i, true));
        console.log(i, " last element", page);
    }

    //Add next element
    if (i === totalPage) {
        let nextElement = createPaginationElement(page + 1, page, ">>", true);
        nextElement.setAttribute("id", "next");
        elementList.push(nextElement);
    }

    return elementList;
}

function createPaginationElement(i, page, innerHtml, activeControl, isDisabled) {
    let numberButton = document.createElement("button");
    numberButton.setAttribute("class", "page-link");
    numberButton.setAttribute("data-page", i);
    numberButton.innerHTML = innerHtml;

    numberButton.disabled = isDisabled === true;

    let cssClass = "page-item";
    if (activeControl !== undefined && activeControl && page === i) {
        cssClass = "page-item active";
    }

    let liElement = document.createElement("li");
    liElement.setAttribute("class", cssClass);
    liElement.appendChild(numberButton);
    return liElement;
}

function activePassivePrevNextItem(page, firstPage, totalPage) {
    let prev = document.querySelector("#prev button");
    let next = document.querySelector("#next button");

    if (prev == null) return;

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

function getLogTypeAsInt(logType) {
    if (logType === "None") return 0;
    if (logType === "Success") return 1;
    if (logType === "Fail") return 2;
}

async function copySearchFilter() {
    let logDatePicker = $("#LogDate").data('daterangepicker');
    
    let params = {
        logDateFrom: logDatePicker.startDate.format(),
        logDateTo: logDatePicker.endDate.format(),
        logType: parseInt(getFieldValue("#LogType", 0)),
        eventName: getFieldValue("select#EventName", ""),
        serviceName: getFieldValue("#ServiceName", ""),
        query: getFieldValue("input#Query", ""),
    };

    let filter = `${window.location.href}?${jQuery.param(params)}`;
    await copyToClipboard(filter);
    try {
        toastr.success('Search Filter copied to clipboard!');
    } catch (err) {
        toastr.warning('Unable to copy link to clipboard!');
    }
}

async function copyDetailLink(id) {
    if (!id) {
        id = document.querySelector("#logDetailModal #Id").value;
    }

    let params = {
        id: id,
    };

    let filter = `${window.location.href}?${jQuery.param(params)}`;

    try {
        await copyToClipboard(filter);
        toastr.success('Log detail link copied to clipboard!');
    } catch (err) {
        toastr.warning('Unable to copy link to clipboard!');
    }
}

async function copyToClipboard(content) {
    if (navigator.clipboard && window.isSecureContext) {
        await navigator.clipboard.writeText(content);
    } else {
        const textArea = document.createElement("textarea");
        textArea.value = content;
        
        textArea.style.position = "absolute";
        textArea.style.left = "-999999px";

        if ($('.modal.show').length > 0) {
            $('.modal.show').find('.modal-footer').append(textArea);
        } else {
            document.body.prepend(textArea);
        }

        textArea.select();

        try {
            document.execCommand('copy');
        } catch (error) {
            console.error(error);
        } finally {
            textArea.remove();
        }
    }
}