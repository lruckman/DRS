import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as SearchFormStore from '../store/SearchForm';
import App from '../App'

export default connect(
    (state: ApplicationState) => ({
        documentsSelected: state.search.documents.selectedIds.length !== 0
    })
    , {}
)(App);