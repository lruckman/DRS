var React = require('react');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;
var FileDrop = require('react-file-drop');

var AddDocument = React.createClass({displayName: "AddDocument",
    getInitialState () {
        return {
            libraryIds: [],
            disableAbstract: true
        };
    },
    save: function () {
        var title = this.refs.title.value.trim();
        var abstract = this.refs.abstract.value.trim();
        var genAbstract = this.refs.generateAbstract.checked;
        var libraryIds = this.state.libraryIds;

        this.props.onSubmit(this.props.file, libraryIds, title, abstract, genAbstract);

        this.setState({ libraryIds: [] });
    },
    handleSelectChange (value) {
        if (typeof value === 'string') {
            this.setState({ libraryIds: [value] });
            return;
        }
        this.setState({ libraryIds: value });
    },
    handleGenerateAbstractChange (e) {
        this.setState({ disableAbstract: e.target.checked });
    },
    render: function () {
        return (
              React.createElement(Modal, {show: this.props.showModal, onHide: this.props.onClose}, 
                React.createElement(Modal.Header, {closeButton: true}, 
                  React.createElement(Modal.Title, null, "Add Document")
                ), 
                React.createElement(Modal.Body, null, 
                    React.createElement("form", {className: "form-horizontal"}, 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {className: "col-sm-2 control-label"}, "File"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("div", {className: "form-control-static"}, 
                                    React.createElement("strong", null, this.props.file ? this.props.file.name : ""), " ", React.createElement("small", {className: "text-muted"}, this.props.file ? (this.props.file.size / 1024).toFixed(2) : "", " KB")
                                )
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "title", className: "col-sm-2 control-label"}, "Title"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("input", {type: "text", ref: "title", id: "title", className: "form-control", placeholder: "Title", autofocus: true})
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "abstract", className: "col-sm-2 control-label"}, "Abstract"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("textarea", {ref: "abstract", id: "abstract", className: "form-control", rows: "3", placeholder: "Abstract", disabled: this.state.disableAbstract})
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("div", {className: "col-sm-offset-2 col-sm-10"}, 
                                React.createElement("div", {className: "checkbox"}, 
                                    React.createElement("label", null, 
                                        React.createElement("input", {type: "checkbox", ref: "generateAbstract", defaultChecked: this.state.disableAbstract, onChange: this.handleGenerateAbstractChange}), " Automatically generate abstract"
                                    )
                                )
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "libraries", className: "col-sm-2 control-label"}, "Libraries"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement(Select, {multi: true, value: this.state.libraryIds, 
                                        placeholder: "Libraries", 
                                        simpleValue: true, 
                                        valueKey: "Value", 
                                        labelKey: "Text", 
                                        options: this.props.libraries, 
                                        onChange: this.handleSelectChange})
                            )
                        )
                    )
                ), 
                React.createElement(Modal.Footer, null, 
                  React.createElement(Button, {onClick: this.props.onClose}, "Close"), 
                    React.createElement(Button, {onClick: this.save, bsStyle: "primary"}, "Save Changes")
                )
              )
        );
    }
});

var SearchForm = React.createClass({displayName: "SearchForm",
    getInitialState () {
        return {
            libraryIds: []
        };
    },
    search: function() {
        var q = this.refs.search.value.trim();
        this.props.onSearchSubmit(q, this.state.libraryIds || []);
    },
    handleSubmit: function(e) {
        e.preventDefault();
        this.search();
    },
    handleSelectChange (value) {
        this.setState({ libraryIds: value });
        this.search();
    },
    render: function () {
        return (
            React.createElement("form", {className: "search-form", onSubmit: this.handleSubmit}, 
                React.createElement("div", {className: "form-group"}, 
                    React.createElement("div", {className: "input-group integrated"}, 
                      React.createElement("input", {type: "text", className: "form-control", placeholder: "Search for documents", ref: "search"}), 
                      React.createElement("span", {className: "input-group-addon"}, 
                        React.createElement("button", {type: "submit"}, 
                          React.createElement("i", {className: "fa fa-search"})
                        )
                      )
                    )
                ), 
                React.createElement("div", {className: "form-group"}, 
                    React.createElement("label", {htmlFor: ""}, "Filter by libraries"), 
                    React.createElement(Select, {multi: true, value: this.state.libraryIds, 
                            valueKey: "Value", 
                            labelKey: "Text", 
                            simpleValue: true, 
                            options: this.props.libraries, 
                            placeholder: "All libraries", 
                            onChange: this.handleSelectChange})
                )
            )
        );
    }
});

