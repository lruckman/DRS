import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as SearchFormStore from '../store/SearchForm';
import DocumentList from '../components/DocumentList'

export default connect(
    (state: ApplicationState) => ({
        keywords: state.search.keywords
        , libraryIds: state.search.libraries.selectedIds
        , libraryOptions: state.search.libraries.allIds.map(id => state.search.libraries.byId[id])
        , error: state.search.error
        , isFetching: state.search.isFetching
        , documents: state.search.documents.allIds.map(id=>state.search.documents.byId[id])
        , selected: state.search.documents.selectedIds
        , nextPage: ''
    })
    , {
        onSelect: SearchFormStore.actionCreators.selectDocument
    }
)(DocumentList);