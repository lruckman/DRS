import { ActionCreatorGeneric } from 'redux-typed';

import * as DocumentResults from './DocumentResults';
import * as SearchFormStore from './SearchForm';

// The top-level state object
export interface ApplicationState {
    //documentResults: DocumentResults.IDocumentResultsState;
    search: SearchFormStore.State;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    //documentResults: DocumentResults.reducer,
    search: SearchFormStore.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}

export type ActionCreator = ActionCreatorGeneric<ApplicationState>;