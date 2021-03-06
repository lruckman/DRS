﻿import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { actionCreators as DocumentStore } from '../../store/entity/Document';
import { DocumentSearchForm, DocumentSearchFormDispatchProps, DocumentSearchFormStateProps } from '../components'

const mapStateToProps = (state: ApplicationState): DocumentSearchFormStateProps => ({
    keywords: state.ui.documentSearch.keywords
    , libraryIds: state.ui.documentSearch.libraryIds
});

const mapDispatchToProps = (dispatch: any): DocumentSearchFormDispatchProps => ({
    onSearch: (keywords: string, libraryIds: number[]) => dispatch(DocumentStore.search(keywords, libraryIds))
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentSearchForm);