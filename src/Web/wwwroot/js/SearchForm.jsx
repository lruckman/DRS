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
    handleSubmit: function(e) {
        e.preventDefault();

        var q = this.refs.search.value.trim();
        var libraryIds = this.state.libraryIds;

        this.search(q, libraryIds);
    },
    handleSelectChange (value) {
        var libraryIds = (typeof value === 'string')
            ? [value]
            : value;
        this.setState({ libraryIds: libraryIds });

        var q = this.refs.search.value.trim();
        this.search(q, libraryIds);
    },
    render: function () {
        return (
            <form className="search-form" onSubmit={this.handleSubmit}>
                <div className="form-group">
                    <div className="input-group integrated">
                      <input type="text" className="form-control" placeholder="Search for documents" ref="search" />
                      <span className="input-group-addon">
                        <button type="submit">
                          <i className="fa fa-search"></i>
                        </button>
                      </span>
                    </div>
                </div> 
                <div className="form-group">
                    <label htmlFor="">Filter by libraries</label>
                    <Select multi value={this.state.libraryIds}
                            valueKey="Value"
                            labelKey="Text"
                            simpleValue
                            options={this.props.libraries}
                            placeholder="All libraries"
                            onChange={this.handleSelectChange} />
                </div>  
            </form>
        );
    }
});

module.exports = SearchForm;