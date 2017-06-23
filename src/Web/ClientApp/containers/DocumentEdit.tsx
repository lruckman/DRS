import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { DocumentEdit, DocumentEditStateProps, DocumentEditDispatchProps } from '../components'

type OwnProps = {}

const mapStateToProps = (state: ApplicationState, props: OwnProps) : DocumentEditStateProps => ({
    libraries: ILibraryListItem[];
    location: string;
});

const mapDispatchToProps = (dispatch: any, props: OwnProps) : DocumentEditDispatchProps => ({
    onUpdate:;
    onClose:;
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentEdit);