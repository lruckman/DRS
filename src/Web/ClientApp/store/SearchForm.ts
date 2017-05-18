import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { AppThunkAction, ActionCreator } from './';
import { Document, Library, DistributionGroup } from '../models';
import { getJson } from '../fetchHelpers';

// -----------------
// STATE

export interface State {
    keywords: string
    , error: Error
    , isFetching: boolean
    , documents: {
        byId: { [id: number]: Document }
        , allIds: number[]
        , selectedIds: number[]
    }
    , libraries: {
        byId: { [id: number]: Library }
        , allIds: number[]
        , selectedIds: number[]
    }
}

interface DocumentList {
    documents: Document[];
    nextLink: string;
}

// -----------------
// ACTIONS

@typeName("FETCHING_LIBRARIES")
class FetchingLibraries extends Action {
    constructor() {
        super();
    }
}
@typeName("FETCHING_LIBRARIES_FAILURE")
class FetchingLibrariesFailure extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("FETCHING_LIBRARIES_SUCCESS")
class FetchingLibrariesSuccess extends Action {
    constructor(public data: DistributionGroup[]) {
        super();
    }
}

@typeName("SEARCHING")
class Searching extends Action {
    constructor(public keywords: string, public libraryIds: number[]) {
        super();
    }
}
@typeName("SEARCHING_FAILURE")
class SearchingFailure extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("SEARCHING_SUCCESS")
class SearchingSuccess extends Action {
    constructor(public data: DocumentList) {
        super();
    }
}

@typeName("SELECTING_DOCUMENT")
class SelectingDocument extends Action {
    constructor() {
        super();
    }
}
@typeName("SELECTING_DOCUMENT_FAILURE")
class SelectingDocumentFailure extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("SELECTING_DOCUMENT_SUCCESS")
class SelectingDocumentSuccess extends Action {
    constructor(public id: number) {
        super();
    }
}

// ----------------
// ACTION CREATORS

export const actionCreators = {
    getLibraries: ()
        : ActionCreator => (dispatch, getState) => {

            dispatch(new FetchingLibraries());

            return getJson(`/api/profile/distributiongroups/`, {})
                .then((data: DistributionGroup[]) => dispatch(new FetchingLibrariesSuccess(data)))
                .catch((error: Error) => dispatch(new FetchingLibrariesFailure(error)));

        }
    , search: (keywords: string, libraryIds: number[])
        : ActionCreator => (dispatch, getState) => {

            dispatch(new Searching(keywords, libraryIds));

            return getJson(`/api/search/?q=${keywords}&libraryids=${libraryIds.join('&libraryids=')}`, {})
                .then((data: DocumentList) => dispatch(new SearchingSuccess(data)))
                .catch((error: Error) => dispatch(new SearchingFailure(error)));
        }
    , selectDocument: (id: number)
        : ActionCreator => (dispatch, getState) => {

            dispatch(new SelectingDocument());
            dispatch(new SelectingDocumentSuccess(id));

        }
};

// ----------------
// REDUCER

const unloadedState: State = {
    keywords: null
    , error: null
    , isFetching: false
    , documents: {
        byId: {}
        , allIds: []
        , selectedIds: []
    }
    , libraries: {
        byId: {}
        , allIds: []
        , selectedIds: []
    }
};

export const reducer: Reducer<State> = (state, action) => {
    if (isActionType(action, FetchingLibraries)) {
        return {
            ...state
            , isFetching: true
        };
    }
    if (isActionType(action, FetchingLibrariesFailure)) {
        return {
            ...state
            , isFetching: false
            , error: (action as FetchingLibrariesFailure).error
        };
    }
    if (isActionType(action, FetchingLibrariesSuccess)) {

        let allIds: number[] = [];

        let byId = (action as FetchingLibrariesSuccess).data
            .reduce((p, c) => {

                let id = +c.id;

                allIds.push(id);

                p[id] = c;

                return p;
            }, {});

        return {
            ...state
            , isFetching: false
            , error: null
            , libraries: {
                byId
                , allIds
                , selectedIds: []
            }
        }
    }

    if (isActionType(action, Searching)) {
        let data = (action as Searching);
        return {
            ...state
            , keywords: data.keywords
            , libraries: {
                ...state.libraries
                , selectedIds: data.libraryIds
            }
            , isFetching: true
            , error: null
        };
    }
    if (isActionType(action, SearchingFailure)) {
        return {
            ...state
            , isFetching: false
            , error: (action as SearchingFailure).error
        };
    }
    if (isActionType(action, SearchingSuccess)) {
        let data = (action as SearchingSuccess).data;

        let allIds: number[] = [];

        let byId = data.documents
            .reduce((p, c) => {

                allIds.push(c.id);

                p[c.id] = c;

                return p;
            }, {});

        return {
            ...state
            , error: null
            , isFetching: false
            , documents: {
                byId
                , allIds
                , selectedIds: []
            }
        }
    }

    if (isActionType(action, SelectingDocument)) {
        let data = (action as SelectingDocument);
        return {
            ...state
            , isFetching: true
            , error: null
        };
    }
    if (isActionType(action, SelectingDocumentFailure)) {
        return {
            ...state
            , isFetching: false
            , error: (action as SelectingDocumentFailure).error
        };
    }
    if (isActionType(action, SelectingDocumentSuccess)) {
        let id = (action as SelectingDocumentSuccess).id;

        return {
            ...state
            , error: null
            , isFetching: false
            , documents: {
                ...state.documents
                , selectedIds: [ id ]
            }
        }
    }

    return state || unloadedState;
};