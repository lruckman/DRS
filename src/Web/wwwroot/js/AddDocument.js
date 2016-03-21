var React = require('react');
var Select = require('react-select');
var Button = require('react-bootstrap').Button;
var Modal = require('react-bootstrap').Modal;

var AddDocument = React.createClass({displayName: "AddDocument",
    getInitialState () {
        return {
            libraryIds: []
        };
    },
    save: function () {
        var title = this.refs.title.value.trim();
        var abstract = this.refs.abstract.value.trim();
        var libraryIds = this.state.libraryIds;

        this.props.onSubmit(this.props.file, libraryIds, title, abstract);

        this.setState({ libraryIds: [] });
    },
    handleSelectChange (value) {
        if (typeof value === 'string') {
            this.setState({ libraryIds: [value] });
            return;
        }
        this.setState({ libraryIds: value });
    },
    render: function () {
        return (
              React.createElement(Modal, {show: this.props.showModal, onHide: this.props.onClose}, 
                React.createElement(Modal.Header, {closeButton: true}, 
                  React.createElement(Modal.Title, null, "Add Document")
                ), 
                React.createElement(Modal.Body, null, 
                    React.createElement("form", {className: "form-horizontal"}, 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {className: "col-sm-2 control-label"}, "File"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("div", {className: "form-control-static"}, 
                                    React.createElement("strong", null, this.props.file ? this.props.file.name : ""), " ", React.createElement("small", {className: "text-muted"}, this.props.file ? (this.props.file.size / 1024).toFixed(2) : "", " KB")
                                )
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "title", className: "col-sm-2 control-label"}, "Title"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement("input", {type: "text", ref: "title", id: "title", className: "form-control", placeholder: "Title", autofocus: true})
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("div", {className: "col-sm-offset-2 col-sm-10"}, 
                                React.createElement("div", {className: "checkbox"}, 
                                    React.createElement("label", null, 
                                        React.createElement("input", {type: "checkbox", ref: "generateAbstract", defaultChecked: this.state.disableAbstract, onChange: this.handleGenerateAbstractChange}), " Automatically generate abstract"
                                    )
                                )
                            )
                        ), 
                        React.createElement("div", {className: "form-group"}, 
                            React.createElement("label", {htmlFor: "libraries", className: "col-sm-2 control-label"}, "Libraries"), 
                            React.createElement("div", {className: "col-sm-10"}, 
                                React.createElement(Select, {multi: true, value: this.state.libraryIds, 
                                        placeholder: "Libraries", 
                                        simpleValue: true, 
                                        valueKey: "Value", 
                                        labelKey: "Text", 
                                        options: this.props.libraries, 
                                        onChange: this.handleSelectChange})
                            )
                        )
                    )
                ), 
                React.createElement(Modal.Footer, null, 
                  React.createElement(Button, {onClick: this.props.onClose}, "Close"), 
                    React.createElement(Button, {onClick: this.save, bsStyle: "primary"}, "Save Changes")
                )
              )
        );
    }
});

module.exports = AddDocument;