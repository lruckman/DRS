import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { Document } from '../../models';
import { getEntity } from '../../utilities';

// -----------------
// STATE

type ByIdRecordset = { [id: number]: Document }

export interface State {
    byId: ByIdRecordset
    , allIds: number[]
}

// ----------------
// ACTIONS


export const actionCreators = {}

// ----------------
// REDUCER

const unloadedState: State = {
    byId: {}
    , allIds: []
}

const addDocuments = (state: State, documents: ByIdRecordset): State => {
    const allIds = [...state.allIds];
    const byId = { ...state.byId };

    Object.keys(documents)
        .forEach((key, index) => {

            const document: Document = documents[key];

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
        return addDocuments(state, getEntity<ByIdRecordset>(action, 'documents'));
    }

    return state || unloadedState;
};