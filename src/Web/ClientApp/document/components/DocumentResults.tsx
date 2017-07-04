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
}

type OwnProps = DocumentResultsStateProps & DocumentResultsDispatchProps;

const DocumentResults = ({ onSelect, documents, nextPage, selected }: OwnProps) => {

    const handleClick = (document: DocumentFile) =>
        onSelect(document.id);

    const handleDblClick = (document: DocumentFile) =>
        window.open(document.id.toString(), '_blank'); //todo:

    return <div data-next-page={nextPage}>

            {
                documents.map(document =>
                    <DocumentResult
                        {...document}
                        key={document.id}
                        isSelected={selected.indexOf(document.id) !== -1}
                        onClick={handleClick}
                        onDblClick={handleDblClick}
                    />
                )
            }
    </div>
}

export default DocumentResults;