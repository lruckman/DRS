import * as React from 'react';
import { DocumentFile, Library } from '../../models';
import DocumentResult from './DocumentResult';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';

export type DocumentResultsStateProps = {
    keywords: string
    , libraryIds: number[]
    , libraryOptions: Library[]
    , isFetching: boolean
    , documents: DocumentFile[]
    , selected: number[]
    , nextPage: string
}

export type DocumentResultsDispatchProps = {
    onSelect: (id: number) => void
    , onOpen: (id: number) => void
}

type OwnProps = DocumentResultsStateProps & DocumentResultsDispatchProps;

const DocumentResults = ({ onSelect, documents, nextPage, onOpen, selected }: OwnProps) =>
    <div data-next-page={nextPage}>
        <ReactCSSTransitionGroup
            transitionName="result"
            transitionEnterTimeout={500}
            transitionLeaveTimeout={300}
        >
            {
                documents.map(document =>
                    <DocumentResult
                        {...document}
                        key={document.id}
                        isSelected={selected.indexOf(document.id) !== -1}
                        onClick={onSelect}
                        onDblClick={onOpen}
                    />
                )
            }
        </ReactCSSTransitionGroup>
    </div>;

export default DocumentResults;