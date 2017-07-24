import { combineReducers } from 'redux';

import * as DocumentEditorStore from './DocumentEditor';
import * as DocumentSearchStore from './DocumentSearch';
import * as ModalStore from './Modal';

export interface State {
    documentEditor: DocumentEditorStore.State
    , documentSearch: DocumentSearchStore.State
    , modal: ModalStore.State
}

export const reducer = combineReducers({
    documentEditor: DocumentEditorStore.reducer
    , documentSearch: DocumentSearchStore.reducer
    , modal: ModalStore.reducer
});