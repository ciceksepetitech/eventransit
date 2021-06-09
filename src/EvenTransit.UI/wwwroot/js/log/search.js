const logDetailModal = new bootstrap.Modal(document.getElementById('logDetailModal'), {});
const serviceDropdown = document.querySelector("#ServiceName");
document.querySelector("#EventName").addEventListener("change", async (e) => {
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

async function getLogDetails(id) {
    const response = await fetch(`/Logs/GetById/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    const result = await response.json();

    document.querySelector("#logDetailModal #EventName").value = result.eventName;
    document.querySelector("#logDetailModal #ServiceName").value = result.serviceName;
    document.querySelector("#logDetailModal #LogType").value = result.type;
    document.querySelector("#logDetailModal #Url").value = result.details.request.url;
    document.querySelector("#logDetailModal #Timeout").value = result.details.request.timeout;
    document.querySelector("#logDetailModal #RequestBody").innerHTML = result.details.request.body;
    document.querySelector("#logDetailModal #IsSuccess").value = result.details.response.isSuccess;
    document.querySelector("#logDetailModal #StatusCode").value = result.details.response.statusCode;
    document.querySelector("#logDetailModal #Message").value = result.details.message;
    document.querySelector("#logDetailModal #ResponseBody").innerHTML = result.details.response.body;

    hljs.highlightAll();

    logDetailModal.show();
}

function clearServiceDropdown() {
    const length = serviceDropdown.options.length - 1;
    for (let i = length; i > 0; i--) {
        serviceDropdown.remove(i);
    }
}
