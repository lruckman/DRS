var React = require('react');
var DocumentProperties = require('DocumentProperties');
var EditDocument = require('EditDocument');
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
    searchHandleSubmit: function (search, libraryIds) {
        this.loadResultsFromServer(search, libraryIds || []);
    },
    documentHandleUpdate: function (location) {
       
    },
    documentHandleClose: function () {
    },
    resultListHandleSelect: function (location) {
        var xhr = new XMLHttpRequest();
        xhr.open('get', location, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onload = function () {

            if (xhr.status === 200) {

                this.setState({
                    selectedDocument: JSON.parse(xhr.responseText)
                });

                return;
            }

            alert(xhr.status + 'An error occurred!');

        }.bind(this);

        xhr.send();
    },
    getInitialState: function() {
        return {
            data: { documents: [] },
            selectedDocument: null
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
                <EditDocument 
                    libraries={this.props.libraries}
                    source={this.props.addDocumentUrl}
                    onUpdate={this.documentHandleUpdate}
                    onClose={this.documentHandleClose} />
                <div className="col-sm-3">
                    <SearchForm 
                        libraries={this.props.libraries}
                        onSearchSubmit={this.searchHandleSubmit} />
                </div>
                <div className={resultsPanelClassNames}>
                    <ResultList 
                        data={this.state.data.documents}
                        nextLink={this.state.data.nextLink}
                        onSelect={this.resultListHandleSelect} />
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