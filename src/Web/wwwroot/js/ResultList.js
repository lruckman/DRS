var React = require('react');
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
var Result = require('Result');

var ResultList = React.createClass({displayName: "ResultList",
    render: function () {
        var documents = this.props.data.documents
            ? this.props.data.documents
            : [];
        var resultNodes = documents.map(function (result) {
            return (
                React.createElement(Result, {
                    key: result.id, 
                    title: result.title, 
                    thumbnailLink: result.thumbnailLink, 
                    viewLink: result.viewLink
                }, 
                    result.abstract
                )
            );
        });
        return (
            React.createElement("div", {className: "result-list clearfix", "data-next-link": this.props.data.nextLink}, 
                React.createElement(ReactCSSTransitionGroup, {
                    transitionName: "result", 
                    transitionEnterTimeout: 500, 
                    transitionLeaveTimeout: 300
                }, 
                  resultNodes
                )
            )
        );
    }
});

module.exports = ResultList;