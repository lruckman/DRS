import { typeName, isActionType, Action, Reducer } from 'redux-typed';

// -----------------
// STATE

export interface State {
    error: Error
}

@typeName("RESET_ERROR_MESSAGE")
export class ResetErrorMessage extends Action {
    constructor() {
        super();
    }
}

// ----------------
// ACTION CREATORS

export const actionCreators = {};

const unloadedState: State = {
    error: null
};


export const reducer: Reducer<State> = (state, action: any) => {

    if (isActionType(action, ResetErrorMessage)) {
        return {
            ...state
            , error: null
        }
    }

    if (action.error) {
        return {
            ...state
            , error: action.error
        }
    }

    return state || unloadedState;
};