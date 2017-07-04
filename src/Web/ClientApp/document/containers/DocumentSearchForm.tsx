import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { actionCreators as DocumentSearchStore } from '../../store/ui/DocumentSearch';
import { DocumentSearchForm, DocumentSearchFormDispatchProps, DocumentSearchFormStateProps } from '../components'

const mapStateToProps = (state: ApplicationState): DocumentSearchFormStateProps => ({
    keywords: null //todo:
    , libraryIds: [] //todo:
});

const mapDispatchToProps = (dispatch: any): DocumentSearchFormDispatchProps => ({
    onSearch: (keywords: string, libraryIds: number[]) => dispatch(DocumentSearchStore.search(keywords, libraryIds))
});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentSearchForm);