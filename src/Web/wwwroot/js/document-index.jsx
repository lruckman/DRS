
var SearchForm = React.createClass({
    handleSubmit: function(e) {
        e.preventDefault();
        var search = ReactDOM.findDOMNode(this.refs.search).value.trim();
        var libraryId = ReactDOM.findDOMNode(this.refs.library).value.trim();
        this.props.onSearchSubmit(search, libraryId);
        ReactDOM.findDOMNode(this.refs.search).value = '';
        return;
    },
    handleClick: function (e) {
        e.preventDefault();
        var libId = e.currentTarget.getAttribute('href').replace('#','').trim();
        var libName = e.currentTarget.innerHTML.trim();
        ReactDOM.findDOMNode(this.refs.filter).innerHTML = libName;
        ReactDOM.findDOMNode(this.refs.library).value = libId;
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
            <div className="result">
                <img src={this.props.thumbnailUrl} className="thumbnail" />
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
                <Result key={result.Id} title={result.Id} thumbnailUrl={result.ThumbnailUrl}>
                    {result.Id}
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
        xhr.open('get', this.props.url + "?search=" + search + "&libraryid=" + libraryId, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ data: data });
        }.bind(this);
        xhr.send();
    },
    handleSearchSubmit: function (search, libraryId) {
        this.loadResultsFromServer(search, libraryId);
    },
    getInitialState: function() {
        return { data: [] };
    },
    componentWillMount: function() {
    },
    render: function() {
        return (
            <div className="search-box">
                <SearchForm onSearchSubmit={this.handleSearchSubmit} libraries={this.props.libraries} />
                <ResultList data={this.state.data} />
            </div>
        );
    }
});