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

type OwnState = {
    keywords: string
    , libraryIds: number[]
}

class DocumentSearchForm extends React.Component<OwnProps, OwnState> {

    constructor(props: OwnProps) {
        super(props);
        this.state = {
            keywords: props.keywords
            , libraryIds: props.libraryIds
        }

        this.handleLibraryChange = this.handleLibraryChange.bind(this);
        this.handleSearchKeyPress = this.handleSearchKeyPress.bind(this);
    }

    componentWillReceiveProps(nextProps: OwnProps) {
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

    handleSearchKeyPress(e: React.SyntheticEvent<HTMLInputElement>) {
        e.preventDefault();
        this.setState({
            ...this.state
            , keywords: e.currentTarget.value
        });
        this.props.onSearch(e.currentTarget.value, this.state.libraryIds);
    }

    public render() {

        const { keywords, libraryIds } = this.state;

        return <form className="search-form">
            <div className="form-group has-feedback">
                <input type="search" className="form-control" value={keywords}
                    onKeyPress={this.handleSearchKeyPress} placeholder="Search for documents" />
                <span className="form-control-feedback">
                    <i className="fa fa-search"></i>
                </span>
            </div>
            <div className="form-group">
                <label>Filter by libraries</label>
                {/*<UserLibraryPicker
                    selected={libraryIds}
                    onChange={this.handleLibraryChange}
                />*/}
            </div>
        </form>
    }
}

export default DocumentSearchForm;