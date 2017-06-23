import * as React from 'react';
import classNames from 'classnames';
import ValidationSummary from './ValidationSummary';
import { Button, Modal, ProgressBar } from 'react-bootstrap';
import * as Select from 'react-select';
import FileDrop from 'react-file-drop';
import { ILibraryListItem } from './ILibraryListItem';
import { IDocumentDetails } from './IDocumentDetails';
import { IModelError } from './IModelError';
import update from 'immutability-helper';

export type DocumentEditStateProps = {
    libraries: ILibraryListItem[];
    location: string;
}

export type DocumentEditDispatchProps = {
    onUpdate: (location: string) => void;
    onClose: () => void;
}

type DocumentEditState = {
    add?: {
        uploading: boolean;
        uploadPercent: number;
        files?: File[];
        count: number;
    };
    showModal?: boolean;
    validationErrors?: IModelError[]; //todo: define
    document?: IDocumentDetails;
}

export default class DocumentEdit extends React.Component<DocumentEditStateProps & DocumentEditDispatchProps, DocumentEditState> {
    
    constructor() {
        super();

        this.state = this.getDefaultState();
        this.close = this.close.bind(this);
        this.save = this.save.bind(this);
        this.handleFileDrop = this.handleFileDrop.bind(this);
        this.handleLibraryChange = this.handleLibraryChange.bind(this);
    }

    refs: {
        abstract: (HTMLInputElement),
        title: (HTMLInputElement)
    }

    getDefaultState(): DocumentEditState {
        return {
            add: {
                uploading: false,
                uploadPercent: 0,
                files: [],
                count: 0
            },
            showModal: false,
            validationErrors: [],
            document: {
                abstract: '',
                title: '',
                libraryIds: [],
                createdOn: null,
                modifiedOn: null,
                pageCount: 0,
                size: 0,
                thumbnailLink: '',
                versionNum: 0,
                resourceUri: ''
            }
        };
    }

    componentWillReceiveProps(props: DocumentEditStateProps & DocumentEditDispatchProps) {
        if (props.location && this.props.location !== props.location) {
            this.edit(props.location);
        }
        this.setState({
            showModal: false
        });
    }

    handleFileDrop(files: File[]) {
        var state = this.getDefaultState();

        for (var i = 0, file; file = files[i]; i++) {
            state.add.files.push(file);
        }

        state.add.count = state.add.files.length;

        this.upload(state.add.files.pop());
        this.setState(state);
    }

    // upload the file
    upload(file: File) {
        var xhr = new XMLHttpRequest();

        this.setState({
            add: update(this.state.add, {
                uploading: { $set: true }
            })
        });

        xhr.open('post', '/api/documents', true);
        xhr.setRequestHeader('Accept', 'application/json');

        xhr.onload = function () {

            this.setState({
                add: update(this.state.add, {
                    uploading: { $set: false },
                    uploadPercent: { $set: 0 }
                })
            });

            if (xhr.status === 201) {

                var location = xhr.getResponseHeader("Location");

                this.edit(location);

                return;
            }

            alert(xhr.status + ' An error occurred!');

        }.bind(this);

        xhr.onprogress = function (e) {
            if (e.lengthComputable) {  //evt.loaded the bytes browser receive
                //evt.total the total bytes seted by the header
                //
                var percentComplete = (e.loaded / e.total) * 100;
                console.log(percentComplete);

                this.setState({
                    add: update(this.state.add, {
                        uploadPercent: { $set: percentComplete }
                    })
                });
            }
        }.bind(this);

        var formData = new FormData();

        formData.append('file', file);

        xhr.send(formData);
    }

