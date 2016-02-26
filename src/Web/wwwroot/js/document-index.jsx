var AddDocument = React.createClass({
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
                <option key={library.Value} value={library.Value}>
                    {library.Text}
                </option>
            );
        }.bind(this));
        return (
              <div>
                <ReactBootstrap.Button bsStyle="default" bsSize="large" onClick={this.open}>
                <i className="fa fa-plus"></i>
                </ReactBootstrap.Button>

              <ReactBootstrap.Modal show={this.state.showModal} onHide={this.close}>
                <ReactBootstrap.Modal.Header closeButton>
                  <ReactBootstrap.Modal.Title>Add Document</ReactBootstrap.Modal.Title>
                </ReactBootstrap.Modal.Header>
                <ReactBootstrap.Modal.Body>
                    <form className="form-horizontal">
                        <div className="form-group">
                            <label htmlFor="library" className="col-sm-2 control-label">Library</label>
                            <div className="col-sm-10">
                                <select ref="library" id="library" className="form-control">{libraryNodes}
                                </select>
                            </div>
                        </div>
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
                </ReactBootstrap.Modal.Body>
                <ReactBootstrap.Modal.Footer>
                  <ReactBootstrap.Button onClick={this.close}>Close</ReactBootstrap.Button>
                    <ReactBootstrap.Button onClick={this.save}>Save</ReactBootstrap.Button>
                </ReactBootstrap.Modal.Footer>
              </ReactBootstrap.Modal>
            </div>
        );
    }
});

var SearchForm = React.createClass({
    handleSubmit: function(e) {
        e.preventDefault();
        var search = this.refs.search.value.trim();
        var libraryId = this.refs.library.value.trim();

        this.props.onSearchSubmit(search, libraryId);
        this.refs.search.value = '';

        return;
    },
    handleClick: function (e) {
        e.preventDefault();
        var libId = e.currentTarget.getAttribute('href').replace('#','').trim();
        var libName = e.currentTarget.innerHTML.trim();

        this.refs.filter.innerHTML = libName;
        this.refs.library.value = libId;
    },
    render: function () {
        var libraryNodes = this.props.libraries.map(function(library) {
            return (
                <li key={library.Value}>
                    <a href={"#" + library.Value} onClick={this.handleClick}>{library.Text}</a>
                </li>
            );
        }.bind(this));
        return (
            <form className="search-form" onSubmit={this.handleSubmit}>
                <AddDocument libraries={this.props.libraries} onSubmit={this.props.onAddSubmit} />
                <div className="container">
                    <div className="row">    
                        <div className="col-xs-8 col-xs-offset-2">
		                    <div className="input-group">
                                <div className="input-group-btn">
                                    <button type="button" className="btn btn-default dropdown-toggle" data-toggle="dropdown">
                    	                <span ref="filter">Filter by</span> <span className="caret"></span>
                                    </button>
                                    <ul className="dropdown-menu" role="menu">
                                      {libraryNodes}
                                    </ul>
                                </div>
                                <input type="hidden" ref="library" />         
                                <input type="text" className="form-control" placeholder="Search" ref="search" />
                                <span className="input-group-btn">
                                    <button className="btn btn-default" type="button">
                                        <span className="glyphicon glyphicon-search"></span>
                                    </button>
                                </span>
                            </div>
                        </div>
	                </div>
                </div>
            </form>
        );
    }
});

var Result = React.createClass({
    render: function() {
        return (
            <div className="result pull-left" style={{ padding: 15 + 'px', width: 200 + 'px'}}>
                <a href={this.props.viewLink} target="_blank">
                    <img src={this.props.thumbnailLink} className="thumbnail img-responsive" />
                    <h1 className="title">
                        {this.props.title}
                    </h1>
                    {this.props.children}
                </a>
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
    loadResultsFromServer: function (search, libraryId) {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.searchUrl + "?q=" + search + "&libraryid=" + libraryId, true);
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
                //this.setState({ data: data });
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
            <div className="search-box">
                <SearchForm onSearchSubmit={this.handleSearchSubmit} 
                            onAddSubmit={this.handleAddSubmit} 
                            libraries={this.props.libraries} />
                <ResultList data={this.state.data} />
            </div>
        );
    }
});