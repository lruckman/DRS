var React = require('react');
var ReactDOM = require('react-dom');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;

var AddDocument = React.createClass({displayName: "AddDocument",
    getInitialState: function() {
        return { showModel: false };
    },
    close: function() {
        this.setState({ showModal: false });
    },
    open: function() {
        this.setState({ showModal: true });
    },
    save: function () {
        var files = this.refs.file.files;
        var libraryId = this.refs.library.value.trim();
        var title = this.refs.title.value.trim();
        var abstract = this.refs.abstract.value.trim();
        var genAbstract = this.refs.generateAbstract.checked;

        this.props.onSubmit(files, libraryId, title, abstract, genAbstract);
    },
    render: function () {
        var libraryNodes = this.props.libraries.map(function(library) {
            return (
                React.createElement("option", {key: library.Value, value: library.Value}, 
                    library.Text
                )
            );
        }.bind(this));
        return (
              React.createElement("div", null, 
                React.createElement(Button, {bsStyle: "default", bsSize: "large", onClick: this.open}, 
                React.createElement("i", {className: "fa fa-plus"})
                ), 

              React.createElement(Modal, {show: this.state.showModal, onHide: this.close}, 
                React.createElement(Modal.Header, {closeButton: true}, 
                  React.createElement(Modal.Title, null, "Add Document")
                ), 
                React.createElement(Modal.Body, null, 
                    React.createElement("form", {className: "form-horizontal"}, 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "library", className: "col-sm-2 control-label"}, "Library"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("select", {ref: "library", id: "library", className: "form-control"}, libraryNodes
                                )
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "file", className: "col-sm-2 control-label"}, "Document"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("input", {type: "file", ref: "file", id: "file", className: "form-control"})
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "title", className: "col-sm-2 control-label"}, "Title"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("input", {type: "text", ref: "title", id: "title", className: "form-control"})
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "abstract", className: "col-sm-2 control-label"}, "Abstract"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("textarea", {ref: "abstract", id: "abstract", className: "form-control", rows: "3"})
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("div", {className: "col-sm-offset-2 col-sm-10"}, 
                                React.createElement("div", {className: "checkbox"}, 
                                    React.createElement("label", null, 
                                        React.createElement("input", {type: "checkbox", ref: "generateAbstract"}), " Automatically generate abstract"
                                    )
                                )
                            )
                        )
                    )
                ), 
                React.createElement(Modal.Footer, null, 
                  React.createElement(Button, {onClick: this.close}, "Close"), 
                    React.createElement(Button, {onClick: this.save}, "Save")
                )
              )
            )
        );
    }
});

var SearchForm = React.createClass({displayName: "SearchForm",
    getInitialState () {
        return {
            libraries: []
        };
    },
    search: function() {
        var q = this.refs.search.value.trim();
        var libraries = [];
        if (this.state.libraries !== null) {
            this.state.libraries.forEach(function(lib) {
                libraries.push(lib.Value);
            });
        }
        this.props.onSearchSubmit(q, libraries);
    },
    handleSubmit: function(e) {
        e.preventDefault();
        this.search();
    },
    handleSelectChange (value) {
        this.setState({ libraries: value });
        this.search();
    },
    render: function () {
        return (
            React.createElement("form", {className: "search-form", onSubmit: this.handleSubmit}, 
                React.createElement(AddDocument, {libraries: this.props.libraries, onSubmit: this.props.onAddSubmit}), 
                React.createElement("div", {className: "input-group"}, 
                  React.createElement("input", {type: "text", className: "form-control", placeholder: "Search for documents", ref: "search"}), 
                  React.createElement("span", {className: "input-group-btn"}, 
                    React.createElement("button", {className: "btn btn-default", type: "submit"}, "Go!")
                  )
                ), 
                React.createElement("div", {className: "form-group"}, 
                    React.createElement("label", {htmlFor: ""}, "Filter by libraries"), 
                    React.createElement(Select, {multi: true, value: this.state.libraries, 
                            valueKey: "Value", 
                            labelKey: "Text", 
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
    loadResultsFromServer: function (search, libraries) {
        var xhr = new XMLHttpRequest();

        xhr.open('get', this.props.searchUrl + "?q=" + search + "&libraries=" + libraries.join("&libraries="), true);
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
    handleSearchSubmit: function (search, libraryId) {
        this.loadResultsFromServer(search, libraryId);
    },
    handleAddSubmit: function (files, libraryId, title, abstract, genAbstract) {
        var xhr = new XMLHttpRequest();
        xhr.open('post', this.props.addDocumentUrl, true);
        xhr.onload = function () {
            if (xhr.status === 201) {
                alert('document created.');
            } else {
                alert(xhr.status + 'An error occurred!');
            }
        }.bind(this);

        var formData = new FormData();

        formData.append('libraryid', libraryId);
        formData.append('title', title);
        formData.append('abstract', abstract);
        formData.append('generateAbstract', genAbstract);

        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            formData.append('file', file, file.name);
        }

        xhr.send(formData);
    },
    getInitialState: function() {
        return { data: [] };
    },
    componentWillMount: function() {
    },
    render: function() {
        return (
            React.createElement("div", {className: "search-box row"}, 
                React.createElement("div", {className: "col-sm-3"}, 
                    React.createElement(SearchForm, {onSearchSubmit: this.handleSearchSubmit, 
                                onAddSubmit: this.handleAddSubmit, 
                                libraries: this.props.libraries})
                ), 
                React.createElement("div", {className: "col-sm-9"}, 
                    React.createElement(ResultList, {data: this.state.data})
                )
            )
        );
    }
});

module.exports = SearchBox;