var React = require('react');
var EditDocument = require('EditDocument');
var ResultList = require('ResultList');
var SearchForm = require('SearchForm');

var SearchBox = React.createClass({
    loadResultsFromServer: function (search, libraryIds) {
        var xhr = new XMLHttpRequest();

        xhr.open('get', this.props.searchUrl + "?q=" + search + "&libraryids=" + libraryIds.join("&libraryids="), true);
        xhr.setRequestHeader('Accept', 'application/json');
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
    handleSearchFormSubmit: function (search, libraryIds) {
        this.loadResultsFromServer(search, libraryIds || []);
    },
    handleDocumentUpdated: function (location) {
       
    },
    handleDocumentClose: function () {
    },
    handleResultListSelect: function(result) {
        
    },
    getInitialState: function() {
        return {
            data: { documents: [] },
            selected: { }
        };
    },
    componentWillMount: function() {
    },
    render: function() {
        return (
            <div className="search-box row">
                <EditDocument 
                    libraries={this.props.libraries}
                    source={this.props.addDocumentUrl}
                    onUpdated={this.handleDocumentUpdated}
                    onClose={this.handleDocumentClose}
                />
                <div className="col-sm-3">
                    <SearchForm 
                        libraries={this.props.libraries}
                        onSearchSubmit={this.handleSearchFormSubmit} 
                    />
                    <ResultList 
                        data={this.state.data.documents}
                        nextLink={this.state.data.nextLink}
                        onSelect={this.handleResultListSelect}
                    />
                </div>
                <div className="col-sm-9">
                </div>
            </div>
        );
    }
});

module.exports = SearchBox;