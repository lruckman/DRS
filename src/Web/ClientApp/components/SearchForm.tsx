import * as React from 'react';
import Select from 'react-select';
import { DistributionGroup } from '../models';
import { UserLibraryPicker } from '../containers';

import 'react-select/dist/react-select.css';

export type SearchFormStateProps = {
    isFetching: boolean
    , keywords: string
    , libraryIds: number[]
}

export type SearchFormDispatchProps = {
    onSearch: (keywords: string, libraryIds: number[]) => void
}

type State = {
    keywords: string
    , libraryIds: number[]
}

class SearchForm extends React.Component<SearchFormStateProps & SearchFormDispatchProps, State> {

    constructor(props: SearchFormStateProps & SearchFormDispatchProps) {
        super(props);
        this.state = {
            keywords: props.keywords
            , libraryIds: props.libraryIds
        }

        this.handleLibraryChange = this.handleLibraryChange.bind(this);
        this.handleSearchKeyUp = this.handleSearchKeyUp.bind(this);
    }

    componentWillReceiveProps(nextProps: SearchFormStateProps & SearchFormDispatchProps) {
        this.setState({
            ...this.state
            , libraryIds: nextProps.libraryIds
            , keywords: nextProps.keywords
        })
    }

    handleLibraryChange(libraryIds: number[]) {
        this.setState({
            ...this.state
            , libraryIds
        });
        this.props.onSearch(this.state.keywords, libraryIds);
    }

    handleSearchKeyUp(e) {
        this.setState({
            ...this.state
            , keywords: e.target.value
        });
        this.props.onSearch(e.target.value, this.state.libraryIds);
    }

    public render() {

        const { keywords, libraryIds } = this.state;

        return <form className="search-form">
            <div className="form-group has-feedback">
                <input type="search" className="form-control" value={keywords}
                    onKeyUp={this.handleSearchKeyUp} placeholder="Search for documents" />
                <span className="form-control-feedback">
                    <i className="fa fa-search"></i>
                </span>
            </div>
            <div className="form-group">
                <label>Filter by libraries</label>
                <UserLibraryPicker selected={libraryIds} onChange={this.handleLibraryChange} />
            </div>
        </form>
    }
}

export default SearchForm;