﻿﻿import * as React from 'react';
import { Modal } from 'react-bootstrap';

import './BaseModal.css';

type ModalStateProps = {
    title: string
    , children?: JSX.Element
    , buttons: JSX.Element[]
    , show: boolean
    , className: string
}

type ModalDispatchProps = {
    onHide: () => void
}

type OwnProps = ModalStateProps & ModalDispatchProps;

const BaseModal = ({ title, children, buttons, show, onHide, className }: OwnProps) =>
    <Modal
        backdrop="static"
        show={show}
        onHide={onHide}
        dialogClassName={className}
    >
        {
            title &&
            <Modal.Header closeButton>
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