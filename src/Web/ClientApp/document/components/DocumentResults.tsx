import * as React from 'react';
import { DocumentFile, Library } from '../../models';
import DocumentResult from './DocumentResult';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import { ContextMenu, MenuItem, ContextMenuTrigger } from 'react-contextmenu';

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

class DocumentResults extends React.Component<OwnProps, null> {

    constructor(props: OwnProps) {
        super(props);

        this.handleDelete = this.handleDelete.bind(this);
        this.handleEdit = this.handleEdit.bind(this);
    }

    handleEdit(e: React.SyntheticEvent<any>, data: { id: number }) {
        this.props.onEdit(data.id);
    }

    handleDelete(e: React.SyntheticEvent<any>, data: { id: number }) {
        this.props.onDelete(data.id);
    }

    render() {
        const { onDelete, onEdit, onSelect, documents, nextPage, selected } = this.props;
        return <div data-next-page={nextPage}>
            <ReactCSSTransitionGroup
                transitionName="result"
                transitionEnterTimeout={500}
                transitionLeaveTimeout={300}
            >

                <ContextMenuTrigger
                    id="documentContextMenu"
                >
                    {
                        documents.map(document =>
                            <DocumentResult
                                {...document}
                                key={document.id}
                                isSelected={selected.indexOf(document.id) !== -1}
                                onSelect={onSelect}
                            />
                        )
                    }
                </ContextMenuTrigger>
                <ContextMenu
                    id="documentContextMenu"
                    className="menu"
                >
                    <MenuItem data={"some_data"} onClick={this.handleEdit}>
                        Edit
                    </MenuItem>
                    <MenuItem divider />
                    <MenuItem data={"some_data"} onClick={this.handleDelete}>
                        Delete
                    </MenuItem>
                </ContextMenu>
            </ReactCSSTransitionGroup>
        </div>;
    }
}

export default DocumentResults;