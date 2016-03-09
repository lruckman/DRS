﻿var React = require('react');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;
var FileDrop = require('react-file-drop');

var AddDocument = React.createClass({
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
                <option key={library.Value} value={library.Value}>
                    {library.Text}
                </option>
            );
        }.bind(this));
        return (
              <Modal show={this.props.showModal} onHide={this.props.onClose}>
                <Modal.Header closeButton>
                  <Modal.Title>Add Document</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className="form-horizontal">
                        <div className="form-group">
                            <label htmlFor="library" className="col-sm-2 control-label">Library</label>
                            <div className="col-sm-10">
                                <select ref="library" id="library" className="form-control">
                                    {libraryNodes}
                                </select>
                            </div>
                        </div>
                        {this.props.file ? this.props.file.name : ""}
                        {this.props.file ? this.props.file.size : ""}
                        <div className="form-group">
                            <label htmlFor="file" className="col-sm-2 control-label">Document</label>
                            <div className="col-sm-10">
                                <input type="file" ref="file" id="file" className="form-control" />
                            </div>
                        </div>
                        <div className="form-group">
                            <label htmlFor="title" className="col-sm-2 control-label">Title</label>
                            <div className="col-sm-10">
                                <input type="text" ref="title" id="title" className="form-control" />
                            </div>
                        </div>
                        <div className="form-group">
                            <label htmlFor="abstract" className="col-sm-2 control-label">Abstract</label>
                            <div className="col-sm-10">
                                <textarea ref="abstract" id="abstract" className="form-control" rows="3"></textarea>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col-sm-offset-2 col-sm-10">
                                <div className="checkbox">
                                    <label>
                                        <input type="checkbox" ref="generateAbstract" /> Automatically generate abstract
                                    </label>
                                </div>
                            </div>
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                  <Button onClick={this.props.onClose}>Close</Button>
                    <Button onClick={this.save}>Save</Button>
                </Modal.Footer>
              </Modal>
        );
    }
});

var SearchForm = React.createClass({
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
            <form className="search-form" onSubmit={this.handleSubmit}>
                <div className="form-group">
                    <div className="input-group integrated">
                      <input type="text" className="form-control" placeholder="Search for documents" ref="search" />
                      <span className="input-group-addon">
                        <button type="submit">
                          <i className="fa fa-search"></i>
                        </button>
                      </span>
                    </div>
                </div> 
                <div className="form-group">
                    <label htmlFor="">Filter by libraries</label>
                    <Select multi value={this.state.libraries}
                            valueKey="Value"
                            labelKey="Text"
                            options={this.props.libraries}
                            placeholder="All libraries"
                            onChange={this.handleSelectChange} />
                </div>  
            </form>
        );
    }
});

var Result = React.createClass({
    render: function() {
        return (
            <div className="result media">
                <div className="media-left">
                    <a href={this.props.viewLink} target="_blank">
                        <img src={this.props.thumbnailLink} className="media-object thumbnail" alt={this.props.title} style={{width: 150 + 'px'}} />
                    </a>
                </div>
                <div className="media-body">
                    <h4 className="media-heading">
                        {this.props.title}
                    </h4>
                    {this.props.children}
                </div>
            </div>
        );
    }
});

var ResultList = React.createClass({
    render: function () {
        var documents = this.props.data.documents
            ? this.props.data.documents
            : [];
        var resultNodes = documents.map(function (result) {
            return (
                <Result key={result.id} title={result.title} thumbnailLink={result.thumbnailLink} viewLink={result.viewLink}>
                    {result.abstract}
                </Result>
            );
        });
        return (
            <div className="result-list clearfix" data-next-link={this.props.data.nextLink}>
                {resultNodes}
            </div>
        );
    }
});

var SearchBox = React.createClass({
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
    handleAddClose: function() {
        this.setState({ add: { show: false, files: [], file: null } });
    },
    handleFileDrop: function (files, event) {
        console.log(files, event);

        var add = this.state.add;
        add.file = files[0]; // files is read-only // todo: correctly track the file we are indexing
        add.files = files;
        add.show = true;

        this.setState({ add: add });
    },
    getInitialState: function() {
        return {
            data: [],
            add: {
                show: false,
                files: [],
                file: null
            }
        };
    },
    componentWillMount: function() {
    },
    render: function() {
        return (
            <div className="search-box row">
                <FileDrop frame={window}
                          onDrop={this.handleFileDrop}>
                    <i className="fa fa-cloud-upload fa-5x"></i>
                    <h4>Drag & Drop files here to index</h4>
                </FileDrop>
                <AddDocument libraries={this.props.libraries}
                             file={this.state.add.file}
                             onSubmit={this.props.onAddSubmit}
                             onClose={this.handleAddClose}
                             showModal={this.state.add.show} />
                <div className="col-sm-3">
                    <SearchForm libraries={this.props.libraries}
                                onSearchSubmit={this.handleSearchSubmit} 
                                onAddSubmit={this.handleAddSubmit} />
                </div>
                <div className="col-sm-9">
                    <ResultList data={this.state.data} />
                </div>
            </div>
        );
    }
});

module.exports = SearchBox;