
var SearchForm = React.createClass({
    handleSubmit: function(e) {
        e.preventDefault();
        var query = ReactDOM.findDOMNode(this.refs.query).value.trim();
        var libraryId = ReactDOM.findDOMNode(this.refs.library).value.trim();
        this.props.onSearchSubmit(query, libraryId);
        ReactDOM.findDOMNode(this.refs.query).value = '';
        return;
    },
    render: function () {
        var libraryNodes = this.props.libraries.map(function(library) {
            return (
                <option key={library.Value} value={library.Value}>{library.Text}</option>
            );
        });
        return (
            <form className="search-form" onSubmit={this.handleSubmit}>
                <select ref="library">
                    {libraryNodes}
                </select>
                <input type="text" placeholder="Search" ref="query" />
                <input type="submit" value="search" />
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
    loadResultsFromServer: function (query, libraryId) {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.url + "?q=" + query + "&libraryid=" + libraryId, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ data: data });
        }.bind(this);
        xhr.send();
    },
    handleSearchSubmit: function (query, libraryId) {
        this.loadResultsFromServer(query, libraryId);
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