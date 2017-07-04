import { combineReducers } from 'redux';

import * as DocumentStore from './Document';
import * as LibraryStore from './Library';

export interface State {
    documents: DocumentStore.State
    , libraries: LibraryStore.State
}

export const reducer = combineReducers({
    documents: DocumentStore.reducer
    , libraries: LibraryStore.reducer
});