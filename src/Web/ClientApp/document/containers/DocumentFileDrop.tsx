import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { actionCreators as DocumentEditorAcitons } from '../../store/ui/DocumentEditor';
import { DocumentFileDrop, DocumentFileDropDispatchProps, DocumentFileDropStateProps } from '../components';

const mapStateToProps = (state: ApplicationState): DocumentFileDropStateProps => ({});

const mapDispatchToProps = (dispatch: any): DocumentFileDropDispatchProps => ({
    onDrop: (files: FileList) => dispatch(DocumentEditorAcitons.upload(files))
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentFileDrop);