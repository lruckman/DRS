import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { DocumentFile, DocumentSearchResults, NormalizedDocuments } from '../../models';
import { getEntity, normalize } from '../../utilities';
import { putJson, getJson, postRaw, deleteJson } from '../../fetchHelpers';
import queryString from 'query-string';

// -----------------
// STATE

type ByIdRecordset = { [id: number]: DocumentFile }

export interface State {
    byId: ByIdRecordset
    , allIds: number[]
}

// ----------------
// ACTIONS

@typeName("DOCUMENT_SAVE_REQUESTED")
class DocumentSaveRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("DOCUMENT_SAVE_FAILED")
class DocumentSaveFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENT_SAVE_SUCCESS")
class DocumentSaveSuccess extends Action {
    constructor(public normalized: NormalizedDocuments) {
        super();
    }
}

@typeName("DOCUMENT_SEARCH_REQUESTED")
export class DocumentSearchRequested extends Action {
    constructor(public keywords: string, public libraryIds: number[]) {
        super();
    }
}
@typeName("DOCUMENT_SEARCH_FAILED")
export class DocumentSearchFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENT_SEARCH_SUCCESS")
export class DocumentSearchSuccess extends Action {
    constructor(public normalized: NormalizedDocuments, public resultCount: number, public nextLink: string) {
        super();
    }
}

@typeName("DOCUMENT_CREATE_REQUESTED")
class DocumentCreateRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("DOCUMENT_CREATE_FAILED")
class DocumentCreateFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENT_CREATE_SUCCESS")
class DocumentCreateSuccess extends Action {
    constructor(public normalized: NormalizedDocuments) {
        super();
    }
}

@typeName("DOCUMENT_DELETE_REQUESTED")
class DocumentDeleteRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("DOCUMENT_DELETE_FAILED")
class DocumentDeleteFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENT_DELETE_SUCCESS")
export class DocumentDeleteSuccess extends Action {
    constructor(public id: number) {
        super();
    }
}

export const actionCreators = {
    save: (document: DocumentFile): TypedActionCreator<Promise<number>> => (dispatch, getState) => {
        dispatch(new DocumentSaveRequested());

        return putJson(`/api/documents/${document.id}`, { body: document })
            .then((data: DocumentFile) => {
                const normalized = normalize.documents([data]);
                dispatch(new DocumentSaveSuccess(normalized));
                return normalized.result[0];
            })
            .catch((error: Error) => dispatch(new DocumentSaveFailed(error)));
    }
    , search: (keywords: string, libraryIds: number[]): TypedActionCreator<Promise<number[]>> => (dispatch, getState) => {
        dispatch(new DocumentSearchRequested(keywords, libraryIds));
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
            .catch((error: Error) => dispatch(new DocumentSearchFailed(error)));
    }
    , create: (file: File): TypedActionCreator<Promise<number>> => (dispatch, getState) => {

        dispatch(new DocumentCreateRequested());

        const options = {
            body: new FormData()
        }

        options.body.append('file', file);

        return postRaw('/api/documents', options)
            .then(response => response.json())
            .then((data: DocumentFile) => {
                const normalized = normalize.documents([data]);
                dispatch(new DocumentCreateSuccess(normalized));
                return normalized.result[0];
            })
            .catch((error: Error) => dispatch(new DocumentCreateFailed(error)));
    }
    , delete: (id: number): TypedActionCreator<Promise<number>> => (dispatch, getState) => {

        dispatch(new DocumentDeleteRequested());

        return deleteJson(`/api/documents/${id}`, {})
            .then(() => dispatch(new DocumentDeleteSuccess(id)))
            .then(() => id)
            .catch((error: Error) => dispatch(new DocumentDeleteFailed(error)));
    }
}

// ----------------
// REDUCER

const unloadedState: State = {
    byId: {}
    , allIds: []
}

const addEntities = (state: State, documents: ByIdRecordset): State => {
    const allIds = [...state.allIds];
    const byId = { ...state.byId };

    Object.keys(documents)
        .forEach((key, index) => {

            const document: DocumentFile = documents[key];

            if (!byId[document.id]) {
                allIds.push(document.id);
            }

            byId[document.id] = document;

        });

    return {
        byId
        , allIds
    }
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (getEntity<ByIdRecordset>(action, 'documents')) {
        return addEntities(state, getEntity<ByIdRecordset>(action, 'documents'));
    }

    if (isActionType(action, DocumentDeleteSuccess)) {
        let omit;
        return {
            ...state
            , byId: {
                ...state.byId
                , [action.id]: omit
            }
            , allIds: state.allIds.filter(id => id != action.id)
        }
    }

    return state || unloadedState;
};