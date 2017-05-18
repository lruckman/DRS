import { getJson } from '../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from './';
import { Document } from '../models';
import queryString from 'query-string';

// -----------------
// STATE

export interface State {
    byId: { [id: number]: Document }
    , allIds: number[]
}

export interface SearchDocumentsResult {
    documents: Document[];
    nextLink: string;
}

// ----------------
// ACTIONS

@typeName("DOCUMENTS_SEARCH")
class DocumentsSearch extends Action {
    constructor() {
        super();
    }
}
@typeName("DOCUMENTS_SEARCH_FAILURE")
class DocumentsSearchFailure extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENTS_SEARCH_SUCCESS")
class DocumentsSearchSuccess extends Action {
    constructor(public normalized: SearchDocumentsResult) {
        super();
    }
}

export const actionCreators = {
    search: (keywords: string, libraryIds: number[]): TypedActionCreator<Promise<SearchDocumentsResult>> => (dispatch, getState) => {
        const qs = queryString.stringify({ q: keywords, libraryids: libraryIds.join("&libraryids=") });
        return getJson(`/api/search/?${qs}`, {})
            .then((data: SearchDocumentsResult) => dispatch(new DocumentsSearchSuccess(data)))
            .catch((error: Error) => dispatch(new DocumentsSearchFailure(error)));
    }
};

// ----------------
// REDUCER

const unloadedState: State = {
    byId: {}
    , allIds: []
}

const addDocuments = (state: State, documents: { [id: number]: Document }): State => {
    const allIds: number[] = [...state.allIds];
    const byId: { [id: number]: Document } = { ...state.byId };

    Object.keys(documents)
        .forEach((key, index) => {

            const document: Document = documents[key];

            if (!byId[document.id]) {
                allIds.push(document.id);
            }

            byId[document.id] = document;

        });

    return {
        ...state
        , byId
        , allIds
    }
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (action.normalized && action.normalized.entities && action.normalized.entities.documents) {
        return addDocuments(state, action.normalized.entities.documents);
    }

    return state || unloadedState;
};