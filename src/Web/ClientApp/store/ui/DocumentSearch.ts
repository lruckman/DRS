import { getJson } from '../../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { DocumentSearchResults, NormalizedDocuments } from '../../models';
import { normalize } from '../../utilities';
import { DocumentSearchFailed, DocumentSearchRequested, DocumentSearchSuccess } from '../entity/Document';

// -----------------
// STATE

export interface State {
    allIds: number[]
    , selectedIds: number[]
    , keywords: string
    , libraryIds: number[]
    , isSearching: boolean
    , matchCount: number
}

// ----------------
// ACTIONS

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
    selectDocument: (id: number): TypedActionCreator<void> => (dispatch, getState) => {
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
    , keywords: ''
    , libraryIds: []
    , selectedIds: []
    , isSearching: false
    , matchCount: 0
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (isActionType(action, DocumentSearchRequested)) {
        return {
            ...state
            , isSearching: true
            , keywords: action.keywords
            , libraryIds: action.libraryIds
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