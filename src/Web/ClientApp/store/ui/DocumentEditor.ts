import { getJson } from '../../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { normalize } from '../../utilities';
import { actionCreators as DocumentActions } from '../entity/Document';
import { actionCreators as ModalActions } from '../ui/Modal';
import { DocumentFile } from '../../models';

// -----------------
// STATE

export interface State {
    ids: number[]
    , editCount: number
}

// ----------------
// ACTIONS

@typeName("FILE_UPLOAD_REQUESTED")
class FileUploadRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("FILE_UPLOAD_FAILED")
class FileUploadFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("FILE_UPLOAD_SUCCESS")
class FileUploadSuccess extends Action {
    constructor(public ids: number[]) {
        super();
    }
}

@typeName("START_DOCUMENT_EDIT")
class StartDocumentEdit extends Action {
    constructor(public ids: number[]) {
        super();
    }
}

@typeName("CANCEL_DOCUMENT_EDIT")
class CancelDocumentEdit extends Action {
    constructor(public id: number) {
        super();
    }
}

export const actionCreators = {
    delete: (id: number): TypedActionCreator<void> => (dispatch, getState) => {
        const handleConfirmed = (isConfirmed: boolean) => {
            if (isConfirmed) {
                dispatch(DocumentActions.delete(id));
                return;
            }
        }

        const confirmDelete = ModalActions.confirmDelete(
            handleConfirmed
            , `Are you sure you want to delete the document titled '${getState().entities.documents.byId[id].title}'?`
        );

        dispatch(confirmDelete);
    }
    , edit: (ids: number | number[]): TypedActionCreator<void> => (dispatch, getState) => {

        if (ids instanceof Array) {
            dispatch(new StartDocumentEdit(ids));
            return;
        }

        dispatch(new StartDocumentEdit([ids]));
    }
    , upload: (files: FileList): TypedActionCreator<Promise<number[]>> => (dispatch, getState) => {
        dispatch(new FileUploadRequested());

        const promises: Promise<number>[] = Array.from(files)
            .map(file => dispatch(DocumentActions.create(file)));

        return Promise.all(promises)
            .then(ids => {
                if (ids) {
                    const newIds = ids.filter(id => id);
                    if (newIds.length !== 0) {
                        dispatch(new FileUploadSuccess(newIds));
                        dispatch(actionCreators.edit(newIds));
                        return newIds;
                    }
                }
            })
            .catch((error: Error) => alert(error.message));
    }
    , save: (document: DocumentFile): TypedActionCreator<Promise<number>> => (dispatch, getState) => {
        return dispatch(DocumentActions.save(document))
            .then(id => {
                if (id) {
                    return dispatch(actionCreators.cancel(id));
                }
                return null;
            });
    }
    , cancel: (id?: number): TypedActionCreator<number> => (dispatch, getState) => {
        const ids = getState().ui.documentEditor.ids;

        if (ids.length === 0) {
            return null;
        }

        if (!id) {
            id = ids[0];
        }

        dispatch(new CancelDocumentEdit(id));
        return id;
    }
};

// ----------------
// REDUCER

const unloadedState: State = {
    ids: []
    , editCount: 0
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (isActionType(action, CancelDocumentEdit)) {
        return {
            ...state
            , ids: state.ids.filter(id => id != action.id)
        }
    }

    if (isActionType(action, StartDocumentEdit)) {
        return {
            ids: action.ids
            , editCount: action.ids.length
        }
    }

    return state || unloadedState;
};