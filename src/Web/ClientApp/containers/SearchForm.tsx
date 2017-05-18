import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import * as SearchFormStore from '../../store/SearchForm';
import SearchForm from '../../components/SearchForm'

export default connect(
    (state: ApplicationState) => ({
        error: state.search.error
        , isFetching: state.search.isFetching
        , keywords: state.search.keywords
        , libraryIds: state.search.libraries.selectedIds
        , libraryOptions: state.search.libraries.allIds.map(id => state.search.libraries.byId[id])
    })
    , { onSearch: SearchFormStore.actionCreators.search }
)(SearchForm);