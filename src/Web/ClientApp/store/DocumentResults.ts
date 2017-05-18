import { fetch, addTask } from 'domain-task';
import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface IDocumentResultsState {
    data: IGetDocumentMatches;
    selected: number[];
}

export interface IDocumentResult {
    abstract: string;
    id: number;
    selfLink: string;
    viewLink: string;
    thumbnailLink: string;
    title: string;
}

export interface IGetDocumentMatches {
    documents: IDocumentResult[];
    nextLink: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface GetDocumentMatchesAction {
    type: 'GET_DOCUMENT_MATCHES';
    keywords: string;
    libraryIds: number[];
    data: IGetDocumentMatches;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = GetDocumentMatchesAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    getDocumentMatches: (keywords: string, libraryIds: number[]): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        //if (startDateIndex !== getState().weatherForecasts.startDateIndex) {
        let fetchTask = fetch(`/api/search/?q=${keywords}&libraryids=${libraryIds.join("&libraryids=")}`)
            .then(response => response.json() as Promise<IGetDocumentMatches>)
            .then(data => {
                dispatch({ type: 'GET_DOCUMENT_MATCHES', keywords: keywords, libraryIds: libraryIds, data: data });
            });
        //}
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: IDocumentResultsState = {
    data: {
        documents: []
        , nextLink: null
    }
    , selected: []
};

export const reducer: Reducer<IDocumentResultsState> = (state, action) => {
    switch (action.type) {
        case 'GET_DOCUMENT_MATCHES':
            return {
                ...state
                , data: action.data
            };
        default:
    }

    return state || unloadedState;
};