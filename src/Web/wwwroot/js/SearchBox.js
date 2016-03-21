var React = require('react');
var EditDocument = require('EditDocument');
var ResultList = require('ResultList');
var SearchForm = require('SearchForm');

var SearchBox = React.createClass({displayName: "SearchBox",
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
    handleSearchSubmit: function (search, libraryIds) {
        this.loadResultsFromServer(search, libraryIds || []);
    },
    handleDocumentUpdated: function (location) {
       
    },
    handleDocumentClose: function () {
    },
    
    getInitialState: function() {
        return {
            data: []
        };
    },
    componentWillMount: function() {
    },
    render: function() {
        return (
            React.createElement("div", {className: "search-box row"}, 
                React.createElement(EditDocument, {
                    libraries: this.props.libraries, 
                    source: this.props.addDocumentUrl, 
                    onUpdated: this.handleDocumentUpdated, 
                    onClose: this.handleDocumentClose}
                ), 
                React.createElement("div", {className: "col-sm-3"}, 
                    React.createElement(SearchForm, {
                        libraries: this.props.libraries, 
                        onSearchSubmit: this.handleSearchSubmit}
                    )
                ), 
                React.createElement("div", {className: "col-sm-9"}, 
                    React.createElement(ResultList, {
                        data: this.state.data}
                    )
                )
            )
        );
    }
});

module.exports = SearchBox;