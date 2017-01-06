import * as React from 'react';
import Select from 'react-select';
import { ILibraryListItem } from './ILibraryListItem';

interface ISearchFormProp {
    onSearchSubmit?: (q: string, libraryIds: Array<number>) => void,
    libraries?: ILibraryListItem[]
}

interface ISearchFormState {
    libraryIds: number[]
}

export default class SearchForm extends React.Component<ISearchFormProp, ISearchFormState> {

    timer: any;

    refs: { search: (HTMLInputElement) }

    static defaultProps: ISearchFormProp;

    constructor() {
        super();

        this.searchHandleKeyUp = this.searchHandleKeyUp.bind(this);
        this.libraryHandleSelectChange = this.libraryHandleSelectChange.bind(this);

        this.state = {
            libraryIds: []
        };
    }
    
    search(q, libraryIds) {
        this.props.onSearchSubmit(q, libraryIds || []);
    }

    searchHandleKeyUp(e) {
        clearTimeout(this.timer = null);

        this.timer = setTimeout(function () {
            var q = this.refs.search.value.trim();
            var libraryIds = this.state.libraryIds;

            this.search(q, libraryIds);
        }.bind(this), 250);
    }

    libraryHandleSelectChange(value) {
        var libraryIds = (typeof value === 'string')
            ? [value]
            : value;
        this.setState({ libraryIds: libraryIds });

        var q = this.refs.search.value.trim();
        this.search(q, libraryIds);
    }

    render() {
        return <form className="search-form">
                <div className="form-group search">
                    <i className="fa fa-search"></i>
                    <input
                        type="search"
                        className="form-control"
                        onKeyUp={this.searchHandleKeyUp}
                        placeholder="Search for documents"
                        ref="search" />
                </div>
                <div className="form-group">
                    <label htmlFor="">Filter by libraries</label>
                    <Select
                        multi
                        value={this.state.libraryIds}
                        valueKey="value"
                        labelKey="text"
                        simpleValue
                        options={this.props.libraries}
                        placeholder="All libraries"
                        onChange={this.libraryHandleSelectChange} />
                </div>
            </form>;
    }
}

SearchForm.defaultProps = {
    onSearchSubmit: () => { },
    libraries: []
}