import { getJson } from '../../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { Document } from '../../models';
import queryString from 'query-string';

// -----------------
// STATE

export interface State {
    allIds: number[];
    selectedIds: number[];
    isFetching: boolean;
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
    , select: (id: number): TypedActionCreator<Promise<any>> => (dispatch, getState) => {

    }
};

// ----------------
// REDUCER

const unloadedState: State = {
    allIds: []
    , selectedIds: []
    , isFetching: false
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (isActionType(action, DocumentsSearch)) {
        return {
            ...state
            , isFetching: true
        }
    }

    if (isActionType(action, DocumentsSearchFailure)) {
        return {
            ...state
            , isFetching: false
        }
    }

    if (isActionType(action, DocumentsSearchSuccess)) {
        return {
            ...state
            , allIds: (action as DocumentsSearchSuccess).normalized.result
            , isFetching: false
        }
    }

    return state || unloadedState;
};