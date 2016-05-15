var React = require('react');
var DocumentProperties = require('DocumentProperties');
var DocumentEdit = require('DocumentEdit');
var ResultList = require('ResultList');
var SearchForm = require('SearchForm');
var classNames = require('classnames');

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
    handleSearchSubmit: function (search, libraryIds) {
        this.loadResultsFromServer(search, libraryIds || []);
    },
    handleDocumentUpdate: function(location) {
    },
    handleDocumentClose: function () {
    },
    handleResultSelect: function (location) {
        var xhr = new XMLHttpRequest();
        xhr.open('get', location, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onload = function () {

            if (xhr.status === 200) {

                this.setState({
                    selectedDocument: JSON.parse(xhr.responseText),
                    editDocument: location
                });

                return;
            }

            alert(xhr.status + ' An error occurred!');

        }.bind(this);

        xhr.send();
    },
    getInitialState: function() {
        return {
            data: { documents: [] },
            selectedDocument: null,
            editDocument: null
        };
    },
    componentWillMount: function() {
    },
    render: function () {
        var resultsPanelClassNames = classNames({
            'col-sm-9': this.state.selectedDocument === null,
            'col-sm-7': this.state.selectedDocument !== null
        });
        var propertyPanelClassNames = classNames({
            'col-sm-2': this.state.selectedDocument !== null,
            'hidden': this.state.selectedDocument === null,
            'slide-in-right': true
        });
        return (
            <div className="search-box row">
                <DocumentEdit libraries={this.props.libraries}
                              location={this.state.editDocument}
                              onUpdate={this.handleDocumentUpdate}
                              onClose={this.handleDocumentClose} />
                <div className="col-sm-3">
                    <SearchForm 
                        libraries={this.props.libraries}
                        onSearchSubmit={this.handleSearchSubmit} />
                </div>
                <div className={resultsPanelClassNames}>
                    <ResultList 
                        data={this.state.data.documents}
                        nextLink={this.state.data.nextLink}
                        onSelect={this.handleResultSelect} />
                </div>
                <div className={propertyPanelClassNames}>
                    <DocumentProperties
                        document={this.state.selectedDocument || undefined} />
                </div>
            </div>
        );
    }
});

module.exports = SearchBox;