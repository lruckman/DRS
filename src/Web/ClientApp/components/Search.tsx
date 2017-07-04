import * as React from 'react';
import classNames from 'classnames';
import { DocumentResults, DocumentSearchForm } from '../document/containers';


export type SearchStateProps = {
    documentsSelected: boolean
}

export type SearchDispatchProps = { }

type OwnProps = SearchDispatchProps & SearchStateProps;

const Search = ({ documentsSelected }: OwnProps) => {
    const resultsPanelClassNames = classNames({
        'col-sm-9': !documentsSelected,
        'col-sm-7': documentsSelected
    });

    const propertyPanelClassNames = classNames({
        'col-sm-2': documentsSelected,
        'hidden': !documentsSelected,
        'slide-in-right': true
    });

    return <div className="search-box row">
        <div className="col-sm-3">
            <DocumentSearchForm />
        </div>
        <div className={resultsPanelClassNames}>
            <DocumentResults />
        </div>
        <div className={propertyPanelClassNames}>
           
        </div>
    </div>;
}

export default Search;