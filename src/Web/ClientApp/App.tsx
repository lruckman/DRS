import * as React from 'react';
import classNames from 'classnames';
import DocumentProperties from './DocumentProperties';
import DocumentEdit from './DocumentEdit';
import ResultList, { IResult } from './ResultList';
import SearchForm from './SearchForm';
import { IDocumentDetails } from "./IDocumentDetails";
import { ILibraryListItem } from './ILibraryListItem';

interface ISearchResponse {
    documents: IResult[];
    nextLink: string;
}

interface IAppProp {
    addDocumentUrl?: string,
    libraries?: ILibraryListItem[];
    searchUrl?: string;
}

interface IAppState {
    selectedDocument: IDocumentDetails;
    editDocument?: string;
    data: ISearchResponse;
}

export default class App extends React.Component<IAppProp, IAppState> {

    static defaultProps: IAppProp;

    constructor() {
        super();

        this.handleSearchSubmit = this.handleSearchSubmit.bind(this);
        this.handleDocumentUpdate = this.handleDocumentUpdate.bind(this);
        this.handleDocumentClose = this.handleDocumentClose.bind(this);
        this.handleResultSelect = this.handleResultSelect.bind(this);

        this.state = {
            data: { documents: [], nextLink: '' },
            selectedDocument: null,
            editDocument: null
        };
    }

    loadResultsFromServer(search: string, libraryIds: number[]) {
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
    }

    handleSearchSubmit(search: string, libraryIds: number[]) {
        this.loadResultsFromServer(search, libraryIds || []);
    }

    handleDocumentUpdate(location) { }

    handleDocumentClose() { }

    handleResultSelect(location: string) {
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
    }

    render() {
        var resultsPanelClassNames = classNames({
            'col-sm-9': this.state.selectedDocument === null,
            'col-sm-7': this.state.selectedDocument !== null
        });
        var propertyPanelClassNames = classNames({
            'col-sm-2': this.state.selectedDocument !== null,
            'hidden': this.state.selectedDocument === null,
            'slide-in-right': true
        });
        return <div className="search-box row">
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
        </div>;
    }
}

App.defaultProps = {
    addDocumentUrl: '',
    libraries: [],
    searchUrl: ''
}