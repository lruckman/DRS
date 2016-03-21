var React = require('react');
var Result = require('Result');

var ResultList = React.createClass({displayName: "ResultList",
    render: function () {
        var documents = this.props.data.documents
            ? this.props.data.documents
            : [];
        var resultNodes = documents.map(function (result) {
            return (
                React.createElement(Result, {key: result.id, title: result.title, thumbnailLink: result.thumbnailLink, viewLink: result.viewLink}, 
                    result.abstract
                )
            );
        });
        return (
            React.createElement("div", {className: "result-list clearfix", "data-next-link": this.props.data.nextLink}, 
                resultNodes
            )
        );
    }
});

module.exports = ResultList;