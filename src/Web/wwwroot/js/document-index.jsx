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

        if (files.length === 0 || libraryId === '') {
            alert('Required fields are required.');
            return;
        }

        this.props.onSubmit(files, libraryId);
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
                    <select ref="library" className="form-control">
                        {libraryNodes}
                    </select>
                    <input type="file" ref="file" className="form-control" />
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
            <div className="result" style={{ width: 100 + 'px'}}>
                <img src={this.props.thumbnailUrl} className="thumbnail img-responsive" />
                <h1 className="title">
                    {this.props.title}
                </h1>
                {this.props.children}
            </div>
        );
    }
});

var ResultList = React.createClass({
    render: function () {
        var resultNodes = this.props.data.map(function (result) {
            return (
                <Result key={result.id} title={result.id} thumbnailUrl={result.thumbnailUrl}>
                    {result.id}
                </Result>
            );
        });
        return (
            <div className="result-list">
                {resultNodes}
            </div>
        );
    }
});

var SearchBox = React.createClass({
    loadResultsFromServer: function (search, libraryId) {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.searchUrl + "?search=" + search + "&libraryid=" + libraryId, true);
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
    handleAddSubmit: function (files, libraryId) {
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