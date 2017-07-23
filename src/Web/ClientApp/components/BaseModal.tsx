﻿import * as React from 'react';
import { Modal } from 'react-bootstrap';

import './BaseModal.css';

export type BaseModalStateProps = {
    title: string
    , children?: JSX.Element
    , buttons: JSX.Element[]
    , show: boolean
    , className: string
    , closeButton?: boolean
}

export type BaseModalDispatchProps = {
    onHide: () => void
}

type OwnProps = BaseModalStateProps & BaseModalDispatchProps;

const BaseModal = ({ title, children, buttons, show, onHide, className, closeButton }: OwnProps) =>
    <Modal
        backdrop="static"
        show={show}
        onHide={onHide}
        dialogClassName={className}
    >
        {
            title &&
            <Modal.Header closeButton={closeButton}>
                <Modal.Title>{title}</Modal.Title>
            </Modal.Header>
        }
        <Modal.Body>
            {children}
        </Modal.Body>
        {
            buttons &&
            <Modal.Footer>
                {buttons}
            </Modal.Footer>
        }
    </Modal>;

export default BaseModal;