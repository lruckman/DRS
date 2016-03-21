var React = require('react');
var Select = require('react-select');

var SearchForm = React.createClass({displayName: "SearchForm",
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
            React.createElement("form", {className: "search-form", onSubmit: this.handleSubmit}, 
                React.createElement("div", {className: "form-group"}, 
                    React.createElement("div", {className: "input-group integrated"}, 
                      React.createElement("input", {type: "text", className: "form-control", placeholder: "Search for documents", ref: "search"}), 
                      React.createElement("span", {className: "input-group-addon"}, 
                        React.createElement("button", {type: "submit"}, 
                          React.createElement("i", {className: "fa fa-search"})
                        )
                      )
                    )
                ), 
                React.createElement("div", {className: "form-group"}, 
                    React.createElement("label", {htmlFor: ""}, "Filter by libraries"), 
                    React.createElement(Select, {multi: true, value: this.state.libraryIds, 
                            valueKey: "Value", 
                            labelKey: "Text", 
                            simpleValue: true, 
                            options: this.props.libraries, 
                            placeholder: "All libraries", 
                            onChange: this.handleSelectChange})
                )
            )
        );
    }
});

module.exports = SearchForm;