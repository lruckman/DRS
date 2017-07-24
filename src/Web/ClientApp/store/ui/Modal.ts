﻿import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';

// -----------------
// STATE

export interface State {
    abortText: string
    , confirmText: string
    , onConfirm: (isConfirmed: boolean) => void
    , title: string
    , children: any
    , destructive: boolean
    , warning: boolean
    , show: boolean
}

// -----------------
// ACTIONS

@typeName("HIDING_CONFIRMATION")
export class HidingConfirmation extends Action {
    constructor() {
        super();
    }
}
@typeName("SHOWING_CONFIRMATION")
export class ShowingConfirmation extends Action {
    constructor(public payload: State) {
        super();
    }
}

// ----------------
// ACTION CREATORS

export const actionCreators = {
    confirmDelete: (handleConfirmed: (isConfirmed: boolean) => void, message: string)
        : TypedActionCreator<void> => (dispatch, getState) => {
            var payload = {
                abortText: 'No, cancel'
                , confirmText: 'Yes, delete this record'
                , onConfirm: handleConfirmed
                , title: 'Confirm Delete'
                , children: message
                , destructive: true
                , warning: false
                , show: true
            };
            dispatch(new ShowingConfirmation(payload));
        }
    , confirmWarning: (handleConfirmed: (isConfirmed: boolean) => void, title: string, message: string, abortText: string, confirmText: string)
        : TypedActionCreator<void> => (dispatch, getState) => {
            var payload = {
                abortText: abortText
                , confirmText: confirmText
                , onConfirm: handleConfirmed
                , title
                , children: message
                , destructive: false
                , warning: true
                , show: true
            }
            dispatch(new ShowingConfirmation(payload));
        }
    , confirm: (handleConfirmed: (isConfirmed: boolean) => void, title: string, message: string, abortText?: string, confirmText?: string)
        : TypedActionCreator<void> => (dispatch, getState) => {
            var payload = {
                abortText: abortText || 'No, cancel'
                , confirmText: confirmText || 'OK'
                , onConfirm: handleConfirmed
                , title
                , children: message
                , destructive: false
                , warning: false
                , show: true
            }
            dispatch(new ShowingConfirmation(payload));
        }
    , hideConfirmation: ()
        : TypedActionCreator<void> => (dispatch, getState) => {
            dispatch(new HidingConfirmation());
        }
};

// ----------------
// REDUCER

const unloadedState: State = {
    abortText: 'Cancel'
    , confirmText: 'OK'
    , onConfirm: (isConfirmed) => { }
    , title: null
    , children: null
    , destructive: false
    , warning: false
    , show: false
};

export const reducer: Reducer<State> = (state, action: any) => {

    if (isActionType(action, HidingConfirmation)) {
        return { ...state, show: false };
    }
    if (isActionType(action, ShowingConfirmation)) {
        return action.payload;
    }

    return state || unloadedState;
};