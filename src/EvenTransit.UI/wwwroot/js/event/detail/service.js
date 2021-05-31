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
    
    for(let i = 0; i < headerItems.length; i++)
    {
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
    console.log(result);
});

document.querySelector("#add-header").addEventListener("click", e=>{
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
    keyCell.appendChild(keyInput);
    
    let valueInput = document.createElement("input");
    valueInput.setAttribute('type', 'text');
    valueInput.setAttribute('class', 'form-control form-control-sm header-value');
    valueCell.appendChild(valueInput);
});

function removeHeaderItem(e){
    const rowIndex = e.target.parentElement.parentElement.parentElement.rowIndex - 1;
    document.querySelector('#headers tbody').deleteRow(rowIndex);
}