var Result = React.createClass({displayName: "Result",
    render: function() {
        return (
            React.createElement("div", {className: "result media"}, 
                React.createElement("div", {className: "media-left"}, 
                    React.createElement("a", {href: this.props.viewLink, target: "_blank"}, 
                        React.createElement("img", {src: this.props.thumbnailLink, className: "media-object thumbnail", alt: this.props.title, style: {width: 150 + 'px'}})
                    )
                ), 
                React.createElement("div", {className: "media-body"}, 
                    React.createElement("h4", {className: "media-heading"}, 
                        this.props.title
                    ), 
                    this.props.children
                )
            )
        );
    }
});

var ResultList = React.createClass({displayName: "ResultList",
    render: function () {
        var documents = this.props.data.documents
            ? this.props.data.documents
            : [];
        var resultNodes = documents.map(function (result) {
            return (
                React.createElement(Result, {key: result.id, title: result.title, thumbnailLink: result.thumbnailLink, viewLink: result.viewLink}, 
                    result.abstract
                )
            );
        });
        return (
            React.createElement("div", {className: "result-list clearfix", "data-next-link": this.props.data.nextLink}, 
                resultNodes
            )
        );
    }
});

var SearchBox = React.createClass({displayName: "SearchBox",
    loadResultsFromServer: function (search, libraryIds) {
        var xhr = new XMLHttpRequest();

        xhr.open('get', this.props.searchUrl + "?q=" + search + "&libraryids=" + libraryIds.join("&libraryids="), true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                this.setState({ data: data });
            } else {
                alert('An error occurred!');
            }
        }.bind(this);
        xhr.send();
    },
    handleSearchSubmit: function (search, libraryIds) {
        this.loadResultsFromServer(search, libraryIds);
    },
    handleAddSubmit: function (file, libraryIds, title, abstract, genAbstract) {
        var xhr = new XMLHttpRequest();
        xhr.open('post', this.props.addDocumentUrl, true);

        xhr.onload = function () {
            if (xhr.status === 201) {

                // copy add object so we can update it

                var add = this.state.add;

                if ( (add.index+1) < add.files.length) {

                    // set the next file

                    add.file = add.files[++add.index]; 

                    // update state

                    this.setState({ add: add });

                    return;
                }

                alert((add.index + 1) + ' documents created.');
                return;
            }

            alert(xhr.status + 'An error occurred!');

        }.bind(this);

        var formData = new FormData();

        libraryIds.forEach(function(libraryId) {
            formData.append('libraryids', libraryId);
        });

        formData.append('title', title);
        formData.append('abstract', abstract);
        formData.append('generateAbstract', genAbstract);

        formData.append('file', file);

        xhr.send(formData);
    },
    handleAddClose: function() {
        this.setState({ add: { show: false, files: [], file: null } });
    },
    handleFileDrop: function (files, event) {
        console.log(files, event);

        // get the default state for the add object

        var add = this.getInitialState().add;

        // update the defaults

        add.files = files;
        add.file = files[add.index];
        add.show = true;

        this.setState({ add: add });
    },
    getInitialState: function() {
        return {
            data: [],
            add: {
                show: false,
                file: null,
                files: [],
                index: 0
            }
        };
    },
    componentWillMount: function() {
    },
    render: function() {
        return (
            React.createElement("div", {className: "search-box row"}, 
                React.createElement(FileDrop, {frame: window, targetAlwaysVisible: true, 
                          onDrop: this.handleFileDrop}, 
                    React.createElement("i", {className: "fa fa-cloud-upload fa-5x"}), 
                    React.createElement("h4", null, "Drop Files Here to Add")
                ), 
                React.createElement(AddDocument, {libraries: this.props.libraries, 
                             file: this.state.add.file, 
                             onSubmit: this.handleAddSubmit, 
                             onClose: this.handleAddClose, 
                             showModal: this.state.add.show}), 
                React.createElement("div", {className: "col-sm-3"}, 
                    React.createElement(SearchForm, {libraries: this.props.libraries, 
                                onSearchSubmit: this.handleSearchSubmit})
                ), 
                React.createElement("div", {className: "col-sm-9"}, 
                    React.createElement(ResultList, {data: this.state.data})
                )
            )
        );
    }
});

module.exports = SearchBox;