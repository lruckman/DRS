var React = require('react');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;
var FileDrop = require('react-file-drop');

var AddDocument = React.createClass({
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
              <Modal show={this.props.showModal} onHide={this.props.onClose}>
                <Modal.Header closeButton>
                  <Modal.Title>Add Document</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className="form-horizontal">
                        <div className="form-group">
                            <label  className="col-sm-2 control-label">File</label>
                            <div className="col-sm-10">
                                <div className="form-control-static">
                                    <strong>{this.props.file ? this.props.file.name : ""}</strong> <small className="text-muted">{this.props.file ? (this.props.file.size / 1024).toFixed(2) : ""} KB</small>
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <label htmlFor="title" className="col-sm-2 control-label">Title</label>
                            <div className="col-sm-10">
                                <input type="text" ref="title" id="title" className="form-control" placeholder="Title" autofocus />
                            </div>
                        </div>
                        <div className="form-group">
                            <label htmlFor="abstract" className="col-sm-2 control-label">Abstract</label>
                            <div className="col-sm-10">
                                <textarea ref="abstract" id="abstract" className="form-control" rows="3" placeholder="Abstract" disabled={this.state.disableAbstract}></textarea>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col-sm-offset-2 col-sm-10">
                                <div className="checkbox">
                                    <label>
                                        <input type="checkbox" ref="generateAbstract" defaultChecked={this.state.disableAbstract} onChange={this.handleGenerateAbstractChange} /> Automatically generate abstract
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <label htmlFor="libraries" className="col-sm-2 control-label">Libraries</label>
                            <div className="col-sm-10">
                                <Select multi value={this.state.libraryIds}
                                        placeholder="Libraries"
                                        simpleValue
                                        valueKey="Value"
                                        labelKey="Text"
                                        options={this.props.libraries}
                                        onChange={this.handleSelectChange} />
                            </div>
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                  <Button onClick={this.props.onClose}>Close</Button>
                    <Button onClick={this.save} bsStyle="primary">Save Changes</Button>
                </Modal.Footer>
              </Modal>
        );
    }
});

var SearchForm = React.createClass({
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
                    <Select multi value={this.state.libraryIds}
                            valueKey="Value"
                            labelKey="Text"
                            simpleValue
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
            <div className="search-box row">
                <FileDrop frame={window} targetAlwaysVisible={true}
                          onDrop={this.handleFileDrop}>
                    <i className="fa fa-cloud-upload fa-5x"></i>
                    <h4>Drop Files Here to Add</h4>
                </FileDrop>
                <AddDocument libraries={this.props.libraries}
                             file={this.state.add.file}
                             onSubmit={this.handleAddSubmit}
                             onClose={this.handleAddClose}
                             showModal={this.state.add.show} />
                <div className="col-sm-3">
                    <SearchForm libraries={this.props.libraries}
                                onSearchSubmit={this.handleSearchSubmit} />
                </div>
                <div className="col-sm-9">
                    <ResultList data={this.state.data} />
                </div>
            </div>
        );
    }
});

module.exports = SearchBox;