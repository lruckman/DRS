import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import { DocumentThumbnail, DocumentThumbnailStateProps, DocumentThumbnailDispatchProps } from '../components';

type OwnProps = {
    documentId: number
}

const mapStateToProps = (state: ApplicationState, { documentId }: OwnProps): DocumentThumbnailStateProps => {
    const document = state.entities.documents.byId[documentId];
    return {
        alt: document.title
        , url: document.thumbnailLink
    }
};

const mapDispatchToProps = (dispatch: any, props: OwnProps): DocumentThumbnailDispatchProps => ({});

export default connect(mapStateToProps, mapDispatchToProps)(DocumentThumbnail);