import * as React from 'react';
import { DocumentFile, Library } from '../../models';
import { DocumentResult, DocumentContextMenu } from './';
import { ContextMenuTrigger } from 'react-contextmenu';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';

import './DocumentResults.css';

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
    onDelete: (id: number) => void
    , onEdit: (id: number) => void
    , onSelect: (id: number) => void
}

type OwnProps = DocumentResultsStateProps & DocumentResultsDispatchProps;

function collect(props) {
    return props;
}

class DocumentResults extends React.Component<OwnProps, null> {

    constructor(props: OwnProps) {
        super(props);

        this.handleItemClick = this.handleItemClick.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleEdit = this.handleEdit.bind(this);
    }

    handleEdit(e: React.SyntheticEvent<any>, documentId: number) {
        this.props.onEdit(documentId);
    }

    handleDelete(e: React.SyntheticEvent<any>, documentId: number) {
        this.props.onDelete(documentId);
    }

    handleItemClick(e: React.SyntheticEvent<any>, action: string, documentId: number) {
        if (action === 'delete') {
            this.handleDelete(e, documentId);
            return;
        }
        if (action === 'edit') {
            this.handleEdit(e, documentId);
            return;
        }
        console.log(`Unknown document context menu action: ${action}`);
    }

    render() {
        const { onDelete, onEdit, onSelect, documents, nextPage, selected } = this.props;
        return <div data-next-page={nextPage}>
            <ReactCSSTransitionGroup
                transitionName="result"
                transitionEnterTimeout={500}
                transitionLeaveTimeout={300}
            >
                {
                    documents.map(document =>
                        <div className="card pulse" key={document.id}>
                            <ContextMenuTrigger
                                id="documentContextMenu"
                                collect={collect}
                                onItemClick={(e: React.SyntheticEvent<any>, data: { action: string }, target: any) => this.handleItemClick(e, data.action, document.id)}
                            >
                                <DocumentResult
                                    {...document}
                                    isSelected={selected.indexOf(document.id) !== -1}
                                    onSelect={onSelect}
                                />
                            </ContextMenuTrigger>
                        </div>
                    )
                }
            </ReactCSSTransitionGroup>
            <DocumentContextMenu />
        </div>;
    }
}

export default DocumentResults;