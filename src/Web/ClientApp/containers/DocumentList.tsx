import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { actionCreators as DocumentSearchActions } from '../store/ui/DocumentSearch';
import { DocumentList, DocumentListDispatchProps, DocumentListStateProps } from '../components'

const mapStateToProps = (state: ApplicationState): DocumentListStateProps => ({
    keywords: '' //state.search.keywords
    , libraryIds: [] // state.search.libraries.selectedIds
    , libraryOptions: [] // state.search.libraries.allIds.map(id => state.search.libraries.byId[id])
    , isFetching: state.ui.documentSearch.isFetching
    , documents: state.ui.documentSearch.allIds.map(id => state.entities.documents.byId[id])
    , selected: state.ui.documentSearch.selectedIds
    , nextPage: ''
});

const mapDispatchToProps = (dispatch: any): DocumentListDispatchProps => ({
    onSelect: DocumentSearchActions.select
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentList);