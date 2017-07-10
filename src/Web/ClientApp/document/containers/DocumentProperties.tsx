import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { actionCreators as DocumentSearchActions } from '../../store/ui/DocumentSearch';
import { DocumentProperties, DocumentPropertiesDispatchProps, DocumentPropertiesStateProps } from '../components';

const mapStateToProps = (state: ApplicationState): DocumentPropertiesStateProps => ({
    document: state.ui.documentSearch.selectedIds.length === 1
        ? state.entities.documents.byId[state.ui.documentSearch.selectedIds[0]]
        : null
});

const mapDispatchToProps = (dispatch: any): DocumentPropertiesDispatchProps => ({});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentProperties);