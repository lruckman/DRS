import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { actionCreators as DocumentEditorActions } from '../../store/ui/DocumentEditor';
import { DocumentEditor, DocumentEditorDispatchProps, DocumentEditorStateProps } from '../components';
import { DocumentFile } from '../../models';

const mapStateToProps = (state: ApplicationState): DocumentEditorStateProps => {
    const show = state.ui.documentEditor.ids.length !== 0;
    return {
        editCount: state.ui.documentEditor.editCount
        , document: show
            ? state.entities.documents.byId[state.ui.documentEditor.ids[0]]
            : null
        , show
    }
}

const mapDispatchToProps = (dispatch: any): DocumentEditorDispatchProps => ({
    onHide: () => dispatch(DocumentEditorActions.cancel())
    , onSave: (values: DocumentFile) => dispatch(DocumentEditorActions.save(values))
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentEditor);