var React = require('react');
var Select = require('react-select');

var SearchForm = React.createClass({
    getInitialState () {
        return {
            libraryIds: []
        };
    },
    search: function(q, libraryIds) {
        this.props.onSearchSubmit(q, libraryIds || []);
    },
    searchHandleKeyUp: function (e) {
        if (e.keyCode !== 13) {
            return;
        }

        e.preventDefault();

        var q = this.refs.search.value.trim();
        var libraryIds = this.state.libraryIds;

        this.search(q, libraryIds);
    },
    libraryHandleSelectChange (value) {
        var libraryIds = (typeof value === 'string')
            ? [value]
            : value;
        this.setState({ libraryIds: libraryIds });

        var q = this.refs.search.value.trim();
        this.search(q, libraryIds);
    },
    render: function () {
        return (
            <form className="search-form">
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
                        valueKey="Value"
                        labelKey="Text"
                        simpleValue
                        options={this.props.libraries}
                        placeholder="All libraries"
                        onChange={this.libraryHandleSelectChange} />
                </div>  
            </form>
        );
    }
});

module.exports = SearchForm;