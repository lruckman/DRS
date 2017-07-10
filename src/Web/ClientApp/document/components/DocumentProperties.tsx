import * as React from 'react';
import * as moment from 'moment';
import { DocumentThumbnail } from '../components';
import { DocumentFile } from '../../models';
import { UpdatingDateLabel } from '../../components';

export type DocumentPropertiesStateProps = {
    document: DocumentFile
}

export type DocumentPropertiesDispatchProps = {}

type OwnProps = DocumentPropertiesDispatchProps & DocumentPropertiesStateProps;

const DocumentProperties = ({ document }: OwnProps) => {

    if (!document) {
        return null;
    }

    const { thumbnailLink, title, abstract, size, pageCount, createdOn, modifiedOn, version } = document;

    return <div>
        <DocumentThumbnail url={thumbnailLink} alt={title} />
        <h3>{title}</h3>
        <p>{abstract}</p>
        <dl>
            <dt>Size:</dt>
            <dd>{size} KB</dd>
            <dt>Page Count:</dt>
            <dd>{pageCount}</dd>
            <dt>Added On</dt>
            <dd>
                <UpdatingDateLabel
                    date={createdOn}
                />
            </dd>
            <dt>Updated On</dt>
            <dd>
                <UpdatingDateLabel
                    date={modifiedOn}
                />
            </dd>
            <dt>Version Num</dt>
            <dd>{version}</dd>
        </dl>
    </div>;
}

export default DocumentProperties;