    // get the details for the document
    edit(location: string) {
        var xhr = new XMLHttpRequest();

        xhr.open('get', location, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onload = function () {

            if (xhr.status === 200) {
                
                var document = JSON.parse(xhr.responseText); // as IDocumentDetails;

                document.resourceUri = location;

                this.setState({
                    showModal: true,
                    document: document
                });

                return;
            }

            alert(xhr.status + 'An error occurred!');

        }.bind(this);

        xhr.send();
    }

    // save any changes made
    save() {

        // gather input values so we can update

        var title = this.refs.title.value.trim();
        var abstract = this.refs.abstract.value.trim();
        var libraryIds = this.state.document.libraryIds;

        var xhr = new XMLHttpRequest();
        xhr.open('put', this.state.document.resourceUri, true);
        xhr.setRequestHeader('Accept', 'application/json');

        xhr.onload = function () {

            if (xhr.status === 400) {

                var data = JSON.parse(xhr.responseText);
                this.setState({ validationErrors: data.errors });

                return;
            }

            if (xhr.status !== 200) {

                alert(xhr.status + 'An error occurred!');

                return;
            }

            // done here, hide the modal

            this.setState({ showModal: false });

            // successfully updated. notify parent.

            this.props.onUpdate(this.state.document.location);

            if (this.state.add.files.length > 0) {

                // we have other files to index move to next

                var files = this.state.add.files.slice();

                // upload the next

                this.upload(files.pop());

                this.setState({
                    add: update(this.state.add, {
                        files: { $set: files }
                    })
                });

                return;
            }

            this.setState(this.getDefaultState());

        }.bind(this);

        // update the document

        var formData = new FormData();

        libraryIds.forEach(function (libraryId) {
            formData.append('libraryids', libraryId);
        });

        formData.append('title', title);
        formData.append('abstract', abstract);

        xhr.send(formData);
    }

    close() {
        // reset component state
        this.setState(this.getDefaultState());
        // tell the parent
        this.props.onClose();
    }

    handleLibraryChange(value: string | number[]) {
        var document = this.state.document;

        document.libraryIds = (typeof value === 'string')
            ? ((value === "") ? [] : [Number(value)])
            : value;

        this.setState({ document: document });
    }
    render() {

        var fileDrop;

        if (typeof (window) !== 'undefined') {
            fileDrop = <FileDrop
                frame={window}
                onDrop={this.handleFileDrop}>
                <div className="inner">
                    <div>
                        <i className="fa fa-file-archive-o fa-4x"></i> &nbsp;
                            <i className="fa fa-file-pdf-o fa-5x"></i> &nbsp;
                            <i className="fa fa-file-text-o fa-4x"></i>
                    </div>
                    <h4>Drop to index your files</h4>
                </div>
            </FileDrop>;
        }

        return <div>
            {fileDrop}
            <Modal
                backdrop="static"
                bsSize="sm"
                show={this.state.add.uploading}>
                <Modal.Body>
                    <ProgressBar
                        active
                        striped
                        bsStyle="success"
                        max={100}
                        min={0}
                        now={this.state.add.uploadPercent}
                        label={'Uploading... - %(percent)s%'} />
                </Modal.Body>
            </Modal>
            <Modal
                backdrop="static"
                bsSize="lg"
                show={this.state.showModal}
                onHide={this.close}>
                <Modal.Header closeButton>
                    <Modal.Title>
                        Update Document
                      </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className="form-horizontal">
                        <div className="row">
                            <div className="col-md-4">
                                <img src={this.state.document.thumbnailLink} className="img-responsive thumbnail" />
                                <dl className="text-muted dl-horizontal">
                                    <dt>Pages:</dt>
                                    <dd>{this.state.document.pageCount}</dd>
                                    <dt>File Size:</dt>
                                    <dd>{(this.state.document.size / 1024).toFixed(2)} KB</dd>
                                </dl>
                            </div>
                            <div className="col-md-8">
                                <ValidationSummary
                                    errors={this.state.validationErrors || undefined} />
                                <div className="form-group">
                                    <label htmlFor="title" className="col-sm-2 control-label">Title</label>
                                    <div className="col-sm-10">
                                        <input
                                            type="text"
                                            defaultValue={this.state.document.title}
                                            ref="title"
                                            className="form-control"
                                            placeholder="Title"
                                            autoFocus
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
                                            ref="abstract"
                                            defaultValue={this.state.document.abstract}
                                            />
                                    </div>
                                </div>
                                <div className="form-group">
                                    <label htmlFor="libraries" className="col-sm-2 control-label">Libraries</label>
                                    <div className="col-sm-10">
                                        <Select
                                            multi
                                            placeholder="Libraries"
                                            simpleValue
                                            value={this.state.document.libraryIds}
                                            valueKey="value"
                                            labelKey="text"
                                            options={this.props.libraries}
                                            onChange={this.handleLibraryChange}
                                            />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.close}>Close</Button>
                    <Button onClick={this.save} bsStyle="primary">Save Changes</Button>
                </Modal.Footer>
            </Modal>
        </div>;
    }
}