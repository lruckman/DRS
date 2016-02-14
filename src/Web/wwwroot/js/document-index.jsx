
var SearchForm = React.createClass({
    handleSubmit: function(e) {
        e.preventDefault();
        var query = ReactDOM.findDOMNode(this.refs.query).value.trim();
        this.props.onSearchSubmit(query);
        ReactDOM.findDOMNode(this.refs.query).value = '';
        return;
    },
    render: function() {
        return (
            <form className="search-form" onSubmit={this.handleSubmit}>
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
                <Result key={result.Id} title={result.Id}>
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
    loadResultsFromServer: function (query) {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.url + "?q=" + query, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ data: data });
        }.bind(this);
        xhr.send();
    },
    handleSearchSubmit: function (query) {
        this.loadResultsFromServer(query);
    },
    getInitialState: function() {
        return { data: [] };
    },
    componentWillMount: function() {
        this.loadResultsFromServer('');

    },
    render: function() {
        return (
            <div className="search-box">
                <SearchForm onSearchSubmit={this.handleSearchSubmit} />
                <ResultList data={this.state.data} />
            </div>
        );
    }
});

ReactDOM.render(
    <SearchBox url="/api/v1/search" />,
    document.getElementById('content')
);