import * as React from 'react';
//import { UserLibraryPicker } from '../containers';

export type DocumentSearchFormStateProps = {
    keywords: string
    , libraryIds: number[]
}

export type DocumentSearchFormDispatchProps = {
    onSearch: (keywords: string, libraryIds: number[]) => void
}

type OwnProps = DocumentSearchFormDispatchProps & DocumentSearchFormStateProps;

class DocumentSearchForm extends React.Component<OwnProps, null> {

    constructor(props: OwnProps) {
        super(props);

        this.handleLibraryChange = this.handleLibraryChange.bind(this);
        this.handleSearchChange = this.handleSearchChange.bind(this);
    }

    handleLibraryChange(libraryIds: number[]) {
        this.props.onSearch(this.props.keywords, libraryIds);
    }

    handleSearchChange(e: React.SyntheticEvent<HTMLInputElement>) {
        e.preventDefault();
        this.props.onSearch(e.currentTarget.value, this.props.libraryIds);
    }

    public render() {
        
        return <form className="search-form">
            <div className="form-group has-feedback">
                <input
                    type="search"
                    className="form-control"
                    value={this.props.keywords}
                    onChange={this.handleSearchChange}
                    placeholder="Search for documents"
                />
                <span className="form-control-feedback">
                    <i className="fa fa-search"></i>
                </span>
            </div>
            {/*<div className="form-group">
                <label>Filter by libraries</label>
                <UserLibraryPicker
                    selected={libraryIds}
                    onChange={this.handleLibraryChange}
                />
            </div>*/}
        </form>
    }
}

export default DocumentSearchForm;