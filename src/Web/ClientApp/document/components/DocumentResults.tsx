import * as React from 'react';
import { DocumentFile, Library } from '../../models';
import DocumentResult from './DocumentResult';

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
    </div>;

export default DocumentResults;