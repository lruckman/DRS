import * as React from 'react';
import { Button } from 'react-bootstrap';
import { BaseModal } from '../../components';
import { DocumentThumbnail } from '../components';
import { DocumentFile } from '../../models';

export type DocumentEditorStateProps = {
    editCount: number
    , document: DocumentFile
    , show?: boolean
}

export type DocumentEditorDispatchProps = {
    onHide: () => void
    , onSave: (values: DocumentFile) => void
}

type OwnProps = DocumentEditorDispatchProps & DocumentEditorStateProps;

type OwnState = {
    document: DocumentFile
}

class DocumentEditor extends React.Component<OwnProps, OwnState> {

    constructor(props: OwnProps) {
        super(props);

        this.state = {
            document: { ...props.document }
        }

        this.handleSave = this.handleSave.bind(this);
        this.handleChange = this.handleChange.bind(this);
    }

    componentWillReceiveProps(nextProps: OwnProps) {
        if (this.props.document !== nextProps.document) {
            this.setState({
                ...this.state
                , document: { ...nextProps.document }
            });
        }
    }

    handleSave() {
        this.props.onSave({ ...this.state.document });
    }

    handleChange(e: React.SyntheticEvent<any>) {
        this.setState({
            ...this.state
            , document: {
                ...this.state.document
                , [e.currentTarget.name]: e.currentTarget.value
            }
        });
    }

    public render() {

        const buttons = [
            <Button key="closeButton" onClick={this.props.onHide}>Close</Button>
            , <Button key="saveButton" onClick={this.handleSave} bsStyle="primary">Save Changes</Button>
        ]

        const documentForm = <form className="form-horizontal">
            <div className="row">
                <div className="col-md-4">
                    <DocumentThumbnail
                        alt={this.state.document.title}
                        url={this.state.document.thumbnailLink}
                    />
                    <dl className="text-muted dl-horizontal">
                        <dt>Pages:</dt>
                        <dd>{this.state.document.pageCount}</dd>
                        <dt>File Size:</dt>
                        <dd>{(this.state.document.size / 1024).toFixed(2)} KB</dd>
                    </dl>
                </div>
                <div className="col-md-8">
                    {/*<ValidationSummary
                                    errors={this.state.validationErrors || undefined} />*/}
                    <div className="form-group">
                        <label htmlFor="title" className="col-sm-2 control-label">Title</label>
                        <div className="col-sm-10">
                            <input
                                type="text"
                                defaultValue={this.state.document.title}
                                name="title"
                                className="form-control"
                                placeholder="Title"
                                autoFocus
                                onChange={this.handleChange}
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <label htmlFor="abstract" className="col-sm-2 control-label">Abstract</label>
                        <div className="col-sm-10">
                            <textarea
                                className="form-control"
                                rows={Number("14")}
                                placeholder="Abstract"
                                name="abstract"
                                defaultValue={this.state.document.abstract}
                                onChange={this.handleChange}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </form>;

        const modalProps = {
            buttons
            , onHide: this.props.onHide
            , show: this.props.show
            , title: 'Update Document'
            , className: 'modal-lg'
            , closeButton: true
            , children: documentForm
        }

        return <BaseModal
            {...modalProps}
        />;
    }
}

export default DocumentEditor;