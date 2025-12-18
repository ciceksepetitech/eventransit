
async function copyAsCurl() {
    if (!currentLogResult || !currentLogResult.details || !currentLogResult.details.request) {
        toastr.warning("No log details available to copy!");
        return;
    }

    const request = currentLogResult.details.request;
    const method = (request.method || 'GET').toUpperCase();
    const url = request.url;
    const body = request.body;
    const headers = request.headers;

    let curlCommand = `curl -X ${method} "${url}"`;

    let hasContentType = false;
    if (headers) {
        for (const [key, value] of Object.entries(headers)) {
            if (key.toLowerCase() !== 'content-length') {
                curlCommand += ` -H "${key}: ${value}"`;
                if (key.toLowerCase() === 'content-type') {
                    hasContentType = true;
                }
            }
        }
    }

    if (body) {
        if (!hasContentType) {
            curlCommand += ` -H "Content-Type: application/json"`;
        }

        // Handle body for all requests that have it
        // If it's a JSON string, try to keep it raw if possible or just escape it correctly
        let content = body;
        if (typeof body === 'object') {
            content = JSON.stringify(body);
        }

        // Escape single quotes for shell safety
        const escapedBody = content.replace(/'/g, "'\\''");

        // Use --data-raw to prevent @ interpretation if it starts with @, though mostly relevant for file uploads. 
        // using -d or --data-raw with single quotes is standard.
        // The user specifically asked for "raw json".
        curlCommand += ` --data-raw '${escapedBody}'`;
    }

    try {
        await copyToClipboard(curlCommand);
        toastr.success('cURL command copied to clipboard!');
    } catch (err) {
        toastr.warning('Unable to copy cURL command!');
        console.error(err);
    }
}
