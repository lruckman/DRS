import * as React from 'react';
import FileDrop from 'react-file-drop';

export type DocumentFileDropStateProps = {};

export type DocumentFileDropDispatchProps = {
    onDrop: (files: File[]) => void
}

type OwnProps = DocumentFileDropDispatchProps & DocumentFileDropStateProps;

const DocumentFileDrop = ({ onDrop }: OwnProps) => {
    if (window) {
        return <FileDrop
            frame={window}
            onDrop={onDrop}>
            <div className="inner">
                <div>
                    <i className="fa fa-file-archive-o fa-4x"></i> &nbsp;
                    <i className="fa fa-file-pdf-o fa-5x"></i> &nbsp;
                    <i className="fa fa-file-text-o fa-4x"></i>
                </div>
                <h4>Drop to index your files</h4>
            </div>
        </FileDrop>;
    }

    return null;
}

export default DocumentFileDrop;