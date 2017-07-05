﻿import { getJson } from '../../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { normalize } from '../../utilities';
import { actionCreators as DocumentActions } from '../entity/Document';
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

@typeName("CANCEL_DOCUMENT_EDIT")
class CancelDocumentEdit extends Action {
    constructor(public id: number) {
        super();
    }
}

export const actionCreators = {
    upload: (files: File[]): TypedActionCreator<Promise<number[]>> => (dispatch, getState) => {
        dispatch(new FileUploadRequested());

        const promises: Promise<number>[] = files
            .map(file => dispatch(DocumentActions.upload(file)));

        return Promise.all(promises)
            .then(ids => {
                dispatch(new FileUploadSuccess(ids));
                return ids;
            });
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

    if (isActionType(action, FileUploadSuccess)) {
        return {
            ids: action.ids
            , editCount: action.ids.length
        }
    }

    if (isActionType(action, CancelDocumentEdit)) {
        return {
            ...state
            , ids: state.ids.filter(id => id != action.id)
        }
    }

    return state || unloadedState;
};