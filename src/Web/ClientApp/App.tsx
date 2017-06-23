import * as React from 'react';
import classNames from 'classnames';
import DocumentProperties from './DocumentProperties';
import { SearchForm, DocumentList, DocumentEdit } from './containers';

//import { IDocumentDetails } from "./IDocumentDetails";
//import { ILibraryListItem } from './ILibraryListItem';

/*
interface ISearchResponse {
    documents: IResult[];
    nextLink: string;
}

interface IAppProp {
    addDocumentUrl?: string,
    libraries?: ILibraryListItem[];
}

interface IAppState {
    selectedDocument: IDocumentDetails;
    editDocument?: string;
    data: ISearchResponse;
}
*/

type Props = {
    documentsSelected: boolean
}

const App = (props: Props) => {

    let { documentsSelected } = props;

    let resultsPanelClassNames = classNames({
        'col-sm-9': !documentsSelected,
        'col-sm-7': documentsSelected
    });

    let propertyPanelClassNames = classNames({
        'col-sm-2': documentsSelected,
        'hidden': !documentsSelected,
        'slide-in-right': true
    });

    return <div className="search-box row">
        <DocumentEdit />
        <div className="col-sm-3">
            <SearchForm />
        </div>
        <div className={resultsPanelClassNames}>
            <DocumentList />
        </div>
        <div className={propertyPanelClassNames}>
            <DocumentProperties />
        </div>
    </div>;
}

export default App;

/*export default class App extends React.Component<Props, void> {
    
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
}
    */
