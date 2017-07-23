﻿import * as React from 'react';
import { Button, Modal } from 'react-bootstrap';
import BaseModal from './BaseModal';

export type ConfirmationModalStateProps = {
    abortText: string
    , confirmText: string
    , title: string
    , children: any
    , destructive: boolean
    , warning: boolean
    , show: boolean
    , onConfirm: (isConfirmed: boolean) => void
}
export type ConfirmationModalDispatchProps = {
    onHide: () => void
}

type OwnProps = ConfirmationModalDispatchProps & ConfirmationModalStateProps;

class ConfirmationModal extends React.Component<OwnProps, null> {
    constructor(props: OwnProps) {
        super(props);

        this.handleConfirm = this.handleConfirm.bind(this);
    }

    handleConfirm(isConfirmed: boolean) {
        this.props.onHide()
        this.props.onConfirm(isConfirmed);
    }

    render() {
        const buttons = [
            <Button key="cancelButton" onClick={this.handleConfirm(false)}>{this.props.abortText || "Cancel"}</Button>
            , <Button key="confirmButton" bsStyle={this.props.destructive ? "danger" : this.props.warning ? "warning" : "primary"} onClick={this.handleConfirm(true)}>{this.props.confirmText || "OK"}</Button>
        ]

        const modalProps = {
            buttons
            , onHide: () => this.handleConfirm(false)
            , show: this.props.show
            , title: this.props.title
            , className: 'alert'
            , children: <span dangerouslySetInnerHTML={{ __html: this.props.children }} />
        }

        return <BaseModal
            {...modalProps}
        />;
    }
}

export default ConfirmationModal;