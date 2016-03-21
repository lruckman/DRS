var React = require('react');

var Result = React.createClass({displayName: "Result",
    render: function() {
        return (
            React.createElement("div", {className: "result media"}, 
                React.createElement("div", {className: "media-left"}, 
                    React.createElement("a", {href: this.props.viewLink, target: "_blank"}, 
                        React.createElement("img", {src: this.props.thumbnailLink, className: "media-object thumbnail", alt: this.props.title, style: {width: 150 + 'px'}})
                    )
                ), 
                React.createElement("div", {className: "media-body"}, 
                    React.createElement("h4", {className: "media-heading"}, 
                        this.props.title
                    ), 
                    this.props.children
                )
            )
        );
    }
});

module.exports = Result;