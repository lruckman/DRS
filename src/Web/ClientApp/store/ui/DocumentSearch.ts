import { getJson } from '../../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { DocumentSearchResults, NormalizedDocuments } from '../../models';
import queryString from 'query-string';
import { normalize } from '../../utilities';

// -----------------
// STATE

export interface State {
    allIds: number[];
    selectedIds: number[];
    isSearching: boolean;
}

// ----------------
// ACTIONS

@typeName("DOCUMENT_SEARCH_REQUESTED")
class DocumentSearchRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("DOCUMENT_SEARCH_FAILED")
class DocumentSearchFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENT_SEARCH_SUCCESS")
class DocumentSearchSuccess extends Action {
    constructor(public normalized: NormalizedDocuments, public resultCount: number, public nextLink: string) {
        super();
    }
}

@typeName("SELECT_DOCUMENT")
class SelectDocument extends Action {
    constructor(public ids: number[]) {
        super();
    }
}

@typeName("DESELECT_DOCUMENT")
class DeselectDocument extends Action {
    constructor(public ids: number[]) {
        super();
    }
}

export const actionCreators = {
    search: (keywords: string, libraryIds: number[]): TypedActionCreator<Promise<number[]>> => (dispatch, getState) => {
        const qs = queryString.stringify({
            keywords
            , libraryids: !libraryIds || libraryIds.length == 0
                ? undefined
                : libraryIds.join("&libraryids=")
        });
        return getJson(`/api/documents/?${qs}`, {})
            .then((data: DocumentSearchResults) => {
                const normalized = normalize.documents(data.documents);
                dispatch(new DocumentSearchSuccess(normalized, data.totalCount, data.nextLink));
                return normalized.result
            })
            .catch((error: Error) => {
                dispatch(new DocumentSearchFailed(error));
                return undefined;
            });
    }
    , selectDocument: (id: number): TypedActionCreator<void> => (dispatch, getState) => {
        dispatch(new SelectDocument([id]));
    }
    , selectAllDocuments: (): TypedActionCreator<void> => (dispatch, getState) => {
        dispatch(new SelectDocument(getState().ui.documentSearch.allIds));
    }
    , deselectDocument: (id: number): TypedActionCreator<void> => (dispatch, getState) => {
        dispatch(new DeselectDocument([id]));
    }
    , deselectAllDocument: (): TypedActionCreator<void> => (dispatch, getState) => {
        dispatch(new DeselectDocument(getState().ui.documentSearch.selectedIds));
    }
};

// ----------------
// REDUCER

const unloadedState: State = {
    allIds: []
    , selectedIds: []
    , isSearching: false
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (isActionType(action, DocumentSearchRequested)) {
        return {
            ...state
            , isSearching: true
        }
    }

    if (isActionType(action, DocumentSearchFailed)) {
        return {
            ...state
            , isSearching: false
        }
    }

    if (isActionType(action, DocumentSearchSuccess)) {
        return {
            ...state
            , allIds: action.normalized.result
            , isSearching: false
        }
    }

    if (isActionType(action, SelectDocument)) {
        return {
            ...state
            , selectedIds: [
                ...action.ids
                , ...state.selectedIds.filter(existing => !action.ids.find(added => existing != added))
            ]
        }
    }

    if (isActionType(action, DeselectDocument)) {
        return {
            ...state
            , selectedIds: state.selectedIds.filter(selected => !action.ids.find(deselected => selected != deselected))
        }
    }

    return state || unloadedState;
};