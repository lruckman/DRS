var React = require('react');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;
var ProgressBar = require('react-bootstrap').ProgressBar;
var FileDrop = require('react-file-drop');

var EditDocument = React.createClass({displayName: "EditDocument",
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
                location: ''
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
        xhr.onprogress = function(e) {
            if (e.lengthComputable) {  //evt.loaded the bytes browser receive
                //evt.total the total bytes seted by the header
                //
                var percentComplete = (e.loaded / e.total) * 100;
                this.setState({ uploadPercentComplete: percentComplete });
            }
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
            React.createElement("div", null, 
                React.createElement(FileDrop, {
                    frame: window, 
                    onDrop: this.handleDrop
                }, 
                    React.createElement("div", {className: "inner"}, 
                        React.createElement("div", null, 
                            React.createElement("i", {className: "fa fa-file-archive-o fa-4x"}), "  ", 
                            React.createElement("i", {className: "fa fa-file-pdf-o fa-5x"}), "  ", 
                            React.createElement("i", {className: "fa fa-file-text-o fa-4x"})
                        ), 
                        React.createElement("h4", null, "Drop to index your files")
                    )
                ), 
                React.createElement(Modal, {
                    bsSize: "sm", 
                    show: this.state.uploading
                }, 
                    React.createElement(Modal.Body, null, 
                        React.createElement(ProgressBar, {
                            active: true, 
                            striped: true, 
                            bsStyle: "success", 
                            now: this.state.uploadPercentComplete, 
                            label: 'Uploading... ' + this.state.document.title + ' - %(percent)s%'}
                        )
                    )
                ), 
                React.createElement(Modal, {bsSize: "lg", show: this.state.showModal, onHide: this.props.onClose}, 
                    React.createElement(Modal.Header, {closeButton: true}, 
                      React.createElement(Modal.Title, null, 
                        "Update Document ", React.createElement("small", null, "(",  this.state.currentFileIndex + 1, " of ",  this.state.filesToIndex.length, ")")
                      )
                    ), 
                    React.createElement(Modal.Body, null, 
                        React.createElement("form", {className: "form-horizontal"}, 
                            React.createElement("div", {className: "row"}, 
                                React.createElement("div", {className: "col-md-4"}, 
                                    React.createElement("img", {src: this.state.document.thumbnailLink, className: "img-responsive thumbnail"}), 
                                    React.createElement("dl", {className: "text-muted dl-horizontal"}, 
                                        React.createElement("dt", null, "Pages:"), 
                                        React.createElement("dd", null, this.state.document.pageCount), 
                                        React.createElement("dt", null, "File Size:"), 
                                        React.createElement("dd", null,  (this.state.document.fileSize / 1024).toFixed(2), " KB")
                                    )
                                ), 
                                React.createElement("div", {className: "col-md-8"}, 
                                    React.createElement("div", {className: "form-group"}, 
                                        React.createElement("label", {htmlFor: "title", className: "col-sm-2 control-label"}, "Title"), 
                                        React.createElement("div", {className: "col-sm-10"}, 
                                            React.createElement("input", {type: "text", 
                                                   defaultValue: this.state.document.title, 
                                                   ref: "title", 
                                                   className: "form-control", 
                                                   placeholder: "Title", 
                                                   autofocus: true})
                                        )
                                    ), 
                                    React.createElement("div", {className: "form-group"}, 
                                        React.createElement("label", {htmlFor: "abstract", className: "col-sm-2 control-label"}, "Abstract"), 
                                        React.createElement("div", {className: "col-sm-10"}, 
                                            React.createElement("textarea", {className: "form-control", 
                                                      rows: "14", 
                                                      placeholder: "Abstract", 
                                                      ref: "abstract", 
                                                      defaultValue: this.state.document.abstract})
                                        )
                                    ), 
                                    React.createElement("div", {className: "form-group"}, 
                                        React.createElement("label", {htmlFor: "libraries", className: "col-sm-2 control-label"}, "Libraries"), 
                                        React.createElement("div", {className: "col-sm-10"}, 
                                            React.createElement(Select, {multi: true, 
                                                    placeholder: "Libraries", 
                                                    simpleValue: true, 
                                                    value: this.state.document.libraryIds, 
                                                    valueKey: "Value", 
                                                    labelKey: "Text", 
                                                    options: this.props.libraries, 
                                                    onChange: this.handleLibraryChange})
                                        )
                                    )
                                )
                            )
                        )
                    ), 
                    React.createElement(Modal.Footer, null, 
                      React.createElement(Button, {onClick: this.close}, "Close"), 
                        React.createElement(Button, {onClick: this.save, bsStyle: "primary"}, "Save Changes")
                    )
                )
)
        );
    }
});

module.exports = EditDocument;