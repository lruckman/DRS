import * as React from 'react';
import Select from 'react-select';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import * as SearchFormStore from '../../store/SearchForm';
import { Library } from '../../models';

import 'react-select/dist/react-select.css';

type Props = {
    error: Error
    , isFetching: boolean
    , keywords: string
    , libraryIds: number[]
    , libraryOptions: Library[]
    , onSearch: (keywords: string, libraryIds: number[]) => void
};

const SearchForm = (props: Props) => {

    let { error, isFetching, keywords, libraryIds, libraryOptions, onSearch } = props;

    let handleLibraryChange = (values: Library[]) =>
        onSearch(keywords, values.map(item => +item.value));

    let handleSearchKeyUp = (e) =>
        onSearch(e.target.value, libraryIds);

    return <form className="search-form">
        {error}
        <div className="form-group search">
            <i className="fa fa-search"></i>
            <input type="search" className="form-control" value={keywords || ''}
                onKeyUp={handleSearchKeyUp} placeholder="Search for documents" />
        </div>
        <div className="form-group">
            <label>Filter by libraries</label>
            <Select multi value={libraryIds} valueKey="id" labelKey="name"
                simpleValue options={libraryOptions} placeholder="All libraries"
                onChange={handleLibraryChange} />
        </div>
    </form>
}

export default SearchForm;