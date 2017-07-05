import { combineReducers } from 'redux';

import * as DocumentEditorStore from './DocumentEditor';
import * as DocumentSearchStore from './DocumentSearch';

export interface State {
    documentEditor: DocumentEditorStore.State
    , documentSearch: DocumentSearchStore.State
}

export const reducer = combineReducers({
    documentEditor: DocumentEditorStore.reducer
    , documentSearch: DocumentSearchStore.reducer
});