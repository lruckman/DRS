﻿import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { ConfirmationModal, ConfirmationModalDispatchProps, ConfirmationModalStateProps } from '../components';
import { actionCreators as ModalActions } from '../store/ui/Modal';

const mapStateToProps = (state: ApplicationState): ConfirmationModalStateProps => ({
    ...state.ui.modal
});

const mapDispatchToProps = (): ConfirmationModalDispatchProps => ({
    onHide: ModalActions.hideConfirmation
});

export default connect(mapStateToProps, mapDispatchToProps)(ConfirmationModal)