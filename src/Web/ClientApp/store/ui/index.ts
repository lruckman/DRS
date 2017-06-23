import { combineReducers } from 'redux';

import * as DocumentSearchStore from './DocumentSearch';

export interface State {
    documentSearch: DocumentSearchStore.State
}

export const reducer = combineReducers({
    documentSearch: DocumentSearchStore.reducer
});