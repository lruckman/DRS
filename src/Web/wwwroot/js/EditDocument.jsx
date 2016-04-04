﻿var React = require('react');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;
var ProgressBar = require('react-bootstrap').ProgressBar;
var FileDrop = require('react-file-drop');

var EditDocument = React.createClass({
    propTypes: {
        libraries: React.PropTypes.array,
        source: React.PropTypes.string,
        onUpdated: React.PropTypes.func,
        onClose: React.PropTypes.func
    },
    getDefaultProps: function() {
        return {
            libraries: [],
            source: '',
            onUpdated: function () { },
            onClose: function () { }
        }    
    },
    getInitialState () {
        return {
            uploading: false,
            uploadPercentComplete: 0,
            showModal: false,
            filesToIndex: [],
            currentFileIndex: 0,
            document: {
                pageCount: 0,
                fileSize: 0,
                title: '',
                abstract: '',
                libraryIds: [],
                thumbnailLink: '',
                location: '',
                icon: ''
            }
        };
    },
    handleDrop: function (files) {
        var state = this.getInitialState();
        //todo: something if they are still indexing files
        state.filesToIndex = files;
        state.currentFileIndex = 0;

        this.setState(state);
        this.upload(files[0]);
    },
    // upload the file
    upload: function(file) {
        var xhr = new XMLHttpRequest();

        this.setState({ uploading: true });

        xhr.open('post', this.props.source, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onload = function () {

            if (xhr.status === 201) {

                var location = xhr.getResponseHeader("Location");

                this.edit(location);

                return;
            }

            alert(xhr.status + 'An error occurred!');

        }.bind(this);
        xhr.onprogress = function (e) {
            if (e.lengthComputable) {  //evt.loaded the bytes browser receive
                //evt.total the total bytes seted by the header
                //
                var percentComplete = (e.loaded / e.total) * 100;
                console.log(percentComplete);
                this.setState({ uploadPercentComplete: percentComplete });
            }
        }.bind(this);

        var formData = new FormData();

        formData.append('file', file);

        xhr.send(formData);
    },
    // get the details for the document
    edit: function (location) {
        var xhr = new XMLHttpRequest();

        xhr.open('get', location, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onload = function () {

            this.setState({
                uploading: false,
                uploadPercentComplete: 0,
            });

            if (xhr.status === 200) {

                var data = JSON.parse(xhr.responseText);
                var document = data;

                document.location = location;

                this.setState({
                    showModal: true,
                    document: document
                });

                return;
            }

            alert(xhr.status + 'An error occurred!');

        }.bind(this);

        xhr.send();
    },
    // save any changes made
    save: function () {

        // gather input values so we can update

        var title = this.refs.title.value.trim();
        var abstract = this.refs.abstract.value.trim();
        var libraryIds = this.state.document.libraryIds;

        var xhr = new XMLHttpRequest();
        xhr.open('put', this.state.document.location, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onload = function () {

            if (xhr.status !== 200) {
                // error occurred
                alert(xhr.status + 'An error occurred!');
                return;
            }

            // done here, hide the modal

            this.setState({ showModal: false });

            // successfully updated. notify parent.

            this.props.onUpdated(this.state.document.location);

            // increment the current file index

            var fileIndex = ++this.state.currentFileIndex;

            if (fileIndex < this.state.filesToIndex.length) {

                // we have other files to index move to next

                this.setState({ currentFileIndex: fileIndex });
                this.upload(this.state.files[fileIndex]);

                return;
            }

            this.setState(this.getInitialState());

        }.bind(this);

        // update the document

        var formData = new FormData();

        libraryIds.forEach(function (libraryId) {
            formData.append('libraryids', libraryId);
        });

        formData.append('title', title);
        formData.append('abstract', abstract);

        xhr.send(formData);
    },
    close: function () {
        // reset component state
        this.setState(this.getInitialState());
        // tell the parent
        this.props.onClose();
    },
    handleLibraryChange (value) {
        var document = this.state.document;

        document.libraryIds = (typeof value === 'string')
            ? [value]
            : value;

        this.setState({
            document: document
        });
    },
    render: function () {
        return (
            <div>
                <FileDrop 
                    frame={window}
                    onDrop={this.handleDrop}
                >
                    <div className="inner">
                        <div>
                            <i className="fa fa-file-archive-o fa-4x"></i> &nbsp;
                            <i className="fa fa-file-pdf-o fa-5x"></i> &nbsp;
                            <i className="fa fa-file-text-o fa-4x"></i>
                        </div>
                        <h4>Drop to index your files</h4>
                    </div>
                </FileDrop>
                <Modal 
                    bsSize="sm" 
                    show={this.state.uploading}
                >
                    <Modal.Body>
                        <ProgressBar 
                            active
                            striped
                            bsStyle="success"
                            now={this.state.uploadPercentComplete}
                            label={'Uploading... ' + this.state.document.title + ' - %(percent)s%'} 
                        />
                    </Modal.Body>
                </Modal>
                <Modal bsSize="lg" show={this.state.showModal} onHide={this.close}>
                    <Modal.Header closeButton>
                      <Modal.Title>
                        Update Document <small>({ this.state.currentFileIndex + 1} of { this.state.filesToIndex.length })</small>
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
                                        <dd>{ (this.state.document.fileSize / 1024).toFixed(2) } KB</dd>
                                    </dl>
                                </div>
                                <div className="col-md-8">
                                    <div className="form-group">
                                        <label htmlFor="title" className="col-sm-2 control-label">Title</label>
                                        <div className="col-sm-10">
                                            <input 
                                                type="text"
                                                defaultValue={this.state.document.title}
                                                ref="title"
                                                className="form-control"
                                                placeholder="Title"
                                                autofocus 
                                            />
                                        </div>
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="abstract" className="col-sm-2 control-label">Abstract</label>
                                        <div className="col-sm-10">
                                            <textarea 
                                                className="form-control"
                                                rows="14"
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
                                                valueKey="Value"
                                                labelKey="Text"
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
</div>
        );
    }
});

module.exports = EditDocument;