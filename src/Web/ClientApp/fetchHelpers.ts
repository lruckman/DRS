import { fetch } from 'domain-task';

const getError = response => response.statusText;

const isJsonResponse = (headers: Headers): boolean => {
    const contentType = headers.get("content-type");
    return (contentType && contentType.indexOf("application/json") !== -1);
}

const getErrorMessage = (response): Promise<string> => {
    const headers: Headers = response.headers;
    return response.text()
        .then((body: string) => {
            if (response.status === 400) {
                // BAD REQUEST
                if (isJsonResponse(headers)) {
                    // JSON RESPONSE
                    const json = body ? JSON.parse(body) : {};
                    if (json.errors) {
                        const flattenedErrors: string[] = [];
                        Object.keys(json.errors)
                            .forEach((key, index) => {
                                const fieldErrors: string[] = json.errors[key];
                                flattenedErrors.push(`${key}: ${fieldErrors.join(', ')}`);
                            });
                        return flattenedErrors.join(', ');
                    }
                }
            }

            return response.statusText;
        });
}

export const checkStatus = (response) => {
    if (response.ok) {
        return response;
    }

    return getErrorMessage(response)
        .then((message: string) => {
            throw { message: message }
        });
}

export const parseJSON = (response) => {
    if (response.status === 204) {
        return response;
    }
    return response.text()
        .then((text: string) => {
            return text ? JSON.parse(text) : {}
        })
}

const setupRequestOptions = (options: any = {}, overrideMethod) => {
    if (overrideMethod) {
        options.method = overrideMethod
    }
    if (!options.headers) {
        options.headers = {}
    }
    options.credentials = 'same-origin'
    return options
}

const setupJsonRequestOptions = (options, overrideMethod) => {
    options = setupRequestOptions(options, overrideMethod)
    options.headers['Accept'] = 'application/json'
    if (options.body) {
        options.headers['Content-Type'] = 'application/json'
        if (typeof options.body !== 'string' || !/^".*"$/g.test(options.body)) {
            options.body = JSON.stringify(options.body)
        }
    }
    return options
}

const fetchJsonWithResponse = (url, options, overrideMethod) => {
    options = setupJsonRequestOptions(options, overrideMethod)
    return fetch(url, options)
}

const fetchJson = (url, options, overrideMethod) => {
    return fetchJsonWithResponse(url, options, overrideMethod)
        .then(checkStatus)
        .then(parseJSON)
}

export const fetchRaw = (url, options, overrideMethod) => {
    options = setupRequestOptions(options, overrideMethod)
    return fetch(url, options)
        .then(checkStatus);
}

export const getJson = (url, options) => fetchJson(url, options, 'GET')
export const postJson = (url, options) => fetchJson(url, options, 'POST')
export const putJson = (url, options) => fetchJson(url, options, 'PUT')
export const deleteJson = (url, options) => fetchJson(url, options, 'DELETE')

export const getRaw = (url, options) => fetchRaw(url, options, 'GET')
export const postRaw = (url, options) => fetchRaw(url, options, 'POST')
export const putRaw = (url, options) => fetchRaw(url, options, 'PUT')
export const deleteRaw = (url, options) => fetchRaw(url, options, 'DELETE')