import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { Search } from '../components';

const mapStateToProps = (state: ApplicationState) => ({
    documentsSelected: state.ui.documentSearch.selectedIds.length !== 0
});

const mapDispatchToProps = (dispatch: any) => ({});

export default connect(mapStateToProps, mapDispatchToProps)(Search);