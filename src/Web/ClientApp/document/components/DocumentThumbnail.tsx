import * as React from 'react';

export type DocumentThumbnailStateProps = {
    url: string
    , alt: string
}

export type DocumentThumbnailDispatchProps = {}

type OwnProps = DocumentThumbnailDispatchProps & DocumentThumbnailStateProps;

const DocumentThumbnail = ({ alt, url }: OwnProps) =>
    <img src={url} alt={alt} className="img-responsive thumbnail" />;

export default DocumentThumbnail;