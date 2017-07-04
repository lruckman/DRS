import * as React from 'react';

export type DocumentThumbnailStateProps = {
    url: string
}

export type DocumentThumbnailDispatchProps = {}

type OwnProps = DocumentThumbnailDispatchProps & DocumentThumbnailStateProps;

const DocumentThumbnail = ({ url }: OwnProps) =>
    <img src={url} className="img-responsive thumbnail" />;

export default DocumentThumbnail;