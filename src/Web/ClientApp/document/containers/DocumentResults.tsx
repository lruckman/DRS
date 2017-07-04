import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { actionCreators as DocumentSearchActions } from '../../store/ui/DocumentSearch';
import { DocumentResults, DocumentResultsDispatchProps, DocumentResultsStateProps } from '../components';

const mapStateToProps = (state: ApplicationState): DocumentResultsStateProps => ({
    keywords: '' //state.search.keywords
    , libraryIds: [] // state.search.libraries.selectedIds
    , libraryOptions: [] // state.search.libraries.allIds.map(id => state.search.libraries.byId[id])
    , isFetching: state.ui.documentSearch.isSearching
    , documents: state.ui.documentSearch.allIds.map(id => state.entities.documents.byId[id])
    , selected: state.ui.documentSearch.selectedIds
    , nextPage: ''
});

const mapDispatchToProps = (dispatch: any): DocumentResultsDispatchProps => ({
    onSelect: (id: number) => dispatch(DocumentSearchActions.selectDocument(id))
    , onOpen: (id: number) => window.open(id.toString(), '_blank')
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentResults);