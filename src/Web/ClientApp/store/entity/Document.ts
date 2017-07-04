import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { DocumentFile, NormalizedDocuments } from '../../models';
import { getEntity, normalize } from '../../utilities';
import { postJson, putJson } from '../../fetchHelpers';

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

@typeName("DOCUMENT_UPLOAD_REQUESTED")
class DocumentUploadRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("DOCUMENT_UPLOAD_FAILED")
class DocumentUploadFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DOCUMENT_UPLOAD_SUCCESS")
class DocumentUploadSuccess extends Action {
    constructor(public normalized: NormalizedDocuments) {
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
    , upload: (file: File): TypedActionCreator<Promise<number[]>> => (dispatch, getState) => {
        dispatch(new DocumentUploadRequested());
        return postJson('/api/documents', { file })
            .then((data: DocumentFile) => {
                const normalized = normalize.documents([data]);
                dispatch(new DocumentUploadSuccess(normalized));
                return normalized.result;
            })
            .catch((error: Error) => dispatch(new DocumentUploadFailed(error)));
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

    return state || unloadedState;
};