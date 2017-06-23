import { combineReducers } from 'redux';

import * as DocumentStore from './Document';

export interface State {
    documents: DocumentStore.State
}

export const reducer = combineReducers({
    documents: DocumentStore.reducer
});