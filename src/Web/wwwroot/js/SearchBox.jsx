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
    searchHandleSubmit: function (search, libraryIds) {
        this.loadResultsFromServer(search, libraryIds || []);
    },
    documentHandleUpdate: function (location) {
       
    },
    documentHandleClose: function () {
    },
    resultHandleSelect: function (location) {
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
            selectedDocument: {
                pageCount: 0,
                fileSize: 0,
                title: '',
                abstract: '',
                libraryIds: [],
                thumbnailLink: '',
                location: '',
                icon: ''
            }
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
                    onUpdate={this.documentHandleUpdate}
                    onClose={this.documentHandleClose} />
                <div className="col-sm-3">
                    <SearchForm 
                        libraries={this.props.libraries}
                        onSearchSubmit={this.searchHandleSubmit} />
                    <ResultList 
                        data={this.state.data.documents}
                        nextLink={this.state.data.nextLink}
                        onSelect={this.resultHandleSelect} />
                </div>
                <div className="col-sm-9">
                    <img
                        src={this.state.selectedDocument.thumbnailLink}
                        className="img-responsive img-thumbnail" />
                </div>
            </div>
        );
    }
});

module.exports = SearchBox;