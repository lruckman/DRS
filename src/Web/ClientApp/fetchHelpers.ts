import { fetch } from 'domain-task';

const getError = response => response.statusText;

export const checkStatus = (response) => {
    if (!response.ok) {
        throw Error(getError(response))
    }
    return response;
}

export const parseJSON = (response) => response.json();